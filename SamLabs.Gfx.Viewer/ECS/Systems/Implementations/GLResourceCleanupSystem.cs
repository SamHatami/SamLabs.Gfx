using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;


[RenderPassAttributes.RenderOrder(RenderOrders.CleanUp)]
public class GLResourceCleanupSystem: RenderSystem
{
    public override int RenderPosition => RenderOrders.CleanUp;
    public GLResourceCleanupSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var entityIds = ComponentManager.GetEntityIdsForComponentType<GlMeshRemoved>();
        if (entityIds.Length == 0) return;
        
        
        foreach (var entityId in entityIds)
        {
            ref var glData = ref ComponentManager.GetComponent<GlMeshDataComponent>(entityId);
            DisposeGLResources(ref glData);
            ComponentManager.RemoveComponentFromEntity<GlMeshRemoved>(entityId);
        }
    }

    private void DisposeGLResources(ref GlMeshDataComponent glMeshData)
    {
        GL.DeleteVertexArray(glMeshData.Vao);
        GL.DeleteBuffer(glMeshData.Vbo);
        if (glMeshData.Ebo != 0) GL.DeleteBuffer(glMeshData.Ebo);
    }
 
}