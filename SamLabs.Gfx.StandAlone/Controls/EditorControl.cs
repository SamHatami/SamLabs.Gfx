using System;
using System.Collections.Concurrent;
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
    
    
    private Scene _currentScene;
    private IViewPort _mainViewport;
    private int _readPickingIndex;
    private int _objectHoveringId;

    private bool _isViewportHovered;
    private Point _currentMousePosition;
    private bool _resizeRequested;
    
    
    //Frame input properties
    private Point _lastMousePosition;
    private bool _leftMouseButtonPressed;
    private bool _rightMouseButtonPressed;
    private bool _middleMouseButtonPressed;
    private Vector2 _mouseMoveDelta;

    private double _mouseWheelDelta;
    private Key _keyDown;
    private Key _keyUp;
    
    //Render Context properties
    private int _height;
    private int _width;

    public ConcurrentQueue<Command> Actions { get; } = new();
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

    
    private DateTime _lastUpdateTime = DateTime.Now;
    private int _frameCount = 0;
    private double _currentFps = 0.0; // The calculated FPS value
    private Vector2 _smoothedDelta;
    private const double FpsUpdateIntervalSeconds = 1.0; // Update FPS every second
    
    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
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
            ViewModel.UpdateFps( _currentFps);
        }
        
        CommandManager.ProcessAllCommands();
        
        //Process commands
        _width = width;
        _height = height;
        
        _systemManager.Update(CaptureFrameInput());
        
        //Main render pass
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainScreenFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.LineSmooth);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Viewport(0, 0, width,height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _systemManager.Render(CaptureRenderContext());

        GL.Disable(EnableCap.DepthTest);
        
        ClearInputData();
        
        base.OpenTkRender(mainScreenFrameBuffer, width, height);
    }

    private RenderContext CaptureRenderContext()
    {
        return new RenderContext()
        {
            ViewHeight = _height,
            ViewWidth = _width,
            ResizeRequested = _resizeRequested,
            ViewPort = _mainViewport
        };
    }

    private FrameInput CaptureFrameInput()
    {
        
        var dX = (float)(_currentMousePosition.X - _lastMousePosition.X);
        var dY = (float)(_currentMousePosition.Y - _lastMousePosition.Y);

        _lastMousePosition = _currentMousePosition;
        
        var frameInput = new FrameInput()
        {
            IsMouseLeftButtonDown = _leftMouseButtonPressed,
            IsMouseRightButtonDown = _rightMouseButtonPressed,
            IsMouseMiddleButtonDown = _middleMouseButtonPressed,
            MousePosition = _currentMousePosition,
            KeyDown = _keyDown, 
            KeyUp = _keyUp,
            DeltaMouseMove = new Vector2(dX, dY),
            MouseWheelDelta = (float)_mouseWheelDelta,
            ViewportSize = new Vector2((float)Bounds.Width, (float)Bounds.Height)
        };
        
        
        return frameInput;
        
    }

    private void ClearInputData()
    {
        _mouseWheelDelta = 0;
    }

    protected override void InitializeOpenTk()
    {
        _systemManager = EcsRoot.SystemManager;
        _renderer = EcsRoot.Renderer;
        
        _renderer.Initialize();
        _systemManager.InitializeRenderSystems(_renderer);
        _mainViewport = _renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height);
        _currentScene = SceneManager.GetCurrentScene();
        
        CommandManager.EnqueueCommand(new AddMainGridCommand(CommandManager,EcsRoot.EntityCreator));
        
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
        if (!e.Handled)
        {
            e.Pointer.Capture(this); // Lock the mouse to this control
            _currentMousePosition = e.GetPosition(this); // Reset "Last" so we don't get a huge jump
        }
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null); // Release the mouse
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        _currentMousePosition = e.GetPosition(this);
        var props = e.GetCurrentPoint(this).Properties;
        _leftMouseButtonPressed = props.IsLeftButtonPressed;
        _rightMouseButtonPressed = props.IsRightButtonPressed;
        _middleMouseButtonPressed = props.IsMiddleButtonPressed;
        _isViewportHovered = true;
        
        base.OnPointerMoved(e);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => _resizeRequested = true;

}