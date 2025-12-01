using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class GLRenderMeshSystem : RenderSystem
{
    public GLRenderMeshSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update(RenderContext renderContext)
    {
        //Get all glmeshes and render them, in the future we will allow to hide meshes aswell
        var meshEntities = ComponentManager.GetEntityIdsFor<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;

        foreach (var meshEntity in meshEntities)
        {
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(meshEntity);
            var materials = ComponentManager.GetComponent<MaterialComponent>(meshEntity);
            RenderMesh(mesh, materials);
        }
    }

    private void RenderMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent)
    {
        var shaderProgram = materialComponent.Shader.ProgramId;
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(mesh.Vao);
        // GL.UniformMatrix4f(_mvpLocation, 1, false, ref model);

        if (mesh.Ebo > 0)
        {
            GL.DrawElements(mesh.PrimitiveType, mesh.VertexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(mesh.PrimitiveType, 0, mesh.VertexCount);
        }

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
}