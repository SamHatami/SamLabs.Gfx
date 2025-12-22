using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GizmoSelectionSystem : UpdateSystem
{
    private readonly EntityManager _entityManager;

    public GizmoSelectionSystem(EntityManager entityManager) : base(entityManager)
    {
        _entityManager = entityManager;
    }

    public override int SystemPosition => SystemOrders.GizmoSelectionUpdate;
    private PickingDataComponent _pickingData;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        _pickingData = ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        
        if (frameInput.LeftClickOccured) //TODO: ctrl-click to do add to selection
        {
            if (_pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable, add esc key to clear
                ClearPreviousSelection();
            
            if(ComponentManager.HasComponent<GizmoComponent>(_pickingData.HoveredEntityId) || ComponentManager.HasComponent<GizmoChildComponent>(_pickingData.HoveredEntityId))
                SetNewGizmoSelection(_pickingData.HoveredEntityId);
        }

        if (frameInput.Cancelation)
            ClearPreviousSelection();
    }
    
    private void GetPickingEntity()
    {
        if (_pickingEntity != -1) return;

        var entities = GetEntitiesIds.With<PickingDataComponent>();
        if (entities.IsEmpty) return; //hmm
        _pickingEntity = entities[0];
    }

    private void SetNewGizmoSelection(int gizmoEntityId)
    {
        var activeGizmo = GetEntitiesIds.With<ActiveGizmoComponent>();
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
        var previousSelection = GetEntitiesIds.With<SelectedChildGizmoComponent>();
        if (previousSelection.IsEmpty) return;

        foreach (var entity in previousSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedChildGizmoComponent>(entity);
    }

}