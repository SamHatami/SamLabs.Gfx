using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Selection;

public class SelectionSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly EntityQueryService _query;

    public override int SystemPosition => SystemOrders.SelectionUpdate;
    private PickingDataComponent _pickingData;
    private int _pickingEntity = -1;
    private int[] _currentSelection = System.Array.Empty<int>();
    private bool _isManipulatorDragging;
    
    public SelectionSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry, EntityQueryService query) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
        _query = query;
    }

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        _pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(_pickingEntity);
        
        var selectedManipulators = _query.With<SelectedManipulatorChildComponent>();
        _isManipulatorDragging = !selectedManipulators.IsEmpty && frameInput.IsDragging;

        if(_isManipulatorDragging) return; 
        
        var validEntities = FilterSelection(new[] {_pickingData.HoveredEntityId});

        if (frameInput.LeftClickOccured) //TODO: ctrl-click to do add to selection
        {
            // The click-out cancellation used to clear selection when the user clicked on empty space.
            // We keep the code here as a commented option so it can be re-enabled by the user in future.
            /*
            if (_pickingData.NothingHovered()) //Clear if clicked outside any selectable, add esc key to clear
            {
                ClearSelection(validEntities);
                DisableActiveManipulators();
                EditorEvents.PublishSelectionCleared(new Core.SelectionClearedArgs(validEntities));
                return;
            }
            */
            if (ComponentRegistry.HasComponent<ManipulatorChildComponent>(_pickingData.HoveredEntityId))
                return;

            SetNewSelection(validEntities);
        }

        if (frameInput.Cancellation)
        {
            ClearSelection(validEntities);
            DisableActiveManipulators();
            EditorEvents.PublishSelectionCleared(new Core.SelectionClearedArgs(validEntities));
        }

        //attach manipulators to selected entities if there is an active manipulator
        AttachToManipulator(_pickingData.SelectedEntityIds);

        //TODO: Add box selection
    }

    private void SetNewManipulatorSelection(int manipulatorEntityId)
    {
        var activeManipulator = _query.With<ActiveManipulatorComponent>();
        if (activeManipulator.IsEmpty) return;

        var previousSelection = _query.With<SelectedManipulatorChildComponent>();
        if (!previousSelection.IsEmpty)
            ComponentRegistry.RemoveComponentFromEntities<SelectedManipulatorChildComponent>(previousSelection);

        ComponentRegistry.SetComponentToEntity(new SelectedManipulatorChildComponent(), manipulatorEntityId);
    }

    private int[] FilterSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;

        return entityIds
            .Where(id => id >= 0)
            .Where(id => !ComponentRegistry.HasComponent<ManipulatorComponent>(id))
            .Where(id => !ComponentRegistry.HasComponent<ManipulatorChildComponent>(id))
            .ToArray();
    }

    private void AttachToManipulator(int[] entityIds)
    {
        var activeManipulator = _query.With<ActiveManipulatorComponent>();

        // If no active manipulator, clear all attachments
        if (activeManipulator.IsEmpty)
        {
            var attachedEntities = _query.With<ManipulatorAttachedComponent>();
            if (!attachedEntities.IsEmpty)
            {
                foreach (var entityId in attachedEntities)
                    ComponentRegistry.RemoveComponentFromEntity<ManipulatorAttachedComponent>(entityId);
            }

            return;
        }

        // Clear existing attachments first to ensure only current selection is attached
        var currentAttachments = _query.With<ManipulatorAttachedComponent>();
        foreach (var entityId in currentAttachments)
        {
            ComponentRegistry.RemoveComponentFromEntity<ManipulatorAttachedComponent>(entityId);
        }

        // Attach to currently selected entities
        foreach (var entityId in entityIds)
        {
            ComponentRegistry.SetComponentToEntity(new ManipulatorAttachedComponent(), entityId);
        }
    }

    private void GetPickingEntity()
    {
        if (_pickingEntity != -1) return;

        var pickingEntityId = _query.With<PickingDataComponent>().First();
        if (pickingEntityId == -1) return; //hmm
        _pickingEntity = pickingEntityId;
    }

    private void SetNewSelection(int[] entityIds)
    {
        if (entityIds.Length == 0)
            return;

        ClearSelectionComponent();

        _pickingData.SelectedEntityIds = entityIds.ToArray();
        foreach (var id in entityIds)
            ComponentRegistry.SetComponentToEntity(new SelectedComponent(), id);

        ComponentRegistry.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void ClearSelection(int[] entityIds)
    {
        ClearSelectionComponent();
        DetachManipulatorsFromEntities(entityIds);

        _pickingData.SelectedEntityIds = Array.Empty<int>();
        ComponentRegistry.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void DetachManipulatorsFromEntities(int[] entityIds)
    {
        foreach (var id in entityIds)
            ComponentRegistry.RemoveComponentFromEntity<ManipulatorAttachedComponent>(id);
    }

    private void ClearSelectionComponent()
    {
        var earlierSelection = _query.With<SelectedComponent>();
        if (earlierSelection.IsEmpty) return;

        foreach (var entity in earlierSelection)
            ComponentRegistry.RemoveComponentFromEntity<SelectedComponent>(entity);
    }

    private void DisableActiveManipulators()
    {
        var activeManipulators = _query.With<ActiveManipulatorComponent>();
        if (activeManipulators.IsEmpty) return;

        foreach (var manipulatorEntity in activeManipulators)
        {
            ComponentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(manipulatorEntity);
        }
    }
}