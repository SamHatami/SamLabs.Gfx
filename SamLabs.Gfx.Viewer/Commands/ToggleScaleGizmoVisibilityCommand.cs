using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class ToggleScaleGizmoVisibilityCommand : Command
{
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _translateGizmoId = -1;
    private int _rotateGizmoId = -1;
    private int _scaleGizmoId = -1;
    public ToggleScaleGizmoVisibilityCommand(CommandManager commandManager)
    {
        _commandManager = commandManager;
    }

    public override void Execute()
    {
        //get the TransformGizmo entity
        //if it has VisibilityComponent, remove it
        //else add it
        //should probably save all the gizmo entities and just toggle visibility of them
        //but for now i'll just clone the commands...
        GetGizmoIds();
        HideOtherGizmos();
        
        if (ComponentManager.HasComponent<ActiveGizmoComponent>(_scaleGizmoId))
            ComponentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(_scaleGizmoId);
        else
            ComponentManager.SetComponentToEntity(new ActiveGizmoComponent(), _scaleGizmoId);
    }

    private void HideOtherGizmos()
    {
        if(ComponentManager.HasComponent<ActiveGizmoComponent>(_translateGizmoId))
            ComponentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(_translateGizmoId);
        if(ComponentManager.HasComponent<ActiveGizmoComponent>(_rotateGizmoId))
            ComponentManager.RemoveComponentFromEntity<ActiveGizmoComponent>(_rotateGizmoId);
    }

    private void GetGizmoIds()
    { 
        if (_translateGizmoId != -1 && _scaleGizmoId != -1 && _rotateGizmoId !=-1) return;
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
    public override void Undo() => _commandManager.EnqueueCommand(new RemoveRenderableCommand(_scene, _translateGizmoId));
}