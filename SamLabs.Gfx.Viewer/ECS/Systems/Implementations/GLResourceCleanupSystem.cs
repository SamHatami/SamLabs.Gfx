using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;


public class GLResourceCleanupSystem: PostRenderSystem
{
    public GLResourceCleanupSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update()
    {
        var entityIds = _componentManager.GetEntityIdsFor<GlMeshRemoved>();
        if (entityIds.Length == 0) return;
        
        
        foreach (var entityId in entityIds)
        {
            ref var glData = ref _componentManager.GetComponent<GlMeshDataComponent>(entityId);
            DisposeGLResources(ref glData);
            _componentManager.RemoveComponentFromEntity<GlMeshRemoved>(entityId);
        }
    }

    private void DisposeGLResources(ref GlMeshDataComponent glMeshData)
    {
        GL.DeleteVertexArray(glMeshData.Vao);
        GL.DeleteBuffer(glMeshData.Vbo);
        if (glMeshData.Ebo != 0) GL.DeleteBuffer(glMeshData.Ebo);
    }
 
}