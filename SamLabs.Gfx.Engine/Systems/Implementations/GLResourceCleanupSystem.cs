using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Flags.GL;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations;


[RenderPassAttributes.RenderOrder(SystemOrders.CleanUp)]
public class GLResourceCleanupSystem: RenderSystem
{
    public override int SystemPosition => SystemOrders.CleanUp;
    private readonly IComponentRegistry _componentRegistry;
    public GLResourceCleanupSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var entityIds = _componentRegistry.GetEntityIdsForComponentType<GlMeshRemoved>();
        if (entityIds.Length == 0) return;
        
        
        foreach (var entityId in entityIds)
        {
            ref var glData = ref _componentRegistry.GetComponent<GlMeshDataComponent>(entityId);
            DisposeGLResources(ref glData);
            _componentRegistry.RemoveComponentFromEntity<GlMeshRemoved>(entityId);
        }
    }

    private void DisposeGLResources(ref GlMeshDataComponent glMeshData)
    {
        GL.DeleteVertexArray(glMeshData.Vao);
        GL.DeleteBuffer(glMeshData.Vbo);
        if (glMeshData.Ebo != 0) GL.DeleteBuffer(glMeshData.Ebo);
    }
 
}