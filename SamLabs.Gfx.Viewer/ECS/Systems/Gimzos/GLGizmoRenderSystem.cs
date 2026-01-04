using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Gimzos;

public class GLGizmoRenderSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.GizmoRender;
    private const float GizmoBaseSize = 0.015f;

    public GLGizmoRenderSystem(EntityManager entityManager) : base(entityManager)
    {
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GizmoComponent>();
        if (gizmoEntities.IsEmpty) return;
        
        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>()[0];
        var pickingData = ComponentManager.GetComponent<PickingDataComponent>(pickingEntity);

        var activeGizmo = ComponentManager.GetEntityIdsForComponentType<ActiveGizmoComponent>();
        if (activeGizmo.IsEmpty) return;
        if (activeGizmo.Length > 1) return; //Only one gizmo can be active at a time.

        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched
        var subEntities = ComponentManager.GetChildEntitiesForParent(activeGizmo[0], childBuffer);
        var selectedGizmos = GetEntityIds.With<SelectedChildGizmoComponent>();
        bool isAnyGizmoDragging = !selectedGizmos.IsEmpty && frameInput.IsMouseLeftButtonDown;
        
        DrawGizmo(activeGizmo[0], subEntities, pickingData, childBuffer, isAnyGizmoDragging);
    }

    private void DrawGizmo(int activeGizmo, ReadOnlySpan<int> gizmoSubEntities, PickingDataComponent pickingData,
        Span<int> childBuffer, bool isDragging)
    {
        ref var parentTransform = ref ComponentManager.GetComponent<TransformComponent>(activeGizmo);
        UpdateChildGizmos(activeGizmo, gizmoSubEntities, childBuffer,ref parentTransform); //Special case for the gizmo

        foreach (var gizmoSubEntity in gizmoSubEntities)
        {
            var isSelected = CheckSelection(gizmoSubEntity, pickingData);
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(gizmoSubEntity);
            var material = ComponentManager.GetComponent<MaterialComponent>(gizmoSubEntity);
            var subGizmoTransform = ComponentManager.GetComponent<TransformComponent>(gizmoSubEntity);
            var gizmoChildComponent = ComponentManager.GetComponent<GizmoChildComponent>(gizmoSubEntity);
            
            RenderGizmoSubMesh(mesh, material, isSelected, isDragging, subGizmoTransform.WorldMatrix(), pickingData, gizmoSubEntity, gizmoChildComponent);
        }
    }

    private bool CheckSelection(int gizmoSubEntity, PickingDataComponent pickingData)
    {
        return !pickingData.IsSelectionEmpty() && ComponentManager.HasComponent<SelectedChildGizmoComponent>(gizmoSubEntity);
    }

    private void UpdateChildGizmos(int activeGizmo, ReadOnlySpan<int> gizmoSubEntities, Span<int> childBuffer,
        ref TransformComponent parentTransform)
    {
        var cameraEntities = ComponentManager.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.IsEmpty) return;

        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        
        //Todo create a utility class for this
        var toGizmo = parentTransform.Position - cameraTransform.Position;
        var forward = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        var depth = Vector3.Dot(toGizmo, forward);
        if (depth < 0.1f) depth = 0.1f;

        var scale = GizmoBaseSize * depth;
        parentTransform.Scale = new Vector3(scale);
    
        foreach (var subEntity in gizmoSubEntities)
        {
            ref var subTransform = ref ComponentManager.GetComponent<TransformComponent>(subEntity);
            var rotatedLocalPos = Vector3.Transform(subTransform.LocalPosition, parentTransform.Rotation);
            var scaledRotatedPos = rotatedLocalPos * scale;
            subTransform.Position = parentTransform.Position + scaledRotatedPos;
            subTransform.Scale = parentTransform.Scale * subTransform.LocalScale;
            subTransform.Rotation = parentTransform.Rotation * subTransform.LocalRotation;
        }
    }



    private void RenderGizmoSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, bool isSelected, bool isDragging,
        Matrix4 modelMatrix, PickingDataComponent pickingData, int entityId,
        GizmoChildComponent gizmoChildComponent)
    {
        
        var isHovered = isDragging 
            ? (isSelected ? 1 : 0)  // During drag: only selected is highlighted
            : (pickingData.HoveredEntityId == entityId ? 1 : 0);  // Not dragging: use picking
        var axis = gizmoChildComponent.Axis.ToInt();
        var selected = isSelected ? 1 : 0;

        // Console.WriteLine($"IsHovered: {isHovered}, IsSelected: {isSelected}");
        GL.Disable(EnableCap.DepthTest);
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetInt(UniformNames.uIsHovered, ref isHovered)
            .SetInt(UniformNames.uIsSelected, ref selected)
            .SetInt(UniformNames.uGizmoAxis, ref axis);
        MeshRenderer.Draw(mesh);

        GL.Enable(EnableCap.DepthTest);
    }
}