using OpenTK.Mathematics;
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

public class TranslateTool : TransformTool
{
    private readonly TranslateToolStrategy _strategy;
    private Vector3 _currentPosition = Vector3.Zero;
    private Vector3 _startPosition = Vector3.Zero;
    private Vector3 _deltaThisSession = Vector3.Zero;
    private bool _isRelativeMode = true;
    
    public override string ToolId => ToolIds.TransformTranslate;
    public override string DisplayName => "Translate";
    
    public double CurrentX => _isRelativeMode ? _deltaThisSession.X : _currentPosition.X;
    public double CurrentY => _isRelativeMode ? _deltaThisSession.Y : _currentPosition.Y;
    public double CurrentZ => _isRelativeMode ? _deltaThisSession.Z : _currentPosition.Z;
    public bool IsRelativeMode => _isRelativeMode;

    public TranslateTool(
        IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityQueryService query,
        EditorEvents editorEvents)
        : base(ManipulatorType.Translate, componentRegistry, commandManager, query, editorEvents)
    {
        _strategy = new TranslateToolStrategy(componentRegistry, query);
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
                _startPosition = entityTransform.Position;
                _currentPosition = entityTransform.Position;
                _deltaThisSession = Vector3.Zero;
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
                
                _currentPosition = entityTransform.Position;
                _deltaThisSession = _currentPosition - _startPosition;
                
                OnPropertyChanged(nameof(CurrentX));
                OnPropertyChanged(nameof(CurrentY));
                OnPropertyChanged(nameof(CurrentZ));
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
            
            _deltaThisSession = Vector3.Zero;
            OnPropertyChanged(nameof(CurrentX));
            OnPropertyChanged(nameof(CurrentY));
            OnPropertyChanged(nameof(CurrentZ));
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
                ["Mode"] = "Translate",
                ["X"] = _isRelativeMode ? _deltaThisSession.X : _currentPosition.X,
                ["Y"] = _isRelativeMode ? _deltaThisSession.Y : _currentPosition.Y,
                ["Z"] = _isRelativeMode ? _deltaThisSession.Z : _currentPosition.Z,
                ["IsRelative"] = _isRelativeMode
            }
        };
    }
}

public class TransformToolUIDescriptor : IToolUIDescriptor
{
    public ToolUIType UIType { get; init; }
    public string ViewModelTypeName { get; init; } = string.Empty;
    public Dictionary<string, object> CurrentData { get; init; } = new();

    public Dictionary<string, object> GetDisplayData() => CurrentData;

    public void UpdateFromUI(Dictionary<string, object> data)
    {
        foreach (var kvp in data)
        {
            CurrentData[kvp.Key] = kvp.Value;
        }
    }
}

