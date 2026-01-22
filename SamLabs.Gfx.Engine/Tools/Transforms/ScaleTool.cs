﻿﻿using OpenTK.Mathematics;
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

public class ScaleTool : TransformTool
{
    private readonly ScaleToolStrategy _strategy;
    private Vector3 _currentScale = Vector3.One;
    private Vector3 _startScale = Vector3.One;
    private Vector3 _deltaScale = Vector3.Zero;
    private bool _isRelativeMode = true;
    
    public override string ToolId => ToolIds.TransformScale;
    public override string DisplayName => "Scale";
    
    // Absolute scale
    public double CurrentX => _currentScale.X;
    public double CurrentY => _currentScale.Y;
    public double CurrentZ => _currentScale.Z;
    
    // Delta scale
    public double DeltaX => _deltaScale.X;
    public double DeltaY => _deltaScale.Y;
    public double DeltaZ => _deltaScale.Z;

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

    public ScaleTool(
        IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityQueryService query,
        EditorEvents editorEvents)
        : base(ManipulatorType.Scale, componentRegistry, commandManager, query, editorEvents)
    {
        _strategy = new ScaleToolStrategy(componentRegistry, query);
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
                _startScale = entityTransform.Scale;
                _currentScale = entityTransform.Scale;
                _deltaScale = Vector3.Zero;
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
                
                _currentScale = entityTransform.Scale;
                _deltaScale = _currentScale - _startScale;
                
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
            
            _deltaScale = Vector3.Zero;
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

        // Always work with absolute scale for manual input
        var newScale = new Vector3((float)x, (float)y, (float)z);

        if (entityTransform.Scale != newScale)
        {
            entityTransform.Scale = newScale;
            entityTransform.IsDirty = true;
            entityTransform.WorldMatrix = entityTransform.LocalMatrix;
            entityTransform.IsDirty = false;

            _currentScale = entityTransform.Scale;
            if (!_isTransforming)
            {
                _startScale = _currentScale;
                _deltaScale = Vector3.Zero;
            }
            else
            {
                _deltaScale = _currentScale - _startScale;
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
                ["Mode"] = "Scale",
                ["X"] = _currentScale.X,
                ["Y"] = _currentScale.Y,
                ["Z"] = _currentScale.Z,
                ["DeltaX"] = _deltaScale.X,
                ["DeltaY"] = _deltaScale.Y,
                ["DeltaZ"] = _deltaScale.Z,
                ["IsRelative"] = _isRelativeMode
            }
        };
    }
}

