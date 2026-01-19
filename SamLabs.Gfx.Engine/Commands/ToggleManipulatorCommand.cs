using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Commands;

//Invoked by user but not recorded, hence Internal
public class ToggleManipulatorCommand : InternalCommand 
{
    private readonly CommandManager _commandManager;
    private readonly ManipulatorType _manipulatorType;
    private readonly IComponentRegistry _componentRegistry;
    private readonly Scene _scene;
    private readonly EntityFactory _entityFactory;
    private int _translateManipulatorId = -1;
    private int _rotateManipulatorId = -1;
    private int _scaleManipulatorId = -1;
    private int _targetManipulatorId;

    public ToggleManipulatorCommand(CommandManager commandManager, ManipulatorType manipulatorType, IComponentRegistry componentRegistry)
    {
        _commandManager = commandManager;
        _manipulatorType = manipulatorType;
        _componentRegistry = componentRegistry;
    }

    public override void Execute()
    {
        GetManipulatorIds(); //execution order here instead of constructor, entities might not have been created before the commands have.
        _targetManipulatorId = _manipulatorType switch
        {
            ManipulatorType.Translate => _translateManipulatorId,
            ManipulatorType.Rotate => _rotateManipulatorId,
            ManipulatorType.Scale => _scaleManipulatorId,
            //Drag manipulator is not activated from the UI as a single manipulator, usually activated by another command 
            _ => -1
        };

        if (_targetManipulatorId == -1) return;

        HideOtherManipulators();

        if (_componentRegistry.HasComponent<ActiveManipulatorComponent>(_targetManipulatorId))
            _componentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(_targetManipulatorId);
        else
            _componentRegistry.SetComponentToEntity(new ActiveManipulatorComponent(), _targetManipulatorId);
    }

    private void HideOtherManipulators()
    {
        var manipulatorIds = new[] { _translateManipulatorId, _rotateManipulatorId, _scaleManipulatorId };
        foreach (var id in manipulatorIds.Where(id => id != _targetManipulatorId))
        {
            if (_componentRegistry.HasComponent<ActiveManipulatorComponent>(id))
                _componentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(id);
        }
    }

    private void GetManipulatorIds()
    {
        if (_translateManipulatorId != -1 && _scaleManipulatorId != -1 && _rotateManipulatorId != -1 ) return;
        var manipulators = _componentRegistry.GetEntityIdsForComponentType<ManipulatorComponent>();

        foreach (var manipulatorEntity in manipulators)
        {
            ref var manipulator = ref _componentRegistry.GetComponent<ManipulatorComponent>(manipulatorEntity);
            switch (manipulator.Type)
            {
                case ManipulatorType.Translate:
                    _translateManipulatorId = manipulatorEntity;
                    break;
                case ManipulatorType.Rotate:
                    _rotateManipulatorId = manipulatorEntity;
                    break;
                case ManipulatorType.Scale:
                    _scaleManipulatorId = manipulatorEntity;
                    break;

            }
        }
    }

    public override void Undo() =>
        _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _translateManipulatorId));
}