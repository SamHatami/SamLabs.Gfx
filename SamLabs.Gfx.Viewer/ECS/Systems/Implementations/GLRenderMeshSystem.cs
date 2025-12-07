using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderMeshSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.MainRender;
    public GLRenderMeshSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
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
        // int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
        // var modelMatrix = Matrix4.Identity;
        // GL.UniformMatrix4f(modelLocation, 1, false, ref modelMatrix);

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