using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class SelectionSystem:UpdateSystem
{
    private PickingDataComponent _pickingData;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        if(_pickingEntity == -1)
        {
            var entities = GetEntities.With<PickingDataComponent>();
            _pickingEntity = entities[0];
        }
        
        // GetComponentsForEntities.With<PickingDataComponent>();
        _pickingData = ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);

        if (frameInput.LeftClickOccured)
        {
            if (_pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable
                _pickingData.SelectedEntityIds = [];
            else
            {
                //single selection, remove selectecomponent from all other add it to the hovered one
                ClearSelectionComponent();
                SetNewSelection([_pickingData.HoveredEntityId]);
            }
        }

        if (frameInput.Cancelation)
            _pickingData.SelectedEntityIds = [];
        
        //TODO: Add box selection

        //if a object hovered and clicked on with left mouse button remove the SelectedDataComponent on every other
        //entity, if it is ctrl-clicked, just add the selectedDatacomponent to that specific entity
    }

    private void SetNewSelection(List<int> entityId)
    {
        _pickingData.SelectedEntityIds = entityId.ToArray();
        foreach (var id in entityId)
            ComponentManager.SetComponentToEntity(new SelectedComponent(), id);
    }

    private void ClearSelectionComponent()
    {
        var earlierSelection = GetEntities.With<SelectedComponent>();
        if(earlierSelection.IsEmpty) return;
        
        foreach (var entity in earlierSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedComponent>(entity);
    }
}