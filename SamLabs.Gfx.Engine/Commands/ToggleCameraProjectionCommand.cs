using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;

namespace SamLabs.Gfx.Engine.Commands;

public class ToggleCameraProjectionCommand : InternalCommand
{
    private readonly IComponentRegistry _componentRegistry;

    public ToggleCameraProjectionCommand(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override void Execute()
    {
        var cameraEntities = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        var cameraEntityId = cameraEntities[0];
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraEntityId);

        cameraData.ProjectionType = cameraData.ProjectionType == ProjectionType.Perspective
            ? ProjectionType.Orthographic
            : ProjectionType.Perspective;
    }

    public override void Undo()
    {
        // Not needed for non-redoable command
    }
}
