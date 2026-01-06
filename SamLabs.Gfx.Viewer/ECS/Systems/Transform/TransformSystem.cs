using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform;

public class TransformSystem : UpdateSystem
{
    private bool _isTransforming;
    private int _selectedGizmoSubEntity;
    private Dictionary<GizmoType, ITransformStrategy> _transformStrategies;
    private TransformComponent _preChangeTransform;
    private TransformComponent _postChangeTransform;

    public override int SystemPosition => SystemOrders.TransformUpdate;

    public TransformSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager, editorEvents)
    {
        _transformStrategies = new Dictionary<GizmoType, ITransformStrategy>
        {
            [GizmoType.Translate] = new TranslateStrategy(),
            [GizmoType.Rotate] = new RotateStrategy(),
            [GizmoType.Scale] = new ScaleStrategy()
        };
        
        _preChangeTransform = new TransformComponent();
        _postChangeTransform = new TransformComponent();
    }

    public override void Update(FrameInput frameInput)
    {
        var activeGizmo = GetEntityIds.With<ActiveGizmoComponent>().First();
        if (activeGizmo == -1) return;

        var selectedEntities = GetEntityIds.With<SelectedComponent>().AndWith<TransformComponent>()
            .Without<GizmoComponent>().Without<GizmoChildComponent>();

        if (selectedEntities.IsEmpty) return;

        //TODO: Currently only supporting single object selection
        //TODO: Option to use global transform or local transform
        ref var entityTransform = ref ComponentManager.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var gizmoTransform = ref ComponentManager.GetComponent<TransformComponent>(activeGizmo);
        var gizmoComponent = ComponentManager.GetComponent<GizmoComponent>(activeGizmo);

        var transformStrategy = _transformStrategies[gizmoComponent.Type];

        gizmoTransform.Position = entityTransform.Position;
        var pickingEntities = GetEntityIds.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;

        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (ComponentManager.HasComponent<GizmoChildComponent>(pickingData.HoveredEntityId))
            {
                _preChangeTransform = entityTransform;
                _isTransforming = true;
                _selectedGizmoSubEntity = pickingData.HoveredEntityId;
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            ref var gizmoChild = ref ComponentManager.GetComponent<GizmoChildComponent>(_selectedGizmoSubEntity);
            transformStrategy.Apply(frameInput, ref entityTransform, ref gizmoTransform, gizmoChild, true);

            if (entityTransform.IsDirty) //This is the parent
            {
                entityTransform.WorldMatrix = entityTransform.LocalMatrix;
                entityTransform.IsDirty  = false;
            }
        }

        if (frameInput.IsMouseLeftButtonDown || !_isTransforming) return;
        
        _postChangeTransform = ComponentManager.GetComponent<TransformComponent>(selectedEntities[0]);
        CommandManager.AddUndoCommand(new TransformCommand(selectedEntities[0], _preChangeTransform, _postChangeTransform));
        _isTransforming = false;
        _selectedGizmoSubEntity = -1;
        transformStrategy.Reset();
    }
    
}