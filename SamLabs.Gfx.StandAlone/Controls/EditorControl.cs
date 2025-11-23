using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.Threading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.StandAlone.Controls.OpenTk;
using SamLabs.Gfx.StandAlone.ViewModels;
using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
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

    public static readonly DirectProperty<EditorControl, IRenderer> RendererProperty =
        AvaloniaProperty.RegisterDirect<EditorControl, IRenderer>(nameof(Renderer), o => o.Renderer,
            (o, v) => o.Renderer = v);

    public IRenderer Renderer
    {
        get => Renderer;
        set => SetAndRaise(RendererProperty, ref field, value);
    }

    private Scene _currentScene;
    private IViewPort _mainViewport;
    private int _readPickingIndex;
    private int _objectHoveringId;
    private Point _lastMousePosition;
    private bool _leftMouseButtonPressed;
    private bool _rightMouseButtonPressed;
    private Vector2 _mouseMoveDelta;
    private double _mouseMoveDeltaX;
    private double _mouseMoveDeltaY;
    private bool _isViewportHovered;
    private Point _currentMousePosition;
    private bool _resizeRequested;
    private EntityManager _EntityManager;
    private ComponentManager _ComponentManager;
    private ISceneManager _SceneManager;

    public ConcurrentQueue<Command> Actions { get; } = new();
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        //Count fps

        base.OnOpenGlRender(gl, fb);
    }

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {

        if (_resizeRequested)
        {
            _currentScene.Camera.AspectRatio = (float)width / (float)height;
            Renderer.ResizeViewportBuffers(_mainViewport, width, height);
            _resizeRequested = false;
        }
        
        
        Renderer.SendViewProjectionToBuffer(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);

        //Dequeue actions --> probably move this to somewhere else

        _currentScene.Actions.TryDequeue(out var sceneAction);
        sceneAction?.Invoke();


        foreach (var renderPass in Renderer.RenderPasses)
        {
            renderPass.Render();
        }
        // //First render pass to picking buffer
        Renderer.ClearPickingBuffer(_mainViewport);
        Renderer.RenderToPickingBuffer(_mainViewport);
        GL.Disable(EnableCap.Blend);
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.DrawPickingId();
        GL.Enable(EnableCap.Blend);

        StorePickingId(_currentMousePosition);

        Renderer.StopRenderToBuffer();

        //render data passes 

        //Second render pass to viewport buffer -> Avalonia already has a framebuffer bound for this, which is fb
        // _renderer.ClearViewportBuffer(_mainViewport);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainScreenFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Viewport(0, 0, width,height);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ProcessMouseEvents();

        _currentScene?.Grid.Draw();
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.Draw();
        GL.Disable(EnableCap.DepthTest);
        base.OpenTkRender(mainScreenFrameBuffer, width, height);
    }

    private void ProcessMouseEvents()
    {
        //Add early returns with minor float comparisons

        Vector2 delta;
        lock (this)
        {
            delta = _mouseMoveDelta;
            _mouseMoveDelta = Vector2.Zero;
        }

        if (_leftMouseButtonPressed)
            _currentScene.CameraController.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
        //Draw orbiting sphere
        else if (_rightMouseButtonPressed)
            _currentScene.CameraController.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));


        ReadPickingId();
    }

    protected override void InitializeOpenTk()
    {
        
        //Get all systems and services from DI
        _EntityManager = EcsRoot.EntityManager;
        _ComponentManager = EcsRoot.ComponentManager;
        _SceneManager = SceneManager;
        
        
        if (Renderer == null)
            return;

        //TODO: THis is still needed, just not for the main rendering pass
        // _mainViewport =
        //     (ViewPort)_renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height); 

        Renderer.Initialize();
        _mainViewport = Renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height);
        _currentScene = SceneManager.GetCurrentScene();
        _currentScene?.Grid.InitializeGL();
        _currentScene?.Grid.ApplyShader(Renderer.GetShaderProgram("grid"));
        _currentScene.Camera.AspectRatio = (float)Bounds.Width / (float)Bounds.Height;

        SizeChanged += OnSizeChanged;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        _currentMousePosition = e.GetPosition(this);

        if (!Equals(e.Pointer.Captured, this))
        {
            _lastMousePosition = _currentMousePosition;
            _mouseMoveDelta = Vector2.Zero;
            _isViewportHovered = false;
            return;
        }

        var dX = (float)(_currentMousePosition.X - _lastMousePosition.X);
        var dY = (float)(_currentMousePosition.Y - _lastMousePosition.Y);

        lock (this)
        {
            _mouseMoveDelta += new Vector2(dX, dY);
        }

        _leftMouseButtonPressed = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
        _rightMouseButtonPressed = e.GetCurrentPoint(this).Properties.IsRightButtonPressed;

        _lastMousePosition = _currentMousePosition;
        
        base.OnPointerMoved(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
    }


    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _resizeRequested = true;
        
        // RequestNextFrameRendering();
    }

    private void StorePickingId(Point localMousePos)
    {
        int localX = (int)localMousePos.X;
        int localY = (int)localMousePos.Y;

        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        int x = (int)(localMousePos.X * scaling);
        int y = (int)(localMousePos.Y * scaling);
        y = _mainViewport.SelectionRenderView.Height - y; // Flip Y

        x = Math.Clamp(x, 0, _mainViewport.SelectionRenderView.Width - 1);
        y = Math.Clamp(y, 0, _mainViewport.SelectionRenderView.Height - 1);

        _readPickingIndex ^= 1;
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _mainViewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
    }

    private void ReadPickingId()
    {
        unsafe
        {
            //Swap PixelbufferIndex
            GL.BindBuffer(BufferTarget.PixelPackBuffer,
                _mainViewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
            var pboPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
            if (pboPtr != (void*)IntPtr.Zero)
                _objectHoveringId = (int)Marshal.PtrToStructure((IntPtr)pboPtr, typeof(int));
            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

            Dispatcher.UIThread.Post(() => ViewModel.SetObjectId(_objectHoveringId), DispatcherPriority.Normal);
        }
    }
}