using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Tools.Transforms.Strategies;

namespace SamLabs.Gfx.Engine.Tools.Transforms;

public class RotateTool : TransformTool
{
    private readonly RotateToolStrategy _strategy;
    private float _currentAngle = 0f;
    private float _startAngle = 0f;
    private float _deltaAngle = 0f;
    
    public override string ToolId => ToolIds.TransformRotate;
    public override string DisplayName => "Rotate";
    
    public double CurrentAngle => _deltaAngle;

    public RotateTool(
        IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityQueryService query,
        EditorEvents editorEvents)
        : base(ManipulatorType.Rotate, componentRegistry, commandManager, query, editorEvents)
    {
        _strategy = new RotateToolStrategy(componentRegistry, query);
    }

    public override void ProcessInput(FrameInput input)
    {
        if (_state == ToolState.Inactive) return;

        var selectedEntities = Query.AndWith<TransformComponent>(Query.With<SelectedComponent>());
        if (selectedEntities.IsEmpty) return;

        var activeManipulator = Query.With<ActiveManipulatorComponent>().First();
        if (activeManipulator == -1) return;

        ref var entityTransform = ref ComponentRegistry.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var manipulatorTransform = ref ComponentRegistry.GetComponent<TransformComponent>(activeManipulator);
        
        manipulatorTransform.Position = entityTransform.Position;
        
        var pickingEntities = Query.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;
        
        ref var pickingData = ref ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (input.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (ComponentRegistry.HasComponent<ManipulatorChildComponent>(pickingData.HoveredEntityId))
            {
                _preChangeTransform = entityTransform;
                _startAngle = 0f;
                _currentAngle = 0f;
                _deltaAngle = 0f;
                _isTransforming = true;
                _selectedManipulatorSubEntity = pickingData.HoveredEntityId;
                SetState(ToolState.InputCapture);
            }
        }

        if (_isTransforming && input.IsMouseLeftButtonDown)
        {
            ref var manipulatorChild =
                ref ComponentRegistry.GetComponent<ManipulatorChildComponent>(_selectedManipulatorSubEntity);
            
            _strategy.Apply(input, ref entityTransform, ref manipulatorTransform, manipulatorChild, true);

            if (entityTransform.IsDirty)
            {
                entityTransform.WorldMatrix = entityTransform.LocalMatrix;
                entityTransform.IsDirty = false;
            }
        }

        if (!input.IsMouseLeftButtonDown && _isTransforming)
        {
            var postChangeTransform = ComponentRegistry.GetComponent<TransformComponent>(selectedEntities[0]);
            CommandManager.AddUndoCommand(new TransformCommand(selectedEntities[0], _preChangeTransform,
                postChangeTransform, ComponentRegistry));
            
            _isTransforming = false;
            _selectedManipulatorSubEntity = -1;
            _strategy.Reset();
            SetState(ToolState.Active);
            
            _deltaAngle = 0f;
        }
    }

    public override IToolUIDescriptor GetUIDescriptor()
    {
        return new TransformToolUIDescriptor
        {
            UIType = ToolUIType.StaticPanel,
            ViewModelTypeName = "TransformToolViewModel",
            CurrentData = new Dictionary<string, object>
            {
                ["Mode"] = "Rotate",
                ["Angle"] = _deltaAngle
            }
        };
    }
}

