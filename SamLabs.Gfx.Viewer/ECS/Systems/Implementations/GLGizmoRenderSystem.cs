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
    public override int RenderPosition => RenderOrders.GizmoRender;

    public GLGizmoRenderSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager,
        entityManager)
    {
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched

        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GizmoComponent>();
        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>()[0];
        var pickingData = ComponentManager.GetComponent<PickingDataComponent>(pickingEntity);
        if (gizmoEntities.IsEmpty) return;

        int gizmoActiveCount = 0;
        foreach (var gizmoEntity in gizmoEntities)
        {
            var gizmoComponent = ComponentManager.HasComponent<ActiveGizmoComponent>(gizmoEntity);
            if (!gizmoComponent) continue;

#if DEBUG
            gizmoActiveCount++;
            if (gizmoActiveCount > 1)
            {
                throw new InvalidOperationException(
                    "Only one gizmo can be active at a time. You're doing samthing wrong SAM.");
            }
#endif
            var subEntities = ComponentManager.GetChildEntitiesForParent(gizmoEntity, childBuffer);
            DrawGizmo(subEntities, pickingData);
        }
    }

    private void DrawGizmo(ReadOnlySpan<int> gizmoSubEntities, PickingDataComponent pickingData)
    {
        //get the meshes
        //get shader program
        //draw

        foreach (var gizmoSubEntity in gizmoSubEntities)
        {
            var selected = ComponentManager.GetComponent<SelectedComponent>(gizmoSubEntity);
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(gizmoSubEntity);
            var material = ComponentManager.GetComponent<MaterialComponent>(gizmoSubEntity);
            var modelMatrix = ComponentManager.GetComponent<TransformComponent>(gizmoSubEntity).WorldMatrix;
            RenderGizmoSubMesh(mesh, material, true, modelMatrix.Invoke(), pickingData, gizmoSubEntity);
        }

        //same as meshrendering system ish

        //highlight if something is selected?
        //get highlightshader
        //use it
        //draw
    }

    private void RenderGizmoSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, bool isSelected,
        Matrix4 modelMatrix, PickingDataComponent pickingData, int entityId = -1)
    {
        var shaderProgram = materialComponent.Shader.ProgramId;
        // var highlightShaderProgram = materialComponent.HighlightShader.ProgramId;
        var pickingDataHoveredEntityId = pickingData.HoveredEntityId;
        int hovered = (pickingDataHoveredEntityId == entityId) ? 1 : 0;

        GL.Disable(EnableCap.DepthTest);
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetInt(UniformNames.uIsHovered, ref hovered);
        MeshRenderer.Draw(mesh);

        GL.Enable(EnableCap.DepthTest);
    }
}