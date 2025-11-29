using Assimp;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class RenderMeshSystem: RenderSystem
{
    public RenderMeshSystem(ComponentManager componentManager) : base(componentManager)
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
            RenderMesh(mesh);
        }
    }

    private void RenderMesh(GlMeshDataComponent mesh)
    {
        GL.BindVertexArray(mesh.Vao);
    
        if (mesh.VertexCount != 0 ) 
        {
            GL.DrawElements(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, mesh.VertexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, 0, mesh.VertexCount);
        }
    
        GL.BindVertexArray(0);
    }
}