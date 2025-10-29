using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.Viewer.Framework;

public class SceneManager : ISceneManager, IDisposable
{
    private readonly ILogger<SceneManager> _logger;
    private readonly Renderer _renderer;
    private GameWindow? _window;
    private IScene? _currentScene;

    public SceneManager(ILogger<SceneManager> logger, Renderer renderer, GameWindow window)
    {
        _logger = logger;
        _renderer = renderer;
        _window = window;
    }
    
    public void Run(IScene scene)
    {
        if (_window is null)
        {
            _logger.LogError("Window is not created.");
            return;
        }

        _currentScene = scene;
        SetupWindow();

        try
        {
            _window.Run();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while running the window.");
        }
    }

    public IScene GetCurrentScene()
    {
        return _currentScene ??= DefaultScene();
    }

    public void Dispose()
    {
        if (_window?.Exists == true)
        {
            _window.Close();
        }

        _window?.Dispose();
        _window = null;

        (_renderer as IDisposable).Dispose();
    }

    // This creates the GL context. Which makes this the root of the windowing system.
    // If any other GL calls are made before this, they will fail. The service provider must register this before all other
    // services that use GL context.
    private void SetupWindow()
    {
        _window!.Load += LoadScene;
        _window.Resize += OnResize;
        _window.RenderFrame += RenderFrame;
        SetupMouseInput();
    }
    private void LoadScene()
    {
        _currentScene ??= DefaultScene();

        if (_window is null) return;

        _renderer.Initialize();
        _currentScene.Grid.InitializeGL();
        _currentScene.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
        _currentScene.Camera.AspectRatio = _window.Size.X / (float)_window.Size.Y;

        GL.ClearColor(0.12f, 0.12f, 0.14f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
    }

    private void OnResize(ResizeEventArgs e)
    {
        if (_currentScene is null) return;

        GL.Viewport(0, 0, _window!.Size.X, _window.Size.Y);
        _currentScene.Camera.AspectRatio = _window.Size.X / (float)_window.Size.Y;
    }

    private void SetupMouseInput()
    {
        var isLeftDown = false;
        var isRightDown = false;
        var lastPos = Vector2.Zero;

        _window!.MouseDown += e =>
        {
            if (e.Button == MouseButton.Left) isLeftDown = true;
            if (e.Button == MouseButton.Right) isRightDown = true;
        };

        _window.MouseUp += e =>
        {
            if (e.Button == MouseButton.Left) isLeftDown = false;
            if (e.Button == MouseButton.Right) isRightDown = false;
        };

        _window.MouseMove += e =>
        {
            if (_currentScene is null) return;

            var pos = e.Position;
            var delta = pos - lastPos;

            if (isLeftDown)
            {
                _currentScene.Camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
            }
            else if (isRightDown)
            {
                _currentScene.Camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
            }

            lastPos = pos;
        };

        _window.MouseWheel += e =>
        {
            if (_currentScene is null) return;
            _currentScene.Camera.Zoom(e.OffsetY * 0.5f);
        };
    }


    private void RenderFrame(FrameEventArgs obj)
    {
        if (_currentScene is null) return;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);

        foreach (var renderable in _currentScene.GetRenderables())
        {
            renderable.Draw();
        }
        
        _currentScene.Grid.Draw();

        _window!.SwapBuffers();
    }

    private IScene DefaultScene()
    {
        return new Scene
        {
            Camera = Camera.CreateDefault(),
            Grid = new Grid()
        };
    }
}