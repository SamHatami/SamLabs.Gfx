using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Manipulators;

public class GLManipulatorRenderSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.ManipulatorRender;
    private const float manipulatorBaseSize = 0.01f;
    private readonly EntityRegistry _entityRegistry;

    public GLManipulatorRenderSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var manipulatorEntities = ComponentRegistry.GetEntityIdsForComponentType<ManipulatorComponent>();
        if (manipulatorEntities.IsEmpty) return;

        var pickingEntity = ComponentRegistry.GetEntityIdsForComponentType<PickingDataComponent>()[0];
        var pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity);

        if (pickingData.IsSelectionEmpty()) return;

        var activemanipulator = ComponentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (activemanipulator.IsEmpty) return;
        if (activemanipulator.Length > 1) return; //Only one manipulator can be active at a time.

        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched
        var subEntities = ComponentRegistry.GetChildEntitiesForParent(activemanipulator[0], childBuffer);
        var selectedmanipulators = _entityRegistry.Query.With<SelectedManipulatorChildComponent>().Get();
        var isAnymanipulatorDragging = !selectedmanipulators.IsEmpty() && frameInput.IsMouseLeftButtonDown;

        DrawManipulator(activemanipulator[0], subEntities, pickingData, childBuffer, isAnymanipulatorDragging);
    }

    private void DrawManipulator(int activemanipulator, ReadOnlySpan<int> manipulatorSubEntities,
        PickingDataComponent pickingData,
        Span<int> childBuffer, bool isDragging)
    {
        ref var parentTransform = ref ComponentRegistry.GetComponent<TransformComponent>(activemanipulator);
        UpdateChildmanipulators(activemanipulator, manipulatorSubEntities, childBuffer,
            ref parentTransform); //Special case for the manipulator

        GL.Clear(ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
        foreach (var manipulatorSubEntity in manipulatorSubEntities)
        {
            var isSelected = CheckSelection(manipulatorSubEntity, pickingData);
            var mesh = ComponentRegistry.GetComponent<GlMeshDataComponent>(manipulatorSubEntity);
            var material = ComponentRegistry.GetComponent<MaterialComponent>(manipulatorSubEntity);
            var submanipulatorTransform = ComponentRegistry.GetComponent<TransformComponent>(manipulatorSubEntity);
            var manipulatorChildComponent =
                ComponentRegistry.GetComponent<ManipulatorChildComponent>(manipulatorSubEntity);

            RenderManipualtorSubMesh(mesh, material, isSelected, isDragging, submanipulatorTransform.WorldMatrix,
                pickingData, manipulatorSubEntity, manipulatorChildComponent);
        }

        GL.Disable(EnableCap.DepthTest);
    }

    private bool CheckSelection(int manipulatorSubEntity, PickingDataComponent pickingData)
    {
        return !pickingData.IsSelectionEmpty() &&
               ComponentRegistry.HasComponent<SelectedManipulatorChildComponent>(manipulatorSubEntity);
    }

    private void UpdateChildmanipulators(int activeManipulator, ReadOnlySpan<int> manipulatorSubEntities,
        Span<int> childBuffer,
        ref TransformComponent parentTransform)
    {
        var cameraEntities = ComponentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.IsEmpty) return;

        ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(cameraEntities[0]);

        //Todo create a utility class for this => Move to ScaleToScreenSystem when adding ScaleToScreenComponent
        
        var scaledMatrix = ScaleToView(parentTransform, cameraTransform, cameraData);

        foreach (var subEntity in manipulatorSubEntities)
        {
            ref var subTransform = ref ComponentRegistry.GetComponent<TransformComponent>(subEntity);

            subTransform.WorldMatrix = subTransform.LocalMatrix * scaledMatrix;
            subTransform.IsDirty = false;
        }
    }

    private Matrix4 ScaleToView(TransformComponent parentTransform, TransformComponent cameraTransform,
        CameraDataComponent cameraData)
    {
        var toManipulator = parentTransform.Position - cameraTransform.Position;
        var forward = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        var depth = Vector3.Dot(toManipulator, forward);
        if (depth < 0.1f) depth = 0.1f;

        var fovScale = 2.0f;

        if(cameraData.ProjectionType == ProjectionType.Perspective)
            fovScale = 2.0f * depth * MathF.Tan(cameraData.Fov * 0.5f);
        else
            fovScale = cameraData.OrthographicSize * 1.5f;
        var scale = manipulatorBaseSize * fovScale;
        


        var parentRot = parentTransform.LocalMatrix.ExtractRotation();
        var parentPos = parentTransform.LocalMatrix.ExtractTranslation();

        var parentMatrix = Matrix4.CreateScale(scale)
                           * Matrix4.CreateFromQuaternion(parentRot)
                           * Matrix4.CreateTranslation(parentPos);
        return parentMatrix;
    }


    private void RenderManipualtorSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent,
        bool isSelected, bool isDragging,
        Matrix4 modelMatrix, PickingDataComponent pickingData, int entityId,
        ManipulatorChildComponent manipulatorChildComponent)
    {
        var isHovered = isDragging
            ? (isSelected ? 1 : 0) // During drag: only selected is highlighted
            : (pickingData.HoveredEntityId == entityId ? 1 : 0); // Not dragging: use picking
        var axis = manipulatorChildComponent.Axis.ToInt();
        var selected = isSelected ? 1 : 0;

        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetInt(UniformNames.uIsHovered, ref isHovered)
            .SetInt(UniformNames.uIsSelected, ref selected)
            .SetInt(UniformNames.uManipulatorAxis, ref axis);
        MeshRenderer.Draw(mesh);
    }
}