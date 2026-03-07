using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

/// <summary>
/// Transitional Phase 2 upload coordinator.
/// Converts backend-agnostic dirty state into the legacy CreateGlMeshDataFlag path.
/// </summary>
public class MeshUploadSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.Init - 1;

    public MeshUploadSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
        : base(entityRegistry, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var createFlagEntities = ComponentRegistry.GetEntityIdsForComponentType<CreateGlMeshDataFlag>();
        foreach (var entityId in createFlagEntities)
        {
            if (ComponentRegistry.HasComponent<GpuMeshHandleComponent>(entityId))
                continue;

            ComponentRegistry.SetComponentToEntity(new GpuMeshHandleComponent { IsDirty = true }, entityId);
        }

        var entities = ComponentRegistry.GetEntityIdsForComponentType<GpuMeshHandleComponent>();
        if (entities.IsEmpty)
            return;

        foreach (var entityId in entities)
        {
            ref var gpuHandle = ref ComponentRegistry.GetComponent<GpuMeshHandleComponent>(entityId);
            if (!gpuHandle.IsDirty)
                continue;

            if (!ComponentRegistry.HasComponent<CreateGlMeshDataFlag>(entityId)
                && !ComponentRegistry.HasComponent<GlMeshDataComponent>(entityId))
            {
                ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entityId);
            }

            if (ComponentRegistry.HasComponent<GlMeshDataComponent>(entityId))
            {
                gpuHandle.IsDirty = false;
                ComponentRegistry.SetComponentToEntity(gpuHandle, entityId);
            }
        }
    }
}
