using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

//Invoked by user but not recorded, hence Internal
public class ToggleGizmoCommand : InternalCommand 
{
    private readonly CommandManager _commandManager;
    private readonly GizmoType _gizmoType;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _translateGizmoId = -1;
    private int _rotateGizmoId = -1;
    private int _scaleGizmoId = -1;
    private int _targetGizmoId;

    public ToggleGizmoCommand(CommandManager commandManager, GizmoType gizmoType)
    {
        _commandManager = commandManager;
        _gizmoType = gizmoType;
    }

    public override void Execute()
    {
        GetGizmoIds(); //execution order here instead of constructor, entities might not have been created before the commands have.
        _targetGizmoId = _gizmoType switch
        {
            GizmoType.Translate => _translateGizmoId,
            GizmoType.Rotate => _rotateGizmoId,
            GizmoType.Scale => _scaleGizmoId,
            _ => -1
        };

        if (_targetGizmoId == -1) return;

        HideOtherGizmos();

        if (ComponentManager.HasComponent<ActiveGizmoComponent>(_targetGizmoId))
            ComponentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(_targetGizmoId);
        else
            ComponentManager.SetComponentToEntity(new ActiveGizmoComponent(), _targetGizmoId);
    }

    private void HideOtherGizmos()
    {
        var gizmoIds = new[] { _translateGizmoId, _rotateGizmoId, _scaleGizmoId };
        foreach (var id in gizmoIds.Where(id => id != _targetGizmoId))
        {
            if (ComponentManager.HasComponent<ActiveGizmoComponent>(id))
                ComponentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(id);
        }
    }

    private void GetGizmoIds()
    {
        if (_translateGizmoId != -1 && _scaleGizmoId != -1 && _rotateGizmoId != -1) return;
        var gizmos = ComponentManager.GetEntityIdsForComponentType<GizmoComponent>();

        foreach (var gizmoEntity in gizmos)
        {
            ref var gizmo = ref ComponentManager.GetComponent<GizmoComponent>(gizmoEntity);
            switch (gizmo.Type)
            {
                case GizmoType.Translate:
                    _translateGizmoId = gizmoEntity;
                    break;
                case GizmoType.Rotate:
                    _rotateGizmoId = gizmoEntity;
                    break;
                case GizmoType.Scale:
                    _scaleGizmoId = gizmoEntity;
                    break;
            }
        }
    }

    public override void Undo() =>
        _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _translateGizmoId));
}