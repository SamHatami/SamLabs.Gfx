using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderMeshSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.MainRender;

    public GLRenderMeshSystem(EntityManager entityManager) : base(entityManager)
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
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel,ref modelMatrix);
        MeshRenderer.Draw(mesh);
    }
}