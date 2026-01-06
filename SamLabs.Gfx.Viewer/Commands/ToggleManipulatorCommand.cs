using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

//Invoked by user but not recorded, hence Internal
public class ToggleManipulatorCommand : InternalCommand 
{
    private readonly CommandManager _commandManager;
    private readonly ManipulatorType _manipulatorType;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _translateManipulatorId = -1;
    private int _rotateManipulatorId = -1;
    private int _scaleManipulatorId = -1;
    private int _targetManipulatorId;

    public ToggleManipulatorCommand(CommandManager commandManager, ManipulatorType manipulatorType)
    {
        _commandManager = commandManager;
        _manipulatorType = manipulatorType;
    }

    public override void Execute()
    {
        GetManipulatorIds(); //execution order here instead of constructor, entities might not have been created before the commands have.
        _targetManipulatorId = _manipulatorType switch
        {
            ManipulatorType.Translate => _translateManipulatorId,
            ManipulatorType.Rotate => _rotateManipulatorId,
            ManipulatorType.Scale => _scaleManipulatorId,
            _ => -1
        };

        if (_targetManipulatorId == -1) return;

        HideOtherManipulators();

        if (ComponentManager.HasComponent<ActiveManipulatorComponent>(_targetManipulatorId))
            ComponentManager.RemoveComponentFromEntity<ActiveManipulatorComponent>(_targetManipulatorId);
        else
            ComponentManager.SetComponentToEntity(new ActiveManipulatorComponent(), _targetManipulatorId);
    }

    private void HideOtherManipulators()
    {
        var manipulatorIds = new[] { _translateManipulatorId, _rotateManipulatorId, _scaleManipulatorId };
        foreach (var id in manipulatorIds.Where(id => id != _targetManipulatorId))
        {
            if (ComponentManager.HasComponent<ActiveManipulatorComponent>(id))
                ComponentManager.RemoveComponentFromEntity<ActiveManipulatorComponent>(id);
        }
    }

    private void GetManipulatorIds()
    {
        if (_translateManipulatorId != -1 && _scaleManipulatorId != -1 && _rotateManipulatorId != -1) return;
        var manipulators = ComponentManager.GetEntityIdsForComponentType<ManipulatorComponent>();

        foreach (var manipulatorEntity in manipulators)
        {
            ref var manipulator = ref ComponentManager.GetComponent<ManipulatorComponent>(manipulatorEntity);
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