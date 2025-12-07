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

public class GLRenderGizmoSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoRender;

    public GLRenderGizmoSystem(ComponentManager componentManager) : base(componentManager)
    {
    }


    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        Span<int> childBuffer = stackalloc int[12]; //Make sure only the active parents children are fetched

        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GizmoComponent>();
        if (gizmoEntities.IsEmpty) return;

        int gizmoActiveCount = 0;
        foreach (var gizmoEntity in gizmoEntities)
        {
            var gizmoComponent = ComponentManager.HasComponent<VisibilityComponent>(gizmoEntity);
            if (!gizmoComponent) continue;

#if DEBUG
            gizmoActiveCount++;
            if (gizmoActiveCount > 1)
            {
                throw new InvalidOperationException("Only one gizmo can be active at a time. You're doing samthing wrong SAM.");
            }
#endif
            var subEntities = ComponentManager.GetChildEntitiesForParent(gizmoEntity, childBuffer);
            DrawGizmo(subEntities);
        }
    }

    private void DrawGizmo(ReadOnlySpan<int> gizmoSubEntities)
    {
        //get the meshes
        //get shader program
        //draw

        foreach (var gizmoSubEntity in gizmoSubEntities)
        {
            var selected = ComponentManager.GetComponent<SelectedComponent>(gizmoSubEntity);
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(gizmoSubEntity);
            var material = ComponentManager.GetComponent<MaterialComponent>(gizmoSubEntity);
            RenderGizmoSubMesh(mesh, material, false);
        }
        
        //same as meshrendering system ish
        
        //highlight if something is selected?
        //get highlightshader
        //use it
        //draw
    }
    
    private void RenderGizmoSubMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, bool isSelected ,Matrix4 modelMatrix = default)
    {
        var shaderProgram = materialComponent.Shader.ProgramId;
        var highlightShaderProgram = materialComponent.HighlightShader.ProgramId;
        
        if(isSelected)
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(mesh.Vao);
        GL.UniformMatrix4f(materialComponent.Shader.MatrixModelUniformLocation, 1, false, ref modelMatrix);
        GL.DrawElements(mesh.PrimitiveType, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
    
    
}