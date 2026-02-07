using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
namespace SamLabs.Gfx.Engine.Commands;
public class DeactivateManipulatorCommand : InternalCommand
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _manipulatorEntityId;
    public DeactivateManipulatorCommand(IComponentRegistry componentRegistry, int manipulatorEntityId)
    {
        _componentRegistry = componentRegistry;
        _manipulatorEntityId = manipulatorEntityId;
    }
    public override void Execute()
    {
        if (_manipulatorEntityId == -1) return;
        if (_componentRegistry.HasComponent<ActiveManipulatorComponent>(_manipulatorEntityId))
        {
            _componentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(_manipulatorEntityId);
        }
    }
    public override void Undo()
    {
        // No undo needed for internal commands
    }
}
