using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using OpenTK.Mathematics;
using SamLabs.Gfx.Editor.Controls.OpenTk;
using SamLabs.Gfx.Editor.ViewModels;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;
using SamLabs.Gfx.Engine.Systems;

namespace SamLabs.Gfx.Editor.Controls;

/// <summary>
/// The main rendering control that integrates the ECS engine with the MVVM UI layer.
/// This control serves as the integration hub between Avalonia UI and the Entity-Component-System.
/// </summary>
/// <remarks>
/// <para><strong>Architectural Role:</strong></para>
/// <para>This class bridges two architectural patterns:</para>
/// <list type="bullet">
///   <item><description><strong>MVVM (View Layer):</strong> Receives properties from MainWindowViewModel via Avalonia DirectProperties</description></item>
///   <item><description><strong>ECS (Integration Point):</strong> Orchestrates the render loop and system execution</description></item>
/// </list>
/// 
/// <para><strong>Key Responsibilities:</strong></para>
/// <list type="number">
///   <item><description>Manage the OpenGL render loop using Avalonia's OpenGL context</description></item>
///   <item><description>Capture and aggregate input events (keyboard, mouse, pointer) into FrameInput</description></item>
///   <item><description>Build RenderContext for each frame with viewport and scaling information</description></item>
///   <item><description>Coordinate SystemScheduler execution (Update and Render phases)</description></item>
///   <item><description>Handle idle detection to reduce CPU usage when editor is inactive</description></item>
///   <item><description>Support hot-reload of shaders during development (via file watcher)</description></item>
/// </list>
/// 
/// <para><strong>Design Constraints:</strong></para>
/// <para>This control cannot be registered in the DI container because it's instantiated by Avalonia's XAML loader.
/// Dependencies are injected via DirectProperties from the parent ViewModel.</para>
/// 
/// <para><strong>Thread Safety:</strong></para>
/// <para>All rendering must occur on the Avalonia UI thread within the OpenTkRender method.
/// Input events are captured on the UI thread and aggregated into thread-safe concurrent queues.</para>
/// 
/// <para><strong>Known Architectural Concerns:</strong></para>
/// <list type="bullet">
///   <item><description>This class has multiple responsibilities that could be extracted (input aggregation, idle detection)</description></item>
///   <item><description>Direct coupling to MainWindowViewModel violates strict MVVM separation</description></item>
///   <item><description>Directly subscribes to 9+ EditorEvents, creating implicit dependencies</description></item>
///   <item><description>Creates ECS commands in InitializeOpenTk (consider moving to ViewModel)</description></item>
/// </list>
/// </remarks>
public class EditorControl : OpenTkControlBase
{
    public static readonly DirectProperty<EditorControl, CommandManager> CommandManagerProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, CommandManager>(
            nameof(CommandManager),
            o => o.CommandManager,
            (o, v) => o.CommandManager = v);

    private CommandManager _commandManager;
    public CommandManager CommandManager
    {
        get => _commandManager;
        set
        {
            if (_commandManager != null)
                _commandManager.CommandEnqueued -= NotifyActivity;
            
            SetAndRaise(CommandManagerProperty, ref _commandManager, value);
            
            if (_commandManager != null)
                _commandManager.CommandEnqueued += NotifyActivity;
        }
    }

    public static readonly DirectProperty<EditorControl, EngineContext> EngineContextProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, EngineContext>(
            nameof(EngineContext),
            o => o.EngineContext,
            (o, v) => o.EngineContext = v);

    private EngineContext _engineContext;
    public EngineContext EngineContext
    {
        get => _engineContext;
        set => SetAndRaise(EngineContextProperty, ref _engineContext, value);
    }

    public static readonly DirectProperty<EditorControl, ISceneManager> SceneManagerProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, ISceneManager>(
            nameof(SceneManager),
            o => o.SceneManager,
            (o, v) => o.SceneManager = v);

    private ISceneManager _sceneManager;
    public ISceneManager SceneManager
    {
        get => _sceneManager;
        set => SetAndRaise(SceneManagerProperty, ref _sceneManager, value);
    }

    private IRenderer _renderer;
    private SystemScheduler _systemScheduler;
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

    // Performance monitoring constants
    private const double FpsUpdateIntervalSeconds = 1.0;
    private const double MinFrameTimeMs = 16.666667; // ~60 FPS
    private const double IdleTimeoutSeconds = 1.5;
    private const int LeftClickFramePersistence = 2; // Frames to keep click flag active
    
    private DateTime _lastUpdateTime = DateTime.Now;
    private int _frameCount = 0;
    private double _currentFps = 0.0;
    private Stopwatch _frameTimer = new();
    private double _lastFrameTime;
    
    private DateTime _lastRenderTime = DateTime.Now;
    private bool _leftClickOccured;
    private bool _isDragging;
    
    // Idle detection state
    private DateTime _lastActivityTime = DateTime.Now;
    private readonly TimeSpan _idleTimeout = TimeSpan.FromSeconds(IdleTimeoutSeconds);
    private bool _wasIdling = false;
    private EditorWorkState _editorWorkState;
    
    // Hot-reload support for shader development
    private FileSystemWatcher? _shaderWatcher;
    private ConcurrentQueue<string> _pendingShaderReloads = new();

    private void NotifyActivity()
    {
        _lastActivityTime = DateTime.Now;
        RequestNextFrameRendering();
    }

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        if(!_pendingShaderReloads.IsEmpty)
            NotifyActivity();
        
        if(Idle())
        {
            _wasIdling = true;
            return;
        }

        if (_wasIdling)
        {
            _lastUpdateTime = DateTime.Now;
            _frameCount = 0;
            _wasIdling = false;
        }
        
        CalculateFps();
        CommandManager.ProcessAllCommands();
        ProcessPendingShaderReloads();

        //Process commands
        _width = width;
        _height = height;

        var frameInput = CaptureFrameInput();
        var t1 = _frameTimer.Elapsed.TotalMilliseconds;
    
        _systemScheduler.Update(frameInput);
        var t2 = _frameTimer.Elapsed.TotalMilliseconds;
    
        _systemScheduler.Render(frameInput, CaptureRenderContext(mainScreenFrameBuffer));
        var t3 = _frameTimer.Elapsed.TotalMilliseconds;
    
        var totalTime = _frameTimer.Elapsed.TotalMilliseconds;
        var timeSinceLastFrame = totalTime - _lastFrameTime;
    
        // Log when frame time varies significantly
        if (timeSinceLastFrame > 18.0) 
        {
            Debug.WriteLine($"Dropped Frame! Took {timeSinceLastFrame:F2}ms");
        }

        _lastFrameTime = totalTime;
        ClearInputData();
        RequestNextFrameRendering();
        base.OpenTkRender(mainScreenFrameBuffer, width, height);
    }

    /// <summary>
    /// Determines if the editor should enter idle state to reduce CPU usage.
    /// </summary>
    /// <remarks>
    /// The editor enters idle mode when all of the following conditions are met:
    /// <list type="bullet">
    ///   <item><description>No engine work pending (WorkState.ShouldUpdate() returns false)</description></item>
    ///   <item><description>No pending commands in the CommandManager queue</description></item>
    ///   <item><description>No active tool (e.g., transform manipulator)</description></item>
    ///   <item><description>No user activity for the configured idle timeout period</description></item>
    /// </list>
    /// When idle, the render loop skips frame processing to conserve resources.
    /// </remarks>
    /// <returns>True if the editor should be in idle state; otherwise, false.</returns>
    private bool Idle()
    {
        if(_editorWorkState.ShouldUpdate()) return false;
        if (CommandManager?.HasPendingCommands == true) return false;
        if(EngineContext.ToolManager.ActiveTool != null) return false; // Don't idle while a tool is active (e.g. transform tool) 
        if (DateTime.Now - _lastActivityTime < _idleTimeout) return false;
        
        return true;
    }

    private void CalculateFps()
    {
        _lastRenderTime = DateTime.Now;
        _frameCount++;
        var currentTime = DateTime.Now;
        var elapsedTime = currentTime - _lastUpdateTime;
        if (elapsedTime.TotalSeconds >= FpsUpdateIntervalSeconds)
        {
            _currentFps = _frameCount / elapsedTime.TotalSeconds;


            _frameCount = 0;
            _lastUpdateTime = currentTime;
            ViewModel.UpdateFps(_currentFps);
        }

        _frameTimer.Restart();
    }

    /// <summary>
    /// Captures the current render context state for the ECS render systems.
    /// </summary>
    /// <param name="mainScreenFrameBuffer">The OpenGL framebuffer handle from Avalonia.</param>
    /// <returns>A RenderContext containing viewport dimensions, scaling, and framebuffer information.</returns>
    /// <remarks>
    /// This method bridges the UI layer (Avalonia) with the ECS rendering layer by packaging
    /// UI-specific information (viewport size, DPI scaling) into a format the ECS systems can consume.
    /// </remarks>
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

    /// <summary>
    /// Captures and aggregates all input events that occurred since the last frame into a FrameInput structure.
    /// </summary>
    /// <returns>A FrameInput containing the current frame's input state.</returns>
    /// <remarks>
    /// <para>This method consolidates input from multiple sources:</para>
    /// <list type="bullet">
    ///   <item><description>Mouse button states (left, right, middle)</description></item>
    ///   <item><description>Mouse position and movement deltas (aggregated from all pointer events)</description></item>
    ///   <item><description>Keyboard state (key down/up events)</description></item>
    ///   <item><description>Mouse wheel delta</description></item>
    ///   <item><description>Interaction state (dragging, clicking)</description></item>
    /// </list>
    /// <para>Input events are queued in thread-safe collections (_pointerDeltas) and consumed here
    /// to ensure all input within a frame is captured, even if multiple events fired between frames.</para>
    /// </remarks>
    private FrameInput CaptureFrameInput()
    {
        var totalDelta = Vector2.Zero; // Initialize with OpenTK's zero vector

        while (_pointerDeltas.TryDequeue(out var delta))
        {
            totalDelta += delta;
        }

        var frameInput = new FrameInput()
        {
            IsMouseLeftButtonDown = _leftMouseButtonPressed,
            IsMouseRightButtonDown = _rightMouseButtonPressed,
            IsMouseMiddleButtonDown = _middleMouseButtonPressed,
            IsDragging = _isDragging,
            LeftClickOccured = _leftClickOccured,
            Cancellation = _keyUp == Key.Escape,
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
        
        // Keep click flag true for 2 frames so tools can see it
        if (_leftClickFrameCounter > 0)
        {
            _leftClickFrameCounter--;
        }
        else
        {
            _leftClickOccured = false;
        }
        
        _mouseWheelDelta = 0;
        _keyUp = Key.None; // reset key-up so Cancellation is only true for single frame
    }

    protected override void InitializeOpenTk()
    {
        _systemScheduler = EngineContext.SystemScheduler;
        _renderer = EngineContext.Renderer;
        _editorWorkState = EngineContext.WorkState;

        _renderer.Initialize();
        _systemScheduler.InitializeRenderSystems(_renderer);
        _mainViewport = _renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height);
        SceneManager.GetCurrentScene();

        CommandManager.EnqueueCommand(new CreateMainCameraCommand(CommandManager, EngineContext.EntityFactory));
        CommandManager.EnqueueCommand(new AddMainGridCommand(CommandManager, EngineContext.EntityFactory));
        CommandManager.EnqueueCommand(new CreateManipulatorsCommand(CommandManager, EngineContext.EntityFactory));
        SizeChanged += OnSizeChanged;
        
        SubscribeToEditorEvents();
        
        InitializeShaderWatcher();
    }

    private void SubscribeToEditorEvents()
    {
        EngineContext.EditorEvents.EntityAdded += OnEditorEvent;
        EngineContext.EditorEvents.EntityRemoved += OnEditorEvent;
        EngineContext.EditorEvents.EntityUpdated += OnEditorEvent;
        EngineContext.EditorEvents.EntityDeleted += OnEditorEvent;
        EngineContext.EditorEvents.SelectedEntityChanged += OnEditorEvent;
        EngineContext.EditorEvents.SelectedEntityAdded += OnEditorEvent;
        EngineContext.EditorEvents.TransformUpdating += OnEditorEvent;
        
        EngineContext.EditorEvents.ToolActivated += OnEditorEvent;
        EngineContext.EditorEvents.ToolDeactivated += OnEditorEvent;
    }

    private void InitializeShaderWatcher()
    {
        try
        {
            // Try to get shader folder from environment variable first, fallback to relative path
            var shaderFolder = Environment.GetEnvironmentVariable("SAMLABS_GFX_SHADER_PATH") 
                               ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SamLabs.Gfx.Engine", "Rendering", "Shaders");
            
            shaderFolder = Path.GetFullPath(shaderFolder);
            
            if (!Directory.Exists(shaderFolder))
            {
                Debug.WriteLine($"Shader folder not found: {shaderFolder}. Set SAMLABS_GFX_SHADER_PATH environment variable for hot-reload support.");
                return;
            }
            
            _shaderWatcher = new FileSystemWatcher(shaderFolder, "*.*");
            _shaderWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
            _shaderWatcher.Changed += OnShaderFileChanged;
            _shaderWatcher.Created += OnShaderFileChanged;
            _shaderWatcher.Renamed += OnShaderFileChanged;
            _shaderWatcher.EnableRaisingEvents = true;
            
            Debug.WriteLine($"Watching for shader changes in: {shaderFolder}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to setup shader file watcher: {ex.Message}");
        }
    }
    
    private int _fileCallback = 0;
    private int _leftClickFrameCounter;

    private void OnShaderFileChanged(object sender, FileSystemEventArgs e)
    {
        var ext = Path.GetExtension(e.FullPath);
        if(_fileCallback < 1)
        {
            _fileCallback++;
            return;
        }
        if (ext.Equals(".vert", StringComparison.OrdinalIgnoreCase) || 
            ext.Equals(".frag", StringComparison.OrdinalIgnoreCase))
        {
            _fileCallback = 0;
            _pendingShaderReloads.Enqueue(e.FullPath);
        }
        
    }
    
    private void ProcessPendingShaderReloads()
    {
        while (_pendingShaderReloads.TryDequeue(out var shaderPath))
        {
            try
            {
                _renderer.ReloadShader(shaderPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to reload shader: {ex.Message}");
            }
        }
    }

    private void OnEditorEvent<T>(object? sender, T e) => NotifyActivity();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        NotifyActivity();
        _keyDown = e.Key;
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        NotifyActivity();
        _keyUp = e.Key; // capture key-up so the frame input can detect Escape release
        if(_keyDown == e.Key)
            _keyDown = Key.None;
        base.OnKeyUp(e);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        NotifyActivity();
        _mouseWheelDelta = e.Delta.Y;
        base.OnPointerWheelChanged(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        NotifyActivity();
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        NotifyActivity();
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            _leftClickOccured = true;
            _leftClickFrameCounter = LeftClickFramePersistence;
        }
        _isDragging = false; 
        base.OnPointerReleased(e);
        e.Pointer.Capture(null); // Release the mouse
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        NotifyActivity();
        _currentMousePosition = e.GetPosition(this);
        var eventDelta = _currentMousePosition - _lastMousePosition;
        
        if (eventDelta.X != 0.0 || eventDelta.Y != 0.0)
        {
            _pointerDeltas.Enqueue(new Vector2((float)eventDelta.X, (float)eventDelta.Y));
        }
        
        _isViewportHovered = true;
        _lastMousePosition = _currentMousePosition;
        
        if(_leftMouseButtonPressed &&  eventDelta.X != 0.0 && eventDelta.Y != 0.0)
            _isDragging = true;
        
        CaptureMouseState(e);
        
        base.OnPointerMoved(e);
    }

    private void CaptureMouseState(PointerEventArgs e)
    {
        var props = e.GetCurrentPoint(this).Properties;
        _leftMouseButtonPressed = props.IsLeftButtonPressed;
        _rightMouseButtonPressed = props.IsRightButtonPressed;
        _middleMouseButtonPressed = props.IsMiddleButtonPressed;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _resizeRequested = true;
        NotifyActivity();
    }

    protected override void OpenTkTeardown()
    {
        base.OpenTkTeardown();
        
        SizeChanged -= OnSizeChanged;
        
        if (_shaderWatcher != null)
        {
            _shaderWatcher.EnableRaisingEvents = false;
            _shaderWatcher.Changed -= OnShaderFileChanged;
            _shaderWatcher.Created -= OnShaderFileChanged;
            _shaderWatcher.Renamed -= OnShaderFileChanged;
            _shaderWatcher.Dispose();
            _shaderWatcher = null;
        }
        
        if (CommandManager != null)
            CommandManager.CommandEnqueued -= NotifyActivity;

        if (EngineContext?.EditorEvents != null)
        {
            EngineContext.EditorEvents.EntityAdded -= OnEditorEvent;
            EngineContext.EditorEvents.EntityRemoved -= OnEditorEvent;
            EngineContext.EditorEvents.EntityUpdated -= OnEditorEvent;
            EngineContext.EditorEvents.EntityDeleted -= OnEditorEvent;
            EngineContext.EditorEvents.SelectedEntityChanged -= OnEditorEvent;
            EngineContext.EditorEvents.SelectedEntityAdded -= OnEditorEvent;
            EngineContext.EditorEvents.TransformUpdating -= OnEditorEvent;
            EngineContext.EditorEvents.ToolActivated -= OnEditorEvent;
            EngineContext.EditorEvents.ToolDeactivated -= OnEditorEvent;
        }
    }
}

