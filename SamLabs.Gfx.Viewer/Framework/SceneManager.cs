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

    public SceneManager(ILogger<SceneManager> logger, Renderer renderer)
    {
        _logger = logger;
        _renderer = renderer;
    }

    private GameWindow CreateWindow(IScene scene)
    {
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(1280, 720),
            Title = "SamLabs.Gfx.Viewer",
            // To use a specific GL version uncomment below:
            // API = ContextAPI.OpenGL,
            // APIVersion = new Version(4, 5)
        };

        var window = new GameWindow(GameWindowSettings.Default, nativeSettings);

        var camera = scene.Camera;
        
        window.Load += LoadScene;

        window.Resize += e =>
        {
            GL.Viewport(0, 0, window.Size.X, window.Size.Y);
            camera.AspectRatio = window.Size.X / (float)window.Size.Y;
        };

        // Simple input for orbit (left mouse) and pan (right mouse) + zoom (scroll)
        bool isLeftDown = false;
        bool isRightDown = false;
        Vector2 lastPos = Vector2.Zero;

        window.MouseDown += (MouseButtonEventArgs e) =>
        {
            if (e.Button == MouseButton.Left) isLeftDown = true;
            if (e.Button == MouseButton.Right) isRightDown = true;
        };

        window.MouseUp += (MouseButtonEventArgs e) =>
        {
            if (e.Button == MouseButton.Left) isLeftDown = false;
            if (e.Button == MouseButton.Right) isRightDown = false;
        };

        window.MouseMove += (MouseMoveEventArgs e) =>
        {
            var pos = e.Position;
            if (isLeftDown)
            {
                var delta = pos - lastPos;
                camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
            }
            else if (isRightDown)
            {
                var delta = pos - lastPos;
                camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
            }

            lastPos = pos;
        };

        window.MouseWheel += (MouseWheelEventArgs e) => { camera.Zoom(e.OffsetY * 0.5f); };

        //This is where the rendering happens
        window.RenderFrame += RenderFrame;


        return window;
    }

    private void RenderFrame(FrameEventArgs obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //The current scene should just have a draw.
        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);
        if (_currentScene.Grid is not null)
            _currentScene.Grid.Draw(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);

        foreach (var renderable in _currentScene.GetRenderables())
        {
            renderable.Draw();
        }
        _window.SwapBuffers();
    }

    private void LoadScene()
    {
        //If current scene is null, create default scene
        if (_currentScene is null)
            _currentScene = DefaultScene();
        if (_window is null)
            return;

        _renderer.Initialize();
        _currentScene.Grid.InitializeGL();
        _currentScene.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
        _currentScene.Camera.AspectRatio = _window.Size.X / (float)_window.Size.Y;
        GL.ClearColor(0.12f, 0.12f, 0.14f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
    }

    public IScene GetCurrentScene()
    {
        if (_currentScene is null)
            return DefaultScene();

        return _currentScene;
    }

    private IScene DefaultScene()
    {
        return new Scene()
        {
            Camera = Camera.CreateDefault(),
            Grid = new Grid()
        };
    }

    public void SetCurrentScene(IScene scene)
    {
    }

    public void Run(IScene scene)
    {
        _window ??= CreateWindow(scene);
        try
        {
            _window.Run();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while running the window.");
        }
    }

    public void Dispose()
    {
        // Check if the window was created and ensure it's closed/disposed.
        if (_window != null && _window.Exists)
        {
            _window.Close();
        }

        _window?.Dispose();
        _window = null;

        if (_renderer is IDisposable disposableRenderer)
        {
            disposableRenderer.Dispose();
        }
    }
}