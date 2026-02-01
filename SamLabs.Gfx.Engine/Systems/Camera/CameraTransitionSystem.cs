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
    private readonly EntityRegistry _entityRegistry;

    public CameraTransitionSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        var flagIsActive = _entityRegistry.Query.With<TransitionCameraFlag>().Get();
        if (flagIsActive.Length == 0) return;

        var transitionData = _entityRegistry.Query.With<CameraTransitionDataComponent>().Get();
        var cameraData = _entityRegistry.Query.With<CameraDataComponent>().Get();
    }
}