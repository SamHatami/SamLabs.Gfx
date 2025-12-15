using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderMeshSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.MainRender;

    public GLRenderMeshSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        //Get all glmeshes and render them, in the future we will allow to hide meshes aswell
        var meshEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;

        foreach (var meshEntity in meshEntities)
        {
            var transform = ComponentManager.GetComponent<TransformComponent>(meshEntity);
            var modelMatrix = transform.WorldMatrix; //Todo this should be updated every frame when the model is moving
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(meshEntity);
            if (mesh.IsGizmo) continue;
            var materials = ComponentManager.GetComponent<MaterialComponent>(meshEntity);
            RenderMesh(mesh, materials, modelMatrix.Invoke());
        }
    }

    private void RenderMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent,
        Matrix4 modelMatrix = default)
    {
        var shaderProgram = materialComponent.Shader.ProgramId;
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(mesh.Vao);
        GL.UniformMatrix4f(materialComponent.Shader.UniformLocations[UniformNames.uModel].Location, 1, false,
            ref modelMatrix);

        if (mesh.Ebo > 0)
        {
            GL.DrawElements(mesh.PrimitiveType, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(mesh.PrimitiveType, 0, mesh.VertexCount);
        }

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
}