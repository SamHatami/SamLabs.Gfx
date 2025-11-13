using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Core.Mathematics;
using SamLabs.Gfx.Viewer.Framework.ImGuiBackends;
using SamLabs.Gfx.Viewer.Framework.ImGuiBackends.ImGuiStyles;

namespace SamLabs.Gfx.Viewer.Framework;

public class ViewerWindow : GameWindow
{
    private IScene _currentScene;
    private Renderer _renderer; //IRenderer 
    public ConcurrentQueue<Action> Actions { get; } = new();
    private bool _isLeftDown;
    private bool _isRightDown;
    private Vector2 _lastMousePos;
    private System.Numerics.Vector2 _uv0 = new(0, 1);
    private System.Numerics.Vector2 _uv1 = new(1, 0);
    private ViewPort _mainViewport;
    private bool _isViewportHovered;
    private bool _isViewportFocused;

    public ViewerWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
        gameWindowSettings, nativeWindowSettings)
    {
    }

    #region MouseEvents

    private void OnMouseDownWindow(MouseButtonEventArgs e)
    {
        if (!_isViewportHovered) return;

        if (e.Button == MouseButton.Left) _isLeftDown = true;
        if (e.Button == MouseButton.Right) _isRightDown = true;
    }

    private void OnMouseUpWindow(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left) _isLeftDown = false;
        if (e.Button == MouseButton.Right) _isRightDown = false;
    }

    private void OnMouseMoveCamera(MouseMoveEventArgs e)
    {
        var pos = e.Position;
        var delta = pos - _lastMousePos;

        if (_isViewportHovered && (_isLeftDown || _isRightDown))
        {
            if (_isLeftDown)
            {
                _currentScene.Camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
            }
            else if (_isRightDown)
            {
                _currentScene.Camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
            }
        }

        _lastMousePos = pos;
    }

    private void OnMouseWheelZoom(MouseWheelEventArgs e)
    {
        if (!_isViewportHovered) return;
        if (_currentScene == null) return;

        _currentScene.Camera.Zoom(e.OffsetY * 0.5f);
    }

    #endregion

    private void OnResize(ResizeEventArgs e)
    {
        if (_currentScene is null) return;
        GL.Viewport(0, 0, Size.X, Size.Y);
        _currentScene.Camera.AspectRatio = Size.X / (float)Size.Y;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

        _mainViewport = (ViewPort)_renderer.CreateViewport("Main", Size.X, Size.Y); //( "Main", 0, 0, Size.X, Size.Y)


        Resize += OnResize;
        MouseDown += OnMouseDownWindow;
        MouseUp += OnMouseUpWindow;
        MouseMove += OnMouseMoveCamera;
        MouseWheel += OnMouseWheelZoom;

        SetupImGui();
    }

    private void SetupImGui()
    {
        GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);

        ImGui.CreateContext();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        ImGui.StyleColorsDark();

        var darkStyleCustom = new DarkStyle();
        // ImGuiStylePtr style = ImGui.GetStyle();
        // if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        // {
        //     style.WindowRounding = 0.0f;
        //     style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
        // }

        ImguiImplOpenTk4.Init(this);
        ImguiImplOpenGL3.Init();
    }

    private void LoadScene()
    {
        _renderer.Initialize();
        _currentScene.Grid.InitializeGL();
        _currentScene.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
        _currentScene.Camera.AspectRatio = Size.X / (float)Size.Y;
    }


    public void SetRenderer(Renderer renderer)
    {
        _renderer = renderer;
    }

    public void Run(IScene scene)
    {
        _currentScene = scene;
        LoadScene();
        try
        {
            base.Run();
        }
        catch (Exception e)
        {
        }
    }


    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);


        ImguiImplOpenGL3.NewFrame();
        ImguiImplOpenTk4.NewFrame();
        ImGui.NewFrame();

        ImGui.DockSpaceOverViewport();
        CameraPositionPanel();
        ImGui.ShowDemoWindow();
        MainViewPortPanel();

        ImGui.Render();
        GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        RenderScene();

        ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

        if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
        {
            ImGui.UpdatePlatformWindows();
            ImGui.RenderPlatformWindowsDefault();
            Context.MakeCurrent();
        }


        SwapBuffers();
    }


    private void RenderScene()
    {
        if (_currentScene is null) return;


        _renderer.BeginRenderToViewPort(_mainViewport);
        _renderer.SetCamera(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);
        _currentScene.Grid.Draw();

        foreach (var renderable in _currentScene.GetRenderables())
            renderable.Draw();
        _renderer.EndRenderToViewPort();
    }


    private void CameraPositionPanel()
    {
        ImGui.Begin("Properties");

        ImGui.Text("Camera Settings");
        ImGui.Separator();

        if (_currentScene?.Camera != null)
        {
            var pos = _currentScene.Camera.Position;
            ImGui.Text($"Position: ({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");

            // Example: editable values
            float fov = 45.0f; // Get from your camera
            if (ImGui.SliderFloat("FOV", ref fov, 30.0f, 120.0f))
            {
                try
                {
                    _currentScene.Camera.Fov = fov.toRadians();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        ImGui.End();
    }

    private void MainViewPortPanel()
    {
        ImGui.Begin("Viewport", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove |
                                ImGuiWindowFlags.NoCollapse);
        var viewportSize = ImGui.GetContentRegionAvail();
        var viewportPos = ImGui.GetCursorScreenPos();

        if ((int)viewportSize.X != _mainViewport.Width || (int)viewportSize.Y != _mainViewport.Height)
        {
            if (viewportSize.X > 0 && viewportSize.Y > 0)
            {
                _renderer.ResizeViewportBuffer(_mainViewport, (int)viewportSize.X, (int)viewportSize.Y);
                _currentScene.Camera.AspectRatio = viewportSize.X / viewportSize.Y;
            }
        }

        ImGui.Image((IntPtr)_mainViewport.TextureId, new System.Numerics.Vector2(viewportSize.X, viewportSize.Y), _uv0,
            _uv1);

        _isViewportHovered = ImGui.IsItemHovered();
        _isViewportFocused = ImGui.IsWindowFocused();

        if (_isViewportHovered && _isViewportFocused)
            Trace.WriteLine("Viewport Hovered and Focused");

        // if (ImGui.BeginPopupContextWindow(null)) 
        // {
        //     // 2. Display your menu items
        //     if (ImGui.MenuItem("Reset Panel Layout"))
        //     {
        //         // Add your logic here
        //     }
        //     if (ImGui.MenuItem("Change Theme"))
        //     {
        //         // Add your logic here
        //     }
        //     ImGui.Separator();
        //     if (ImGui.MenuItem("Close Panel"))
        //     {
        //         // Handle closing the window/setting its p_open flag to false
        //     }
        //
        //     // 3. Always call EndPopup()
        //     ImGui.EndPopup();
        // }

        CreateOrientationPanel(viewportSize, viewportPos);

        ImGui.End();
    }

    private void CreateOrientationPanel(System.Numerics.Vector2 parentSize,
        System.Numerics.Vector2 basePanelPos = default)
    {
        var overlaySize = new System.Numerics.Vector2(500, 32); // Fixed size for the overlay
        var offset = new System.Numerics.Vector2(0.5f * (parentSize.X - overlaySize.X), 10);

        var overlayPos = basePanelPos + offset;

        ImGui.SetNextWindowSize(overlaySize);
        ImGui.SetNextWindowPos(overlayPos);

        // Apply flags to make it an overlay:
        ImGuiWindowFlags overlayFlags =
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoDocking |
            ImGuiWindowFlags.NoSavedSettings;

        var alwaysOpen = true;
        // Begin the overlay panel
        ImGui.Begin("Overlay", ref alwaysOpen, overlayFlags);
        {
            ImGui.Text("Orientation ");
            // ImGui.Text($"Parent size: {parentSize}");

            // Optional: Draw a line indicating the offset origin
            // ImGui.GetWindowDrawList().AddLine(...) 
        }
        ImGui.End();
    }


    public readonly static GLDebugProc DebugProcCallback = Window_DebugProc;

    private static void Window_DebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity,
        int length, IntPtr messagePtr, IntPtr userParam)
    {
        var message = Marshal.PtrToStringAnsi(messagePtr, length);

        var showMessage = true;

        switch (source)
        {
            case DebugSource.DebugSourceApplication:
                showMessage = false;
                break;
            case DebugSource.DontCare:
            case DebugSource.DebugSourceApi:
            case DebugSource.DebugSourceWindowSystem:
            case DebugSource.DebugSourceShaderCompiler:
            case DebugSource.DebugSourceThirdParty:
            case DebugSource.DebugSourceOther:
            default:
                showMessage = true;
                break;
        }

        if (showMessage)
        {
            switch (severity)
            {
                case DebugSeverity.DontCare:
                    Console.WriteLine($"[DontCare] [{source}] {message}");
                    break;
                case DebugSeverity.DebugSeverityNotification:
                    //Logger?.LogDebug($"[{source}] {message}");
                    break;
                case DebugSeverity.DebugSeverityHigh:
                    Console.Error.WriteLine($"Error: [{source}] {message}");
                    break;
                case DebugSeverity.DebugSeverityMedium:
                    Console.WriteLine($"Warning: [{source}] {message}");
                    break;
                case DebugSeverity.DebugSeverityLow:
                    Console.WriteLine($"Info: [{source}] {message}");
                    break;
                default:
                    Console.WriteLine($"[default] [{source}] {message}");
                    break;
            }
        }
    }

    public void OnClosed()
    {
        ImguiImplOpenGL3.Shutdown();
        ImguiImplOpenTk4.Shutdown();
    }

    public override void Dispose()
    {
        (_renderer as IDisposable).Dispose();
    }
}