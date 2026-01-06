using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Gimzos;

public class GLManipulatorRenderSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.ManipulatorRender;
    private const float manipulatorBaseSize = 0.015f;

    public GLManipulatorRenderSystem(EntityManager entityManager) : base(entityManager)
    {
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var manipulatorEntities = ComponentManager.GetEntityIdsForComponentType<ManipulatorComponent>();
        if (manipulatorEntities.IsEmpty) return;
        
        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>()[0];
        var pickingData = ComponentManager.GetComponent<PickingDataComponent>(pickingEntity);

        var activemanipulator = ComponentManager.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        if (activemanipulator.IsEmpty) return;
        if (activemanipulator.Length > 1) return; //Only one manipulator can be active at a time.

        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched
        var subEntities = ComponentManager.GetChildEntitiesForParent(activemanipulator[0], childBuffer);
        var selectedmanipulators = GetEntityIds.With<SelectedManipulatorChildComponent>();
        bool isAnymanipulatorDragging = !selectedmanipulators.IsEmpty && frameInput.IsMouseLeftButtonDown;
        
        Drawmanipulator(activemanipulator[0], subEntities, pickingData, childBuffer, isAnymanipulatorDragging);
    }

    private void Drawmanipulator(int activemanipulator, ReadOnlySpan<int> manipulatorSubEntities, PickingDataComponent pickingData,
        Span<int> childBuffer, bool isDragging)
    {
        ref var parentTransform = ref ComponentManager.GetComponent<TransformComponent>(activemanipulator);
        UpdateChildmanipulators(activemanipulator, manipulatorSubEntities, childBuffer,ref parentTransform); //Special case for the manipulator

        foreach (var manipulatorSubEntity in manipulatorSubEntities)
        {
            var isSelected = CheckSelection(manipulatorSubEntity, pickingData);
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(manipulatorSubEntity);
            var material = ComponentManager.GetComponent<MaterialComponent>(manipulatorSubEntity);
            var submanipulatorTransform = ComponentManager.GetComponent<TransformComponent>(manipulatorSubEntity);
            var manipulatorChildComponent = ComponentManager.GetComponent<ManipulatorChildComponent>(manipulatorSubEntity);
            
            RenderManipualtorSubMesh(mesh, material, isSelected, isDragging, submanipulatorTransform.WorldMatrix, pickingData, manipulatorSubEntity, manipulatorChildComponent);
        }
    }

    private bool CheckSelection(int manipulatorSubEntity, PickingDataComponent pickingData)
    {
        return !pickingData.IsSelectionEmpty() && ComponentManager.HasComponent<SelectedManipulatorChildComponent>(manipulatorSubEntity);
    }

    private void UpdateChildmanipulators(int activeManipulator, ReadOnlySpan<int> manipulatorSubEntities, Span<int> childBuffer,
        ref TransformComponent parentTransform)
    {
        var cameraEntities = ComponentManager.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.IsEmpty) return;

        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        
        //Todo create a utility class for this
        var toManipulator = parentTransform.Position - cameraTransform.Position;
        var forward = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        var depth = Vector3.Dot(toManipulator, forward);
        if (depth < 0.1f) depth = 0.1f;
        var scale = manipulatorBaseSize * depth;
    
        var parentRot = parentTransform.LocalMatrix.ExtractRotation();
        var parentPos = parentTransform.LocalMatrix.ExtractTranslation();

        var parentMatrix = Matrix4.CreateScale(scale) 
                                * Matrix4.CreateFromQuaternion(parentRot) 
                                * Matrix4.CreateTranslation(parentPos);
        
        foreach (var subEntity in manipulatorSubEntities)
        {
            ref var subTransform = ref ComponentManager.GetComponent<TransformComponent>(subEntity);

            subTransform.WorldMatrix = subTransform.LocalMatrix * parentMatrix;
            subTransform.IsDirty = false;
        }
    }



    private void RenderManipualtorSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, bool isSelected, bool isDragging,
        Matrix4 modelMatrix, PickingDataComponent pickingData, int entityId,
        ManipulatorChildComponent manipulatorChildComponent)
    {
        
        var isHovered = isDragging 
            ? (isSelected ? 1 : 0)  // During drag: only selected is highlighted
            : (pickingData.HoveredEntityId == entityId ? 1 : 0);  // Not dragging: use picking
        var axis = manipulatorChildComponent.Axis.ToInt();
        var selected = isSelected ? 1 : 0;

        // Console.WriteLine($"IsHovered: {isHovered}, IsSelected: {isSelected}");
        GL.Disable(EnableCap.DepthTest);
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetInt(UniformNames.uIsHovered, ref isHovered)
            .SetInt(UniformNames.uIsSelected, ref selected)
            .SetInt(UniformNames.uManipulatorAxis, ref axis);
        MeshRenderer.Draw(mesh);

        GL.Enable(EnableCap.DepthTest);
    }
}