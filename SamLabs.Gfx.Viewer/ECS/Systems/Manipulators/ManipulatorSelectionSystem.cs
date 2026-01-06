using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Components.Selection;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Manipulators;

public class ManipulatorSelectionSystem : UpdateSystem
{

    public ManipulatorSelectionSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager,editorEvents)
    {
    }

    public override int SystemPosition => SystemOrders.ManipulatorSelectionUpdate;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        if(pickingData.ManipualtorSelected() && frameInput.IsDragging) return;
        
        if (frameInput.IsMouseLeftButtonDown) //TODO: ctrl-click to do add to selection
        {
            if(pickingData.NothingHovered()) return;
            
            if (pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable, add esc key to clear
                ClearPreviousSelection();
            
            //Are we hovering over a manpulator ?
            if(ComponentManager.HasComponent<ManipulatorComponent>(pickingData.HoveredEntityId) || ComponentManager.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
            {
                pickingData.SelectedManipulatorId = pickingData.HoveredEntityId;
                ComponentManager.SetComponentToEntity(pickingData, _pickingEntity);
                SetNewManipulatorSelection(pickingData.HoveredEntityId);
                return;
            }
        }
        
        pickingData.SelectedManipulatorId = -1;
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

    private void SetNewManipulatorSelection(int manpulatorEntityId)
    {
        var activeManipulator = GetEntityIds.With<ActiveManipulatorComponent>();
        if (activeManipulator.IsEmpty) return;
        
        ClearPreviousSelection();
        
        ComponentManager.SetComponentToEntity(new SelectedManipulatorChildComponent(), manpulatorEntityId);
    }

    private int[] GetManipulatorsFromSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;
        
        return entityIds
            .Where(id => ComponentManager.HasComponent<ManipulatorComponent>(id))
            .Where(id => ComponentManager.HasComponent<ManipulatorChildComponent>(id))
            .ToArray();
    }



    private void ClearPreviousSelection()
    {
        var previousSelection = GetEntityIds.With<SelectedManipulatorChildComponent>();
        if (previousSelection.IsEmpty) return;

        foreach (var entity in previousSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedManipulatorChildComponent>(entity);
    }

}