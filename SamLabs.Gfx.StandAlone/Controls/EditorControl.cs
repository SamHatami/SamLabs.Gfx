using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.StandAlone.Controls.OpenTk;
using SamLabs.Gfx.StandAlone.ViewModels;
using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.StandAlone.Controls;

/// <summary>
/// The main Avalonia control that renders the scene. This control can't be injected into the DI container,
/// instead properties are passed via Avalonias DirectProperties, from the main view model.
/// This control is the main hub for the ECS and rendering system and all rendering calls most originate from the OpenTKRender method.
/// The render context and frame input are updated inside this class.
/// </summary>
public class EditorControl : OpenTkControlBase
{
    public static readonly DirectProperty<EditorControl, CommandManager> CommandManagerProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, CommandManager>(
            nameof(CommandManager),
            o => o.CommandManager,
            (o, v) => o.CommandManager = v);

    public CommandManager CommandManager
    {
        get;
        set => SetAndRaise(CommandManagerProperty, ref field, value);
    }

    public static readonly DirectProperty<EditorControl, EcsRoot> EcsRootProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, EcsRoot>(
            nameof(EcsRoot),
            o => o.EcsRoot,
            (o, v) => o.EcsRoot = v);

    public EcsRoot EcsRoot
    {
        get;
        set => SetAndRaise(EcsRootProperty, ref field, value);
    }

    public static readonly DirectProperty<EditorControl, ISceneManager> SceneManagerProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, ISceneManager>(
            nameof(SceneManager),
            o => o.SceneManager,
            (o, v) => o.SceneManager = v);

    public ISceneManager SceneManager
    {
        get;
        set => SetAndRaise(SceneManagerProperty, ref field, value);
    }

    private IRenderer _renderer;
    private SystemManager _systemManager;

    private IViewPort _mainViewport;
    private bool _isViewportHovered;
    private Point _currentMousePosition;
    private bool _resizeRequested;


    //Frame input properties
    private Point _lastMousePosition;
    private bool _leftMouseButtonPressed;
    private bool _rightMouseButtonPressed;
    private bool _middleMouseButtonPressed;
    private ConcurrentQueue<Vector2> _pointerDeltas = new();

    private double _mouseWheelDelta;
    private Key _keyDown;
    private Key _keyUp;

    //Render Context properties
    private int _height;
    private int _width;

    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;


    private DateTime _lastUpdateTime = DateTime.Now;
    private int _frameCount = 0;
    private double _currentFps = 0.0; // The calculated FPS value
    private int _callCount;
    private DateTime _lastCheckedTime;
    private TimeSpan _checkInterval;
    private DateTime _lastProcessTime;
    private Stopwatch _frameTimer = new();
    private double _lastFrameTime;
    private const double FpsUpdateIntervalSeconds = 1.0; // Update FPS every second
    
    private bool _renderRequested = false;
    private DateTime _lastRenderTime = DateTime.Now;
    private const double MinFrameTimeMs = 16.666667; // ~60 FPS

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        _renderRequested = false;
        _lastRenderTime = DateTime.Now;
        
        _frameCount++;
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsedTime = currentTime - _lastUpdateTime;
        // Check if the update interval has passed
        if (elapsedTime.TotalSeconds >= FpsUpdateIntervalSeconds)
        {
            // Calculate the FPS: frames / elapsed time in seconds
            _currentFps = _frameCount / elapsedTime.TotalSeconds;

            // OPTIONAL: Print the FPS to the console
            System.Diagnostics.Debug.WriteLine($"FPS: {_currentFps:F2}");

            // Reset the counter and timer for the next interval
            _frameCount = 0;
            _lastUpdateTime = currentTime;
            ViewModel.UpdateFps(_currentFps);
        }

        _frameTimer.Restart();
        CommandManager.ProcessAllCommands();

        //Process commands
        _width = width;
        _height = height;

        var frameInput = CaptureFrameInput();
        var t1 = _frameTimer.Elapsed.TotalMilliseconds;
    
        _systemManager.Update(frameInput);
        var t2 = _frameTimer.Elapsed.TotalMilliseconds;
    
        _systemManager.Render(frameInput, CaptureRenderContext(mainScreenFrameBuffer));
        var t3 = _frameTimer.Elapsed.TotalMilliseconds;
    
        ClearInputData();
    
        var totalTime = _frameTimer.Elapsed.TotalMilliseconds;
        var timeSinceLastFrame = totalTime - _lastFrameTime;
    
        // Log when frame time varies significantly
        if (timeSinceLastFrame > 18.0) 
        {
            Debug.WriteLine($"Dropped Frame! Took {timeSinceLastFrame:F2}ms");
        }

        _lastFrameTime = totalTime;
        ClearInputData();
        base.OpenTkRender(mainScreenFrameBuffer, width, height);
    }

    private RenderContext CaptureRenderContext(int mainScreenFrameBuffer)
    {
        return new RenderContext()
        {
            ViewHeight = _height,
            ViewWidth = _width,
            ResizeRequested = _resizeRequested,
            ViewPort = _mainViewport,
            MainViewFrameBuffer = mainScreenFrameBuffer,
            RenderScaling = (float)(TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0)
        };
    }

    private FrameInput CaptureFrameInput()
    {
        Vector2 totalDelta = Vector2.Zero; // Initialize with OpenTK's zero vector

        while (_pointerDeltas.TryDequeue(out var delta))
        {
            totalDelta += delta;
        }

        var frameInput = new FrameInput()
        {
            IsMouseLeftButtonDown = _leftMouseButtonPressed,
            IsMouseRightButtonDown = _rightMouseButtonPressed,
            IsMouseMiddleButtonDown = _middleMouseButtonPressed,
            MousePosition = _currentMousePosition,
            KeyDown = _keyDown,
            KeyUp = _keyUp,
            DeltaMouseMove = totalDelta,
            MouseWheelDelta = (float)_mouseWheelDelta,
            ViewportSize = new Vector2((float)Bounds.Width, (float)Bounds.Height)
        };
        
        return frameInput;
    }

    private void ClearInputData()
    {
        _resizeRequested = false;
        _mouseWheelDelta = 0;
        _keyDown = Key.None;
        _keyUp = Key.None;
    }

    protected override void InitializeOpenTk()
    {
        _systemManager = EcsRoot.SystemManager;
        _renderer = EcsRoot.Renderer;

        _renderer.Initialize();
        

        _systemManager.InitializeRenderSystems(_renderer);
        _mainViewport = _renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height);
        SceneManager.GetCurrentScene();

        CommandManager.EnqueueCommand(new AddMainGridCommand(CommandManager, EcsRoot.EntityCreator));
        CommandManager.EnqueueCommand(new AddTranslateGizmosCommand(CommandManager, EcsRoot.EntityCreator));
        SizeChanged += OnSizeChanged;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        _keyDown = e.Key;
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _keyUp = e.Key;
        base.OnKeyUp(e);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        _mouseWheelDelta = e.Delta.Y;
        base.OnPointerWheelChanged(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        // if (!e.Handled)
        // {
        //     e.Pointer.Capture(this); // Lock the mouse to this control
        //     _currentMousePosition = e.GetPosition(this); // Reset "Last" so we don't get a huge jump
        // }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null); // Release the mouse
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {

        _currentMousePosition = e.GetPosition(this);
        var eventDelta = _currentMousePosition - _lastMousePosition;
        if (eventDelta.X != 0.0 || eventDelta.Y != 0.0)
        {
            _pointerDeltas.Enqueue(new Vector2((float)eventDelta.X, (float)eventDelta.Y));
        }
        
        var props = e.GetCurrentPoint(this).Properties;
        _leftMouseButtonPressed = props.IsLeftButtonPressed;
        _rightMouseButtonPressed = props.IsRightButtonPressed;
        _middleMouseButtonPressed = props.IsMiddleButtonPressed;
        _isViewportHovered = true;
        
        _lastMousePosition = _currentMousePosition;
        
        // var now = DateTime.Now;
        // var elapsed = (now - _lastRenderTime).TotalMilliseconds;
        //
        // if (elapsed >= MinFrameTimeMs && !_renderRequested)
        // {
        //     _renderRequested = true;
        //     RequestNextFrameRendering();
        // }
        
        base.OnPointerMoved(e);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => _resizeRequested = true;
}