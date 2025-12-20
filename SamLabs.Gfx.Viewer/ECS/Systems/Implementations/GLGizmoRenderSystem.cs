using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLGizmoRenderSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.GizmoRender;
    private const float GizmoBaseSize = 1.0f;

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
        DrawGizmo(activeGizmo[0], subEntities, pickingData, childBuffer);
    }

    private void DrawGizmo(int activeGizmo, ReadOnlySpan<int> gizmoSubEntities, PickingDataComponent pickingData,
        Span<int> childBuffer)
    {
        ScaleToView(activeGizmo, gizmoSubEntities, childBuffer);

        foreach (var gizmoSubEntity in gizmoSubEntities)
        {
            var selected = ComponentManager.GetComponent<SelectedComponent>(gizmoSubEntity);
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(gizmoSubEntity);
            var material = ComponentManager.GetComponent<MaterialComponent>(gizmoSubEntity);
            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(gizmoSubEntity).WorldMatrix;
            RenderGizmoSubMesh(mesh, material, true, modelMatrix.Invoke(), pickingData, gizmoSubEntity);
        }
    }

    private void ScaleToView(int activeGizmo, ReadOnlySpan<int> gizmoSubEntities, Span<int> childBuffer)
    {
        var cameraEntities = ComponentManager.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.IsEmpty) return;

        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        ref var gizmoTransform = ref ComponentManager.GetComponent<TransformComponent>(activeGizmo);

        var distance = Vector3.Distance(cameraTransform.Position, gizmoTransform.Position);
        if (distance < 0.1f) distance = 0.1f;
        var scale = distance * MathF.Tan(MathHelper.DegreesToRadians(cameraData.Fov) * 0.5f) * GizmoBaseSize;
        gizmoTransform.Scale = new Vector3(scale);

        //Scale arrows/plane handles
        foreach (var subEntity in gizmoSubEntities)
        {
            ref var subTransform = ref ComponentManager.GetComponent<TransformComponent>(subEntity);
            subTransform.Scale = new Vector3(scale);
        }
    }

    private void RenderGizmoSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, bool isSelected,
        Matrix4 modelMatrix, PickingDataComponent pickingData, int entityId = -1)
    {
        // var highlightShaderProgram = materialComponent.HighlightShader.ProgramId;
        var pickingDataHoveredEntityId = pickingData.HoveredEntityId;
        var hovered = (pickingDataHoveredEntityId == entityId) ? 1 : 0;

        GL.Disable(EnableCap.DepthTest);
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetInt(UniformNames.uIsHovered, ref hovered);
        MeshRenderer.Draw(mesh);

        GL.Enable(EnableCap.DepthTest);
    }
}