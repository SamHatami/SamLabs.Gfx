using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

public class SetProjectionTypeSystem : UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate;
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityQueryService _query;

    public SetProjectionTypeSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents, IComponentRegistry componentRegistry, EntityQueryService query) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
        _componentRegistry = componentRegistry;
        _query = query;
    }

    public override void Update(FrameInput frameInput)
    {
        var cameraEntityId = _query.First(_query.With<CameraComponent>());

        if (cameraEntityId == -1) return;

        var cameraData = _componentRegistry.GetComponent<CameraDataComponent>(cameraEntityId);

        //set projection type etc
    }
}