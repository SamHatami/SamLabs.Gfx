using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;

namespace SamLabs.Gfx.Engine.Commands.Camera;

public class ToggleViewPresetCommand : InternalCommand
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly ViewPreset _targetPreset;
    private ViewPreset _previousPreset;

    public ToggleViewPresetCommand(IComponentRegistry componentRegistry, ViewPreset targetPreset)
    {
        _componentRegistry = componentRegistry;
        _targetPreset = targetPreset;
    }

    public override void Execute()
    {
        var cameraEntities = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        var cameraEntityId = cameraEntities[0];
        ref var viewPresetComponent = ref _componentRegistry.GetComponent<CameraViewPresetComponent>(cameraEntityId);

        _previousPreset = viewPresetComponent.Preset;
        viewPresetComponent.Preset = _targetPreset;
    }

    public override void Undo()
    {
        var cameraEntities = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        var cameraEntityId = cameraEntities[0];
        ref var viewPresetComponent = ref _componentRegistry.GetComponent<CameraViewPresetComponent>(cameraEntityId);

        viewPresetComponent.Preset = _previousPreset;
    }
}