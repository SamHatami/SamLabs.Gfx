using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.PreRenderUpdate)]
public class GlUpdateVertexPositionSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate;

    public GlUpdateVertexPositionSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var entityIds = ComponentRegistry.GetEntityIdsForComponentType<GlMeshDataChangedComponent>();
        if (entityIds.IsEmpty) return;

        foreach (var entityId in entityIds)
        {
            if (ComponentRegistry.HasComponent<GpuMeshHandleComponent>(entityId))
            {
                ref var gpuHandle = ref ComponentRegistry.GetComponent<GpuMeshHandleComponent>(entityId);
                gpuHandle.IsDirty = true;
            }

            ComponentRegistry.RemoveComponentFromEntity<GlMeshDataChangedComponent>(entityId);
        }
    }
}
