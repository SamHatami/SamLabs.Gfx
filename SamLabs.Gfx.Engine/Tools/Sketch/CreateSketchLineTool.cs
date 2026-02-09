using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Sketch;
using SamLabs.Gfx.Engine.Components.Sketch.Geometry;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Tools.Sketch;

public enum SketchLineToolState
{
    Idle,
    AwaitingFirstPoint,
    AwaitingSecondPoint,
    PreviewLine
}

public class CreateSketchLineTool : ITool
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly CommandManager _commandManager;

    private SketchLineToolState _toolState = SketchLineToolState.Idle;
    private int _activeSketchEntityId = -1;
    private PlaneDataComponent _sketchPlane;
    private Vector3 _xAxis;
    private Vector3 _yAxis;
    private Vector2 _firstPointLocal;
    private Vector2 _lastMousePointLocal;
    private Vector3 _previewEnd = Vector3.Zero;
    private float _gridSize = 0.5f;
    private bool _snapToGrid = true;
    private readonly ILogger<CreateSketchLineTool> _logger;

    public string ToolId => ToolIds.SketchLine;
    public string DisplayName => "Sketch Line";
    public ToolCategory Category => ToolCategory.Sketch;
    public ToolState State { get; private set; } = ToolState.Inactive;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ToolStateChangedArgs>? StateChanged;

    public CreateSketchLineTool(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, 
        CommandManager commandManager, ILogger<CreateSketchLineTool> logger)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _commandManager = commandManager;
        _logger = logger;
    }

    public void Activate()
    {
        _logger.LogInformation("CreateSketchLineTool.Activate() called");
        
        // Find active sketch entity
        var sketchEntities = _entityRegistry.Query.With<SketchComponent>().Get();
        _logger.LogInformation($"Found {sketchEntities.Length} sketch entities");
        
        if (sketchEntities.IsEmpty())
        {
            _logger.LogWarning("No sketch entities found, tool cannot activate");
            State = ToolState.Inactive;
            return;
        }

        _activeSketchEntityId = sketchEntities[0];
        _logger.LogInformation($"Using sketch entity ID: {_activeSketchEntityId}");
        
        if (!_componentRegistry.HasComponent<PlaneDataComponent>(_activeSketchEntityId))
        {
            _logger.LogError($"Sketch entity {_activeSketchEntityId} missing PlaneDataComponent!");
            State = ToolState.Inactive;
            return;
        }
        
        _sketchPlane = _componentRegistry.GetComponent<PlaneDataComponent>(_activeSketchEntityId);
        _logger.LogInformation($"Sketch plane - Origin: {_sketchPlane.Origin}, Normal: {_sketchPlane.Normal}");
        
        // Get basis vectors for the sketch plane
        var basis = SketchCoordinateUtility.GetSketchPlaneBasis(_sketchPlane.Normal);
        _xAxis = basis.xAxis;
        _yAxis = basis.yAxis;
        _logger.LogInformation($"Basis vectors - X: {_xAxis}, Y: {_yAxis}");

        _toolState = SketchLineToolState.AwaitingFirstPoint;
        State = ToolState.Active;
        _logger.LogInformation("Tool activated successfully, awaiting first point");
        StateChanged?.Invoke(this, new ToolStateChangedArgs(ToolState.Inactive, ToolState.Active));
    }

    public void Deactivate()
    {
        _toolState = SketchLineToolState.Idle;
        _activeSketchEntityId = -1;
        State = ToolState.Inactive;
        StateChanged?.Invoke(this, new ToolStateChangedArgs(ToolState.Active, ToolState.Inactive));
    }

    public void ProcessInput(FrameInput input)
    {
        if (State != ToolState.Active)
        {
            _logger.LogInformation($"ProcessInput: State is {State}, not Active. Returning.");
            return;
        }

        _logger.LogInformation($"ProcessInput: ToolState={_toolState}, LeftClickOccured={input.LeftClickOccured}, IsMouseDown={input.IsMouseLeftButtonDown}");

        var cameraId = _entityRegistry.Query.With<CameraComponent>().First();
        if (cameraId == -1)
        {
            _logger.LogWarning("No camera entity found!");
            return;
        }

        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraId);

        // Get mouse ray
        var mouseRay = SketchCoordinateUtility.ScreenToWorldRay(
            new Vector2((float)input.MousePosition.X, (float)input.MousePosition.Y),
            input.ViewportSize,
            cameraData
        );

        // Intersect with sketch plane
        var intersectionPoint = SketchCoordinateUtility.RayPlaneIntersection(mouseRay, _sketchPlane);
        if (intersectionPoint == null)
        {
            _logger.LogWarning($"ProcessInput: Ray did not intersect plane");
            return;
        }

        var worldPoint = intersectionPoint.Value;
        var localCoords = SketchCoordinateUtility.WorldToSketchLocal(worldPoint, _sketchPlane, _xAxis, _yAxis);

        // Apply snapping if enabled
        if (_snapToGrid)
            localCoords = SketchCoordinateUtility.SnapToGrid(localCoords, _gridSize);

        _lastMousePointLocal = localCoords;
        _previewEnd = SketchCoordinateUtility.SketchLocalToWorld(localCoords, _sketchPlane, _xAxis, _yAxis);

        switch (_toolState)
        {
            case SketchLineToolState.AwaitingFirstPoint:
                if (input.LeftClickOccured)
                {
                    _logger.LogInformation($"First point clicked at local: {localCoords}, world: {worldPoint}");
                    _firstPointLocal = localCoords;
                    _toolState = SketchLineToolState.AwaitingSecondPoint;
                    _logger.LogInformation($"State changed to AwaitingSecondPoint");
                }
                break;

            case SketchLineToolState.AwaitingSecondPoint:
                _logger.LogInformation($"AwaitingSecondPoint frame: LeftClickOccured={input.LeftClickOccured}");
                
                if (input.LeftClickOccured)
                {
                    _logger.LogInformation($"Second point clicked at local: {localCoords}, world: {worldPoint}");
                    // Create the line segment
                    CommitLineSegment(_firstPointLocal, localCoords);
                    
                    // Option: continue drawing from this point
                    _firstPointLocal = localCoords;
                    _toolState = SketchLineToolState.AwaitingSecondPoint;
                    _logger.LogInformation($"Line committed, state reset to AwaitingSecondPoint for chaining");
                }
                else if (input.Cancellation)
                {
                    _logger.LogInformation("Line cancelled, returning to first point");
                    // Cancel and go back to first point
                    _toolState = SketchLineToolState.AwaitingFirstPoint;
                    State = ToolState.Active;
                }
                // For preview/visual feedback, you could set PreviewLine here and check it in rendering
                break;
        }
    }

    private void CommitLineSegment(Vector2 startLocal, Vector2 endLocal)
    {
        var startWorld = SketchCoordinateUtility.SketchLocalToWorld(startLocal, _sketchPlane, _xAxis, _yAxis);
        var endWorld = SketchCoordinateUtility.SketchLocalToWorld(endLocal, _sketchPlane, _xAxis, _yAxis);

        _logger.LogInformation($"Creating line segment from {startWorld} to {endWorld}");

        var lineSegment = new LineSegmentComponent
        {
            StartPoint = startWorld,
            EndPoint = endWorld
        };

        var command = new AddLineSegmentToSketchCommand(
            _entityRegistry,
            _componentRegistry,
            _activeSketchEntityId,
            lineSegment
        );
        
        _commandManager.EnqueueCommand(command);
        _logger.LogInformation("Line segment command enqueued");
    }

    public IToolUIDescriptor GetUIDescriptor()
    {
        return new SketchLineToolUIDescriptor();
    }

    private class SketchLineToolUIDescriptor : IToolUIDescriptor
    {
        public ToolUIType UIType => ToolUIType.None;
        public string ViewModelTypeName => string.Empty;

        public Dictionary<string, object> GetDisplayData()
        {
            return new Dictionary<string, object>();
        }

        public void UpdateFromUI(Dictionary<string, object> data)
        {
        }
    }
}

