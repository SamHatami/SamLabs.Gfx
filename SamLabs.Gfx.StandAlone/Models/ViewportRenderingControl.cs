using Avalonia;
using Avalonia.Controls;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.StandAlone.Models.OpenTk;
using SamLabs.Gfx.Viewer.Display;

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
    private IScene _currentScene;
    private IViewPort _mainViewport;

    public IRenderer Renderer
    {
        get => _renderer;
        set => SetAndRaise(RendererProperty, ref _renderer, value);
    }

    protected override void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        
        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);
        // //First render pass to picking buffer
        // _renderer.ClearPickingBuffer(_mainViewport);
        // _renderer.RenderToPickingBuffer(_mainViewport);
        // GL.Disable(EnableCap.Blend);
        // foreach (var renderable in _currentScene.GetRenderables())
        //     renderable.DrawPickingId();
        // GL.Enable(EnableCap.Blend);
        //
        // // if (_isViewportHovered) 
        // //     StorePickingId(_localMousePos);
        //
        // _renderer.StopRenderToBuffer();


        //Second render pass to viewport buffer -> Avalonia already has a framebuffer bound for this, which is fb
        // _renderer.ClearViewportBuffer(_mainViewport);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainScreenFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // _renderer.RenderToViewportBuffer(_mainViewport);
        _currentScene?.Grid.Draw();
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.Draw();
        GL.Disable(EnableCap.DepthTest);
        // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        base.OpenTkRender(mainScreenFrameBuffer, width, height);
    }

    protected override void InitializeOpenTk()
    {
        if (Renderer == null)
            return;

        //TODO: THis is still needed, just not for the main rendering pass
        // _mainViewport =
        //     (ViewPort)_renderer.CreateViewportBuffers("Main", (int)Bounds.Width, (int)Bounds.Height); 
        
        Renderer.Initialize();
        _currentScene = SceneManager.GetCurrentScene();
        _currentScene?.Grid.InitializeGL();
        _currentScene?.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
   

        
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        
        _currentScene.Camera.AspectRatio = (float)e.NewSize.Width / (float)e.NewSize.Height;
        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);
        // _renderer.ResizeViewportBuffers(_mainViewport, (int)e.NewSize.Width, (int)e.NewSize.Height);
    }
}