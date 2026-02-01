﻿using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Manipulators;

public class ManipulatorSelectionSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;

    public ManipulatorSelectionSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;

        // Subscribe to selection cleared events so manipulator-specific selection can be cleared
        editorEvents.SelectionCleared += (s, e) => ClearPreviousSelection();
    }

    public override int SystemPosition => SystemOrders.ManipulatorSelectionUpdate;
    private int _pickingEntity = -1;

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        ref var pickingData = ref ComponentRegistry.GetComponent<PickingDataComponent>(_pickingEntity);
        if (pickingData.ManipualtorSelected() && frameInput.IsDragging) return;

        if (frameInput.IsMouseLeftButtonDown) //TODO: ctrl-click to do add to selection
        {
            if (pickingData.NothingHovered()) return;

            if (frameInput.Cancellation)
            {
                // Clear manipulator selection on cancel
                ClearPreviousSelection();
                return;
            }

            if (pickingData.HoveredEntityId < 0) //Clear if clicked outside any selectable, add esc key to clear
                ClearPreviousSelection();

            //Are we hovering over a manpulator ?
            if (ComponentRegistry.HasComponent<ManipulatorComponent>(pickingData.HoveredEntityId) ||
                ComponentRegistry.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
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

        var entities = _entityRegistry.Query.With<PickingDataComponent>().Get();
        if (entities.IsEmpty()) return; //hmm
        _pickingEntity = entities[0];
    }

    private void SetNewManipulatorSelection(int manpulatorEntityId)
    {
        var activeManipulator = _entityRegistry.Query.With<ActiveManipulatorComponent>().Get();
        if (activeManipulator.IsEmpty()) return;

        ClearPreviousSelection();

        ComponentRegistry.SetComponentToEntity(new SelectedManipulatorChildComponent(), manpulatorEntityId);
    }

    private int[] GetManipulatorsFromSelection(int[] entityIds)
    {
        if (entityIds.IsEmpty()) return entityIds;

        return entityIds
            .Where(id => ComponentRegistry.HasComponent<ManipulatorComponent>(id))
            .Where(id => ComponentRegistry.HasComponent<ManipulatorChildComponent>(id))
            .ToArray();
    }


    private void ClearPreviousSelection()
    {
        var previousSelection = _entityRegistry.Query.With<SelectedManipulatorChildComponent>().Get();
        if (previousSelection.IsEmpty()) return;

        foreach (var entity in previousSelection)
            ComponentRegistry.RemoveComponentFromEntity<SelectedManipulatorChildComponent>(entity);
    }
}