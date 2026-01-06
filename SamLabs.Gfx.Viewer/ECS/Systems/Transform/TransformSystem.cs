using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
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
    private int _selectedManipulatorSubEntity;
    private Dictionary<ManipulatorType, ITransformStrategy> _transformStrategies;
    private TransformComponent _preChangeTransform;
    private TransformComponent _postChangeTransform;

    public override int SystemPosition => SystemOrders.TransformUpdate;

    public TransformSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager, editorEvents)
    {
        _transformStrategies = new Dictionary<ManipulatorType, ITransformStrategy>
        {
            [ManipulatorType.Translate] = new TranslateStrategy(),
            [ManipulatorType.Rotate] = new RotateStrategy(),
            [ManipulatorType.Scale] = new ScaleStrategy()
        };
        
        _preChangeTransform = new TransformComponent();
        _postChangeTransform = new TransformComponent();
    }

    public override void Update(FrameInput frameInput)
    {
        var activeManipulator = GetEntityIds.With<ActiveManipulatorComponent>().First();
        if (activeManipulator == -1) return;

        var selectedEntities = GetEntityIds.With<SelectedComponent>().AndWith<TransformComponent>()
            .Without<ManipulatorComponent>().Without<ManipulatorChildComponent>();

        if (selectedEntities.IsEmpty) return;

        //TODO: Currently only supporting single object selection
        //TODO: Option to use global transform or local transform
        ref var entityTransform = ref ComponentManager.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var manipulatorTransform = ref ComponentManager.GetComponent<TransformComponent>(activeManipulator);
        var manipulatorComponent = ComponentManager.GetComponent<ManipulatorComponent>(activeManipulator);

        var transformStrategy = _transformStrategies[manipulatorComponent.Type];

        manipulatorTransform.Position = entityTransform.Position;
        var pickingEntities = GetEntityIds.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;

        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (ComponentManager.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
            {
                _preChangeTransform = entityTransform;
                _isTransforming = true;
                _selectedManipulatorSubEntity = pickingData.HoveredEntityId;
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            ref var manipulatorChild = ref ComponentManager.GetComponent<ManipulatorChildComponent>(_selectedManipulatorSubEntity);
            transformStrategy.Apply(frameInput, ref entityTransform, ref manipulatorTransform, manipulatorChild, true);

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
        _selectedManipulatorSubEntity = -1;
        transformStrategy.Reset();
    }
    
}