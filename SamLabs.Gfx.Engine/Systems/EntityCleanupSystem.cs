using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems;

/// <summary>
/// Removes entities marked with PendingRemovalFlag from the registry.
/// Runs last in the update cycle to ensure all systems have processed their queries.
/// </summary>
public class EntityCleanupSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition => SystemOrders.CleanUp;

    public EntityCleanupSystem(
        EntityRegistry entityRegistry,
        CommandManager commandManager,
        EditorEvents editorEvents,
        IComponentRegistry componentRegistry)
        : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        var entitiesToRemove = _entityRegistry.Query.With<PendingRemovalFlag>().Get();
        
        foreach (var entity in entitiesToRemove)
        {
            ComponentRegistry.RemoveAllComponentsFromEntity(entity);
            _entityRegistry.Remove(entity);
        }
    }
}
