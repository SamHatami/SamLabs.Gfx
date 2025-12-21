using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class SelectionSystem:UpdateSystem
{
    public override int SystemPosition => SystemOrders.SelectionUpdate;
    private PickingDataComponent _pickingData;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();
        
        if(_pickingEntity == -1) return; 
        _pickingData = ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);

        //User clicks on a object to select it.
        //User activates an transform gizmo (by either a button in ui, or shortcut)
        //User hovers an transform sub-entity
        //User clicks the sub entity -> should not cause clear selection
        //User pushes mouse button down and drags while the mouse stays on the transform gizmo subentitu -> Should not cause clear selection
        //requires seperation between object selection and gizmo selection
        if (frameInput.LeftClickOccured)
        {
            if (_pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable
                ClearSelection();
            else
            {
                // if (_pickingData.HoveredEntityType == EntityType.Gizmo)
                // {
                //     // If we clicked a gizmo, we don't want to clear or change the scene object selection
                //     // The transform system will handle gizmo interaction.
                // }
                // else
                // {
                    //single selection, remove selectecomponent from all other add it to the hovered one
                    ClearSelectionComponent();
                    SetNewSelection([_pickingData.HoveredEntityId]);
                // }
            }
        }

        if (frameInput.Cancelation)
            ClearSelection();
        
        //TODO: Add box selection

        //if a object hovered and clicked on with left mouse button remove the SelectedDataComponent on every other
        //entity, if it is ctrl-clicked, just add the selectedDatacomponent to that specific entity
    }

    private void GetPickingEntity()
    {
        if (_pickingEntity != -1) return;
        
        var entities = GetEntities.With<PickingDataComponent>();
        if(entities.IsEmpty) return; //hmm
        _pickingEntity = entities[0];
    }

    private void SetNewSelection(List<int> entityId)
    {
        _pickingData.SelectedEntityIds = entityId.ToArray();
        foreach (var id in entityId)
            ComponentManager.SetComponentToEntity(new SelectedComponent(), id);
        
        ComponentManager.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void ClearSelection()
    {
        _pickingData.SelectedEntityIds = [];
        ClearSelectionComponent();
        ComponentManager.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void ClearSelectionComponent()
    {
        var earlierSelection = GetEntities.With<SelectedComponent>();
        if(earlierSelection.IsEmpty) return;
        
        foreach (var entity in earlierSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedComponent>(entity);
    }
}