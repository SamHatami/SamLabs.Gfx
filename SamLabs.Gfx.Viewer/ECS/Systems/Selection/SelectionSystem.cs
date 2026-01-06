using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
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
    private bool _isGizmoDragging;
    
    public SelectionSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager, editorEvents)
    {
        _entityManager = entityManager;
    }

    public override void Update(FrameInput frameInput)
    {
        GetPickingEntity();

        if (_pickingEntity == -1) return;
        _pickingData = ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        
        var selectedGizmos = GetEntityIds.With<SelectedChildGizmoComponent>();
        _isGizmoDragging = !selectedGizmos.IsEmpty && frameInput.IsDragging;

        if(_isGizmoDragging) return; 
        
        var validEntities = FilterSelection([_pickingData.HoveredEntityId]);

        if (frameInput.LeftClickOccured) //TODO: ctrl-click to do add to selection
        {
            if (_pickingData.NothingHovered()) //Clear if clicked outside any selectable, add esc key to clear
                ClearSelection(validEntities);
            if (ComponentManager.HasComponent<GizmoChildComponent>(_pickingData.HoveredEntityId))
                return;

            SetNewSelection(validEntities);
        }

        if (frameInput.Cancellation)
            ClearSelection(validEntities);

        //attach gizmos to selected entities if there is an active gizmo
        AttachToGizmo(_pickingData.SelectedEntityIds);

        //TODO: Add box selection
    }

    private void SetNewGizmoSelection(int gizmoEntityId)
    {
        var activeGizmo = GetEntityIds.With<ActiveGizmoComponent>();
        if (activeGizmo.IsEmpty) return;

        var previousSelection = GetEntityIds.With<SelectedChildGizmoComponent>();
        if (!previousSelection.IsEmpty)
            ComponentManager.RemoveComponentFromEntities<SelectedChildGizmoComponent>(previousSelection);

        ComponentManager.SetComponentToEntity(new SelectedChildGizmoComponent(), gizmoEntityId);
    }

    private int[] FilterSelection(int[] entityIds)
    {
        if (entityIds.Length == 0) return entityIds;

        return entityIds
            .Where(id => id >= 0)
            .Where(id => !ComponentManager.HasComponent<GizmoComponent>(id))
            .Where(id => !ComponentManager.HasComponent<GizmoChildComponent>(id))
            .ToArray();
    }

    private void AttachToGizmo(int[] entityIds)
    {
        var activeGizmo = GetEntityIds.With<ActiveGizmoComponent>();

        // If no active gizmo, clear all attachments
        if (activeGizmo.IsEmpty)
        {
            var attachedEntities = GetEntityIds.With<GizmoAttachedComponent>();
            if (!attachedEntities.IsEmpty)
            {
                foreach (var entityId in attachedEntities)
                    ComponentManager.RemoveComponentFromEntity<GizmoAttachedComponent>(entityId);
            }

            return;
        }

        // Clear existing attachments first to ensure only current selection is attached
        var currentAttachments = GetEntityIds.With<GizmoAttachedComponent>();
        foreach (var entityId in currentAttachments)
        {
            ComponentManager.RemoveComponentFromEntity<GizmoAttachedComponent>(entityId);
        }

        // Attach to currently selected entities
        foreach (var entityId in entityIds)
        {
            ComponentManager.SetComponentToEntity(new GizmoAttachedComponent(), entityId);
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
        DetachGizmosFromEntities(entityIds);

        _pickingData.SelectedEntityIds = [];
        ComponentManager.SetComponentToEntity(_pickingData, _pickingEntity);
    }

    private void DetachGizmosFromEntities(int[] entityIds)
    {
        foreach (var id in entityIds)
            ComponentManager.RemoveComponentFromEntity<GizmoAttachedComponent>(id);
    }

    private void ClearSelectionComponent()
    {
        var earlierSelection = GetEntityIds.With<SelectedComponent>();
        if (earlierSelection.IsEmpty) return;

        foreach (var entity in earlierSelection)
            ComponentManager.RemoveComponentFromEntity<SelectedComponent>(entity);
    }
}