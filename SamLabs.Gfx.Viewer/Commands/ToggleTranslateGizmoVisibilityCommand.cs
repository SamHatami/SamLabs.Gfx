using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class ToggleTranslateGizmoVisibilityCommand : Command
{
    private readonly CommandManager _commandManager;
    private readonly ComponentManager _componentManager;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _translateGizmoId = -1;
    private int _rotateGizmoId = -1;
    private int _scaleGizmoId = -1;
    public ToggleTranslateGizmoVisibilityCommand(CommandManager commandManager, ComponentManager componentManager)
    {
        _commandManager = commandManager;
        _componentManager = componentManager;
    }

    public override void Execute()
    {
        //get the TransformGizmo entity
        //if it has VisibilityComponent, remove it
        //else add it
        //should probably save all the gizmo entities and just toggle visibility of them
        GetGizmoIds();
        
        if (_componentManager.HasComponent<ActiveGizmoComponent>(_translateGizmoId))
            _componentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(_translateGizmoId);
        else
            _componentManager.SetComponentToEntity(new ActiveGizmoComponent(), _translateGizmoId);
    }

    private void GetGizmoIds()
    { 
        if (_translateGizmoId != -1 && _scaleGizmoId != -1 && _rotateGizmoId !=-1) return;
        var gizmos = _componentManager.GetEntityIdsForComponentType<GizmoComponent>();
        
        foreach (var gizmoEntity in gizmos)
        {
            ref var gizmo = ref _componentManager.GetComponent<GizmoComponent>(gizmoEntity);
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

            break;
        }
    }
    public override void Undo() => _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _translateGizmoId));
}