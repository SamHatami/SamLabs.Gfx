using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Manipulators;

public class ManipulatorSelectionSystem : UpdateSystem
{

    public ManipulatorSelectionSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents) : base(entityRegistry, commandManager,editorEvents)
    {
    }

    public override int SystemPosition => SystemOrders.ManipulatorSelectionUpdate;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        ref var pickingData = ref ComponentRegistry.GetComponent<PickingDataComponent>(_pickingEntity);
        if(pickingData.ManipualtorSelected() && frameInput.IsDragging) return;
        
        if (frameInput.IsMouseLeftButtonDown) //TODO: ctrl-click to do add to selection
        {
            if(pickingData.NothingHovered()) return;
            
            if (pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable, add esc key to clear
                ClearPreviousSelection();
            
            //Are we hovering over a manpulator ?
            if(ComponentRegistry.HasComponent<ManipulatorComponent>(pickingData.HoveredEntityId) || ComponentRegistry.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
            {
                pickingData.SelectedManipulatorId = pickingData.HoveredEntityId;
                ComponentRegistry.SetComponentToEntity(pickingData, _pickingEntity);
                SetNewManipulatorSelection(pickingData.HoveredEntityId);
                return;
            }
        }
        
        pickingData.SelectedManipulatorId = -1;
        ComponentRegistry.SetComponentToEntity(pickingData, _pickingEntity);
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
        
        ComponentRegistry.SetComponentToEntity(new SelectedManipulatorChildComponent(), manpulatorEntityId);
    }

    private int[] GetManipulatorsFromSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;
        
        return entityIds
            .Where(id => ComponentRegistry.HasComponent<ManipulatorComponent>(id))
            .Where(id => ComponentRegistry.HasComponent<ManipulatorChildComponent>(id))
            .ToArray();
    }



    private void ClearPreviousSelection()
    {
        var previousSelection = GetEntityIds.With<SelectedManipulatorChildComponent>();
        if (previousSelection.IsEmpty) return;

        foreach (var entity in previousSelection)
            ComponentRegistry.RemoveComponentFromEntity<SelectedManipulatorChildComponent>(entity);
    }

}