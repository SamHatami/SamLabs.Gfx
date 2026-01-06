using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Selection;

public class SelectionSystem : UpdateSystem
{
    private readonly EntityManager _entityManager;

    public override int SystemPosition => SystemOrders.SelectionUpdate;
    private PickingDataComponent _pickingData;
    private int _pickingEntity = -1;
    private int[] _currentSelection;
    private bool _isManipulatorDragging;
    
    public SelectionSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager, editorEvents)
    {
        _entityManager = entityManager;
    }

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        _pickingData = ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        
        var selectedManipulators = GetEntityIds.With<SelectedManipulatorChildComponent>();
        _isManipulatorDragging = !selectedManipulators.IsEmpty && frameInput.IsDragging;

        if(_isManipulatorDragging) return; 
        
        var validEntities = FilterSelection([_pickingData.HoveredEntityId]);

        if (frameInput.LeftClickOccured) //TODO: ctrl-click to do add to selection
        {
            if (_pickingData.NothingHovered()) //Clear if clicked outside any selectable, add esc key to clear
                ClearSelection(validEntities);
            if (ComponentManager.HasComponent<ManipulatorChildComponent>(_pickingData.HoveredEntityId))
                return;

            SetNewSelection(validEntities);
        }

        if (frameInput.Cancellation)
            ClearSelection(validEntities);

        //attach manipulators to selected entities if there is an active manipulator
        AttachToManipulator(_pickingData.SelectedEntityIds);

        //TODO: Add box selection
    }

    private void SetNewManipulatorSelection(int manipulatorEntityId)
    {
        var activeManipulator = GetEntityIds.With<ActiveManipulatorComponent>();
        if (activeManipulator.IsEmpty) return;

        var previousSelection = GetEntityIds.With<SelectedManipulatorChildComponent>();
        if (!previousSelection.IsEmpty)
            ComponentManager.RemoveComponentFromEntities<SelectedManipulatorChildComponent>(previousSelection);

        ComponentManager.SetComponentToEntity(new SelectedManipulatorChildComponent(), manipulatorEntityId);
    }

    private int[] FilterSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;

        return entityIds
            .Where(id => id >= 0)
            .Where(id => !ComponentManager.HasComponent<ManipulatorComponent>(id))
            .Where(id => !ComponentManager.HasComponent<ManipulatorChildComponent>(id))
            .ToArray();
    }

    private void AttachToManipulator(int[] entityIds)
    {
        var activeManipulator = GetEntityIds.With<ActiveManipulatorComponent>();

        // If no active manipulator, clear all attachments
        if (activeManipulator.IsEmpty)
        {
            var attachedEntities = GetEntityIds.With<ManipulatorAttachedComponent>();
            if (!attachedEntities.IsEmpty)
            {
                foreach (var entityId in attachedEntities)
                    ComponentManager.RemoveComponentFromEntity<ManipulatorAttachedComponent>(entityId);
            }

            return;
        }

        // Clear existing attachments first to ensure only current selection is attached
        var currentAttachments = GetEntityIds.With<ManipulatorAttachedComponent>();
        foreach (var entityId in currentAttachments)
        {
            ComponentManager.RemoveComponentFromEntity<ManipulatorAttachedComponent>(entityId);
        }

        // Attach to currently selected entities
        foreach (var entityId in entityIds)
        {
            ComponentManager.SetComponentToEntity(new ManipulatorAttachedComponent(), entityId);
        }
    }

    private void GetPickingEntity()
    {
        if (_pickingEntity != -1) return;

        var entities = GetEntityIds.With<PickingDataComponent>();
        if (entities.IsEmpty) return; //hmm
        _pickingEntity = entities[0];
    }

    private void SetNewSelection(int[] entityIds)
    {
        if (entityIds.Length == 0)
            return;

        ClearSelectionComponent();

        _pickingData.SelectedEntityIds = entityIds.ToArray();
        foreach (var id in entityIds)
            ComponentManager.SetComponentToEntity(new SelectedComponent(), id);

        ComponentManager.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void ClearSelection(int[] entityIds)
    {
        ClearSelectionComponent();
        DetachManipulatorsFromEntities(entityIds);

        _pickingData.SelectedEntityIds = [];
        ComponentManager.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void DetachManipulatorsFromEntities(int[] entityIds)
    {
        foreach (var id in entityIds)
            ComponentManager.RemoveComponentFromEntity<ManipulatorAttachedComponent>(id);
    }

    private void ClearSelectionComponent()
    {
        var earlierSelection = GetEntityIds.With<SelectedComponent>();
        if (earlierSelection.IsEmpty) return;

        foreach (var entity in earlierSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedComponent>(entity);
    }
}