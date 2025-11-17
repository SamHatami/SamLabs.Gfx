using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.Threading;
using Avalonia.VisualTree;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.StandAlone.Models.OpenTk;
using SamLabs.Gfx.StandAlone.ViewModels;

namespace SamLabs.Gfx.StandAlone.Models;

public class ViewportRenderingControl : OpenTkControlBase
{
    public static readonly DirectProperty<ViewportRenderingControl, ISceneManager> SceneManagerProperty =
        AvaloniaProperty.RegisterDirect<ViewportRenderingControl, ISceneManager>(
            nameof(SceneManager),
            o => o.SceneManager,
            (o, v) => o.SceneManager = v);

    private ISceneManager _sceneManager;

    public ISceneManager SceneManager
    {
        get => _sceneManager;
        set => SetAndRaise(SceneManagerProperty, ref _sceneManager, value);
    }

    public static readonly DirectProperty<ViewportRenderingControl, IRenderer> RendererProperty =
        AvaloniaProperty.RegisterDirect<ViewportRenderingControl, IRenderer>(nameof(Renderer), o => o.Renderer,
            (o, v) => o.Renderer = v);

    private IRenderer _renderer;

    public IRenderer Renderer
    {
        get => _renderer;
        set => SetAndRaise(RendererProperty, ref _renderer, value);
    }

    private IScene _currentScene;
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

    public ConcurrentQueue<Action> Actions { get; } = new();
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        //Count fps

        base.OnOpenGlRender(gl, fb);
    }

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        _currentScene.Camera.AspectRatio = (float)width / (float)height;

        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);

        //Dequeue actions --> probably move this to somewhere else

        _currentScene.Actions.TryDequeue(out var sceneAction);
        sceneAction?.Invoke();

        // //First render pass to picking buffer
        _renderer.ClearPickingBuffer(_mainViewport);
        _renderer.RenderToPickingBuffer(_mainViewport);
        GL.Disable(EnableCap.Blend);
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.DrawPickingId();
        GL.Enable(EnableCap.Blend);

        StorePickingId(_currentMousePosition);

        _renderer.StopRenderToBuffer();


        //Second render pass to viewport buffer -> Avalonia already has a framebuffer bound for this, which is fb
        // _renderer.ClearViewportBuffer(_mainViewport);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainScreenFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
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
            _currentScene.Camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
        //Draw orbiting sphere
        else if (_rightMouseButtonPressed)
            _currentScene.Camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));


        ReadPickingId();
    }

    protected override void InitializeOpenTk()
    {
        if (Renderer == null)
            return;

        //TODO: THis is still needed, just not for the main rendering pass
        // _mainViewport =
        //     (ViewPort)_renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height); 

        Renderer.Initialize();
        _mainViewport = Renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height);
        _currentScene = SceneManager.GetCurrentScene();
        _currentScene?.Grid.InitializeGL();
        _currentScene?.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
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
        _currentScene.Camera.AspectRatio = (float)e.NewSize.Width / (float)e.NewSize.Height;
        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);

        GL.Viewport(0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height);

        // RequestNextFrameRendering();
        // _renderer.ResizeViewportBuffers(_mainViewport, (int)e.NewSize.Width, (int)e.NewSize.Height);
    }

    private void StorePickingId(Point localMousePos)
    {
        int localX = (int)localMousePos.X;
        int localY = (int)localMousePos.Y;

        localX = Math.Max(0, Math.Min(localX, _mainViewport.FullRenderView.Width - 1));
        localY = Math.Max(0, Math.Min(localY, _mainViewport.FullRenderView.Height - 1));

        Console.WriteLine(localX + " " + localY);
        _readPickingIndex ^= 1;
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _mainViewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
        GL.ReadPixels(localX, localY, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
    }

    private void ReadPickingId()
    {
        unsafe
        {
            //Swap PixelbufferIndex
            _readPickingIndex ^= 1;
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