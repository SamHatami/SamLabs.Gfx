using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
namespace SamLabs.Gfx.Engine.Commands;
public class ActivateManipulatorCommand : InternalCommand
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _manipulatorEntityId;
    private readonly bool _hideOthers;
    public ActivateManipulatorCommand(IComponentRegistry componentRegistry, int manipulatorEntityId, bool hideOthers = true)
    {
        _componentRegistry = componentRegistry;
        _manipulatorEntityId = manipulatorEntityId;
        _hideOthers = hideOthers;
    }
    public override void Execute()
    {
        if (_manipulatorEntityId == -1) return;
        if (_hideOthers)
        {
            HideAllManipulators();
        }
        if (!_componentRegistry.HasComponent<ActiveManipulatorComponent>(_manipulatorEntityId))
        {
            _componentRegistry.SetComponentToEntity(new ActiveManipulatorComponent(), _manipulatorEntityId);
        }
    }
    private void HideAllManipulators()
    {
        var activeManipulators = _componentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        foreach (var manipId in activeManipulators)
        {
            if (manipId != _manipulatorEntityId)
            {
                _componentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(manipId);
            }
        }
    }
    public override void Undo()
    {
        // No undo needed for internal commands
    }
}
