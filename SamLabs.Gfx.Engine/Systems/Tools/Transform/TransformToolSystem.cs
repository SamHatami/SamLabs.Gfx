using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Engine.Systems.Tools.Transform.Strategies;

namespace SamLabs.Gfx.Engine.Systems.Tools.Transform;

public class TransformToolSystem : UpdateSystem
{
    private bool _isTransforming;
    private int _selectedManipulatorSubEntity;
    private Dictionary<ManipulatorType, ITransformToolStrategy> _transformStrategies;
    private TransformComponent _preChangeTransform;
    private TransformComponent _postChangeTransform;
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityQueryService _query;

    public override int SystemPosition => SystemOrders.TransformUpdate;

    public TransformToolSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry, EntityQueryService query) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _componentRegistry = componentRegistry;
        _query = query;

        _transformStrategies = new Dictionary<ManipulatorType, ITransformToolStrategy>
        {
            [ManipulatorType.Translate] = new TranslateToolStrategy(_componentRegistry, _query),
            [ManipulatorType.Rotate] = new RotateToolStrategy(_componentRegistry, _query),
            [ManipulatorType.Scale] = new ScaleToolStrategy(_componentRegistry, _query)
        };

        _preChangeTransform = new TransformComponent();
        _postChangeTransform = new TransformComponent();
    }

    public override void Update(FrameInput frameInput)
    {
        var activeManipulator = _query.With<ActiveManipulatorComponent>().First();
        if (activeManipulator == -1) return;

        var selectedEntities = _query.AndWith<TransformComponent>(_query.With<SelectedComponent>());

        if (selectedEntities.IsEmpty) return;

        //TODO: Currently only supporting single object selection
        //TODO: Option to use global transform or local transform
        ref var entityTransform = ref _componentRegistry.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var manipulatorTransform = ref _componentRegistry.GetComponent<TransformComponent>(activeManipulator);
        var manipulatorComponent = _componentRegistry.GetComponent<ManipulatorComponent>(activeManipulator);

        var transformStrategy = _transformStrategies[manipulatorComponent.Type];

        manipulatorTransform.Position = entityTransform.Position;
        var pickingEntities = _query.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;

        ref var pickingData = ref _componentRegistry.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (_componentRegistry.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
            {
                _preChangeTransform = entityTransform;
                _isTransforming = true;
                _selectedManipulatorSubEntity = pickingData.HoveredEntityId;
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            ref var manipulatorChild =
                ref _componentRegistry.GetComponent<ManipulatorChildComponent>(_selectedManipulatorSubEntity);
            transformStrategy.Apply(frameInput, ref entityTransform, ref manipulatorTransform, manipulatorChild, true);

            if (entityTransform.IsDirty) //This is the parent
            {
                entityTransform.WorldMatrix = entityTransform.LocalMatrix;
                entityTransform.IsDirty = false;
            }
        }

        if (frameInput.IsMouseLeftButtonDown || !_isTransforming) return;

        _postChangeTransform = _componentRegistry.GetComponent<TransformComponent>(selectedEntities[0]);
        CommandManager.AddUndoCommand(new TransformCommand(selectedEntities[0], _preChangeTransform,
            _postChangeTransform,_componentRegistry));
        _isTransforming = false;
        _selectedManipulatorSubEntity = -1;
        transformStrategy.Reset();
    }
}