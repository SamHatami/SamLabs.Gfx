using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

//System to transition camera between two positions and targets.
public class CameraTransitionSystem : UpdateSystem
{
    private readonly EntityQueryService _entityQuery;

    public CameraTransitionSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents,
        IComponentRegistry componentRegistry, EntityQueryService entityQuery) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
        _entityQuery = entityQuery;
    }

    public override void Update(FrameInput frameInput)
    {
        var flagIsActive = _entityQuery.With<TransitionCameraFlag>();

        if (flagIsActive.IsEmpty) return;

        var transitionData = _entityQuery.With<CameraTransitionDataComponent>();
        var cameraData = _entityQuery.With<CameraDataComponent>();
    }
}