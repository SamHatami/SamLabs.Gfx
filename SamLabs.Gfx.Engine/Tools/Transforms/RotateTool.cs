using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
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
    private Vector3 _currentEulerAngles = Vector3.Zero;
    private Vector3 _startEulerAngles = Vector3.Zero;
    private Vector3 _deltaAngles = Vector3.Zero; // Delta per axis (degrees)
    private Vector3 _activeAxis = Vector3.UnitY;
    private bool _isRelativeMode = true;
    
    public override string ToolId => ToolIds.TransformRotate;
    public override string DisplayName => "Rotate";
    
    // Absolute euler angles in degrees
    public double CurrentX => _currentEulerAngles.X;
    public double CurrentY => _currentEulerAngles.Y;
    public double CurrentZ => _currentEulerAngles.Z;
    
    // Delta angles in degrees (for relative mode display)
    public double DeltaX => _deltaAngles.X;
    public double DeltaY => _deltaAngles.Y;
    public double DeltaZ => _deltaAngles.Z;

    public bool IsRelativeMode
    {
        get => _isRelativeMode;
        set
        {
            if (_isRelativeMode != value)
            {
                _isRelativeMode = value;
                OnPropertyChanged(nameof(IsRelativeMode));
            }
        }
    }

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
                
                _startEulerAngles = MathExtensions.ExtractEulerAngles(entityTransform.Rotation);
                _currentEulerAngles = _startEulerAngles;
                _deltaAngles = Vector3.Zero;
                
                _isTransforming = true;
                _selectedManipulatorSubEntity = pickingData.HoveredEntityId;
                SetState(ToolState.InputCapture);
            }
        }

        if (_isTransforming && input.IsMouseLeftButtonDown)
        {
            ref var manipulatorChild =
                ref ComponentRegistry.GetComponent<ManipulatorChildComponent>(_selectedManipulatorSubEntity);
            
            _activeAxis = manipulatorChild.Axis.ToVector3();
            _strategy.Apply(input, ref entityTransform, ref manipulatorTransform, manipulatorChild, true);

            if (entityTransform.IsDirty)
            {
                entityTransform.WorldMatrix = entityTransform.LocalMatrix;
                entityTransform.IsDirty = false;
                
                // Update absolute euler angles
                _currentEulerAngles = MathExtensions.ExtractEulerAngles(entityTransform.Rotation);
                
                // Calculate delta only for the active axis
                var fullDelta = _currentEulerAngles - _startEulerAngles;
                if (Math.Abs(_activeAxis.X) > 0.9f)
                    _deltaAngles = new Vector3(fullDelta.X, 0, 0);
                else if (Math.Abs(_activeAxis.Y) > 0.9f)
                    _deltaAngles = new Vector3(0, fullDelta.Y, 0);
                else if (Math.Abs(_activeAxis.Z) > 0.9f)
                    _deltaAngles = new Vector3(0, 0, fullDelta.Z);
                
                OnPropertyChanged(nameof(CurrentX));
                OnPropertyChanged(nameof(CurrentY));
                OnPropertyChanged(nameof(CurrentZ));
                OnPropertyChanged(nameof(DeltaX));
                OnPropertyChanged(nameof(DeltaY));
                OnPropertyChanged(nameof(DeltaZ));
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
            
            _deltaAngles = Vector3.Zero;
            OnPropertyChanged(nameof(DeltaX));
            OnPropertyChanged(nameof(DeltaY));
            OnPropertyChanged(nameof(DeltaZ));
        }
    }

    public override void UpdateValues(double x, double y, double z)
    {
        var selectedEntities = Query.AndWith<TransformComponent>(Query.With<SelectedComponent>());
        if (selectedEntities.IsEmpty) return;

        var entityId = selectedEntities[0];
        ref var entityTransform = ref ComponentRegistry.GetComponent<TransformComponent>(entityId);
        var preChangeTransform = entityTransform;

        // Always work with absolute euler angles for manual input
        var targetEulerAngles = new Vector3((float)x, (float)y, (float)z);

        // Convert euler angles (in degrees) to quaternion
        var newRotation = MathExtensions.QuaternionFromEulerAngles(targetEulerAngles);

        if (entityTransform.Rotation != newRotation)
        {
            entityTransform.Rotation = newRotation;
            entityTransform.IsDirty = true;
            entityTransform.WorldMatrix = entityTransform.LocalMatrix;
            entityTransform.IsDirty = false;

            _currentEulerAngles = targetEulerAngles;
            if (!_isTransforming)
            {
                _startEulerAngles = _currentEulerAngles;
                _deltaAngles = Vector3.Zero;
            }

            CommandManager.AddUndoCommand(new TransformCommand(entityId, preChangeTransform, entityTransform, ComponentRegistry));

            OnPropertyChanged(nameof(CurrentX));
            OnPropertyChanged(nameof(CurrentY));
            OnPropertyChanged(nameof(CurrentZ));
            OnPropertyChanged(nameof(DeltaX));
            OnPropertyChanged(nameof(DeltaY));
            OnPropertyChanged(nameof(DeltaZ));
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
                ["X"] = _currentEulerAngles.X,
                ["Y"] = _currentEulerAngles.Y,
                ["Z"] = _currentEulerAngles.Z,
                ["DeltaX"] = _deltaAngles.X,
                ["DeltaY"] = _deltaAngles.Y,
                ["DeltaZ"] = _deltaAngles.Z,
                ["IsRelative"] = _isRelativeMode
            }
        };
    }
}

