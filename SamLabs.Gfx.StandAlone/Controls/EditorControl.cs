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
    private double _mouseMoveDeltaX;
    private double _mouseMoveDeltaY;
    private double _mouseWheelDelta;
    private Key _keyDown;
    private Key _keyUp;
    
    //Render Context properties
    private int _height;
    private int _width;


    public ConcurrentQueue<Command> Actions { get; } = new();
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        //Process commands
        _width = width;
        _height = height;
        
        var frameInput = CaptureFrameInput();
        _systemManager.Update(frameInput);
        
        
        //Main render pass
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainScreenFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Viewport(0, 0, width,height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        var renderContext = CaptureRenderContext();
        _systemManager.Render(renderContext);

        GL.Disable(EnableCap.DepthTest);
        
        CommandManager.ProcessAllCommands();
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
        var frameInput = new FrameInput()
        {
            IsMouseLeftButtonDown = _leftMouseButtonPressed,
            IsMouseRightButtonDown = _rightMouseButtonPressed,
            IsMouseMiddleButtonDown = _middleMouseButtonPressed,
            MousePosition = _currentMousePosition,
            KeyDown = _keyDown, 
            KeyUp = _keyUp,
            DeltaMouseMove = new Vector2(_mouseMoveDelta.X, _mouseMoveDelta.Y),
            MouseWheelDelta = (float)_mouseWheelDelta
        };
        
        
        return frameInput;
        
    }

    private void ClearInputData()
    {
        lock (this)
        {
            _mouseMoveDelta = Vector2.Zero;
        }
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
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null); // Release the mouse
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        _currentMousePosition = e.GetPosition(this);

        // if (!Equals(e.Pointer.Captured, this))
        // {
        //     _lastMousePosition = _currentMousePosition;
        //     _mouseMoveDelta = Vector2.Zero;
        //     _isViewportHovered = false;
        //     return;
        // }

        var dX = (float)(_currentMousePosition.X - _lastMousePosition.X);
        var dY = (float)(_currentMousePosition.Y - _lastMousePosition.Y);

        lock (this)
        {
            _mouseMoveDelta += new Vector2(dX, dY);
        }

        var props = e.GetCurrentPoint(this).Properties;
        _leftMouseButtonPressed = props.IsLeftButtonPressed;
        _rightMouseButtonPressed = props.IsRightButtonPressed;
        _middleMouseButtonPressed = props.IsMiddleButtonPressed;
        _isViewportHovered = true;

        _lastMousePosition = _currentMousePosition;
        
        base.OnPointerMoved(e);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => _resizeRequested = true;

}