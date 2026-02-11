using System.ComponentModel;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Utility;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Tools.Drawing;

/// <summary>
/// CAD-like line drawing tool for truss bar elements.
/// Workflow: Activate → click to set start point → line follows cursor → click to place end point
/// (creating a bar) → end point becomes start of next segment → Escape/right-click to finish.
/// </summary>
public class DrawBarTool : ITool, INotifyPropertyChanged
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly CommandManager _commandManager;
    private readonly EntityRegistry _entityRegistry;
    private readonly EntityFactory _entityFactory;
    private readonly EditorWorkState _workState;

    private ToolState _state = ToolState.Inactive;
    private Vector3 _startPoint;
    private Vector3 _currentPoint;
    private bool _hasStartPoint;

    public string ToolId => ToolIds.DrawBar;
    public string DisplayName => "Draw Bar";
    public ToolCategory Category => ToolCategory.Sketch;
    public ToolState State => _state;

    public Vector3 StartPoint => _startPoint;
    public Vector3 CurrentPoint => _currentPoint;
    public bool HasStartPoint => _hasStartPoint;

    public event EventHandler<ToolStateChangedArgs>? StateChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public DrawBarTool(
        IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityRegistry entityRegistry,
        EntityFactory entityFactory,
        EditorWorkState workState)
    {
        _componentRegistry = componentRegistry;
        _commandManager = commandManager;
        _entityRegistry = entityRegistry;
        _entityFactory = entityFactory;
        _workState = workState;
    }

    public void Activate()
    {
        _hasStartPoint = false;
        _startPoint = Vector3.Zero;
        _currentPoint = Vector3.Zero;
        SetState(ToolState.Active);
        _workState.RequestContinuousUpdate();
    }

    public void Deactivate()
    {
        _hasStartPoint = false;
        SetState(ToolState.Inactive);
        _workState.StopContinuousUpdate();
    }

    public void ProcessInput(FrameInput input)
    {
        if (_state == ToolState.Inactive) return;

        // Cancel on Escape or right-click
        if (input.Cancellation || input.IsMouseRightButtonDown)
        {
            if (_hasStartPoint)
            {
                // Just cancel the current segment, stay in tool
                _hasStartPoint = false;
                SetState(ToolState.Active);
            }
            else
            {
                Deactivate();
            }
            return;
        }

        // Get mouse world position on the drawing plane
        var worldPos = GetWorldPositionFromMouse(input);
        if (!worldPos.HasValue) return;

        _currentPoint = worldPos.Value;
        OnPropertyChanged(nameof(CurrentPoint));

        if (input.LeftClickOccured)
        {
            if (!_hasStartPoint)
            {
                // First click: set start point
                _startPoint = _currentPoint;
                _hasStartPoint = true;
                SetState(ToolState.InputCapture);
                OnPropertyChanged(nameof(StartPoint));
                OnPropertyChanged(nameof(HasStartPoint));
            }
            else
            {
                // Second click: create bar between start and current, then chain
                var start = _startPoint;
                var end = _currentPoint;

                _commandManager.EnqueueCommand(
                    new AddBarAtPositionsCommand(_commandManager, _entityFactory, start, end));

                // Chain: end becomes new start
                _startPoint = end;
                OnPropertyChanged(nameof(StartPoint));
            }
        }
    }

    private Vector3? GetWorldPositionFromMouse(FrameInput input)
    {
        var cameraId = _entityRegistry.Query.With<CameraComponent>().First();
        if (cameraId == -1) return null;

        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraId);
        ref var cameraTransform = ref _componentRegistry.GetComponent<TransformComponent>(cameraId);

        var mouseRay = cameraData.ScreenPointToWorldRay(
            new Vector2((float)input.MousePosition.X, (float)input.MousePosition.Y),
            input.ViewportSize);

        // Project onto the XY plane (Z=0) - standard 2D truss drawing plane
        var drawingPlane = new Plane(Vector3.Zero, Vector3.UnitZ);

        if (!drawingPlane.RayCast(mouseRay, out var hit))
            return null;

        return mouseRay.GetPoint(hit);
    }

    public IToolUIDescriptor GetUIDescriptor() => new DrawBarToolUIDescriptor(this);

    private void SetState(ToolState newState)
    {
        if (_state != newState)
        {
            var oldState = _state;
            _state = newState;
            StateChanged?.Invoke(this, new ToolStateChangedArgs(oldState, newState));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DrawBarToolUIDescriptor : IToolUIDescriptor
{
    private readonly DrawBarTool _tool;

    public DrawBarToolUIDescriptor(DrawBarTool tool) => _tool = tool;

    public ToolUIType UIType => ToolUIType.None;
    public string ViewModelTypeName => string.Empty;

    public Dictionary<string, object> GetDisplayData() => new()
    {
        ["HasStartPoint"] = _tool.HasStartPoint,
        ["StartPoint"] = _tool.StartPoint,
        ["CurrentPoint"] = _tool.CurrentPoint
    };

    public void UpdateFromUI(Dictionary<string, object> data) { }
}
