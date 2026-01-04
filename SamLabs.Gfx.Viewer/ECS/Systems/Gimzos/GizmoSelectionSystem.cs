using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Gimzos;

public class GizmoSelectionSystem : UpdateSystem
{
    private readonly EntityManager _entityManager;

    public GizmoSelectionSystem(EntityManager entityManager, CommandManager commandManager) : base(entityManager, commandManager)
    {
        _entityManager = entityManager;
    }

    public override int SystemPosition => SystemOrders.GizmoSelectionUpdate;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        if(pickingData.GizmoSelected() && frameInput.IsDragging) return;
        
        if (frameInput.IsMouseLeftButtonDown) //TODO: ctrl-click to do add to selection
        {
            if(pickingData.NothingHovered()) return;
            
            if (pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable, add esc key to clear
                ClearPreviousSelection();
            
            //Are we hovering over a gizmo ?
            if(ComponentManager.HasComponent<GizmoComponent>(pickingData.HoveredEntityId) || ComponentManager.HasComponent<GizmoChildComponent>(pickingData.HoveredEntityId))
            {
                pickingData.SelectedGizmoId = pickingData.HoveredEntityId;
                ComponentManager.SetComponentToEntity(pickingData, _pickingEntity);
                SetNewGizmoSelection(pickingData.HoveredEntityId);
                return;
            }
        }
        
        pickingData.SelectedGizmoId = -1;
        ComponentManager.SetComponentToEntity(pickingData, _pickingEntity);
        ClearPreviousSelection();
    }
    
    private void GetPickingEntity()
    {
        if (_pickingEntity != -1) return;

        var entities = GetEntityIds.With<PickingDataComponent>();
        if (entities.IsEmpty) return; //hmm
        _pickingEntity = entities[0];
    }

    private void SetNewGizmoSelection(int gizmoEntityId)
    {
        var activeGizmo = GetEntityIds.With<ActiveGizmoComponent>();
        if (activeGizmo.IsEmpty) return;
        
        ClearPreviousSelection();
        
        ComponentManager.SetComponentToEntity(new SelectedChildGizmoComponent(), gizmoEntityId);
    }

    private int[] GetGizmosFromSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;
        
        return entityIds
            .Where(id => ComponentManager.HasComponent<GizmoComponent>(id))
            .Where(id => ComponentManager.HasComponent<GizmoChildComponent>(id))
            .ToArray();
    }



    private void ClearPreviousSelection()
    {
        var previousSelection = GetEntityIds.With<SelectedChildGizmoComponent>();
        if (previousSelection.IsEmpty) return;

        foreach (var entity in previousSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedChildGizmoComponent>(entity);
    }

}