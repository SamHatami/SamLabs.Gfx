using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SamLabs.Gfx.Core.Mathematics;
using SamLabs.Gfx.Viewer.Display.ImGuiBackends;
using SamLabs.Gfx.Viewer.Display.ImGuiBackends.ImGuiStyles;
using SamLabs.Gfx.Viewer.Interfaces;
using SamLabs.Gfx.Viewer.Primitives;
using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer.Display;

public class ViewerWindow : GameWindow
{
    private Scene? _currentScene;
    private IRenderer _renderer; //IRenderer 
    public ConcurrentQueue<Action> Actions { get; } = new();
    private bool _isLeftDown;
    private bool _isRightDown;
    private Vector2 _lastMousePos;
    private System.Numerics.Vector2 _uv0 = new(0, 1);
    private System.Numerics.Vector2 _uv1 = new(1, 0);
    private ViewPort _mainViewport;
    private bool _isViewportHovered;
    private bool _isViewportFocused;
    private int _objectHoveringId;
    private bool _pickingPass;
    private System.Numerics.Vector2 _localMousePos;

    private int _readPickingIndex = 0;

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
                _currentScene.CameraController.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
            else if (_isRightDown) _currentScene.CameraController.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
        }

        if (_isViewportHovered)
            ReadPickingId();

        try
        {
        }
        catch (Exception)
        {
            Console.WriteLine("No object under cursor");
        }

        _lastMousePos = pos;
    }


    private void OnMouseWheelZoom(MouseWheelEventArgs e)
    {
        if (!_isViewportHovered) return;
        if (_currentScene == null) return;

        _currentScene.CameraController.Zoom(e.OffsetY * 0.5f);
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

        _mainViewport =
            (ViewPort)_renderer.CreateViewportBuffers("Main", Size.X, Size.Y); //( "Main", 0, 0, Size.X, Size.Y)


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
        _currentScene?.Grid.InitializeGL();
        _currentScene?.Grid.ApplyShader(_renderer.GetShaderProgram("grid"));
        if (_currentScene != null)
            _currentScene.Camera.AspectRatio = Size.X / (float)Size.Y;
    }


    public void SetRenderer(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public void Run(Scene scene)
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
        CommandPanel();
        // ImGui.ShowDemoWindow();
        MainViewPortPanel();

        ImGui.Render();
        GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        Actions.TryDequeue(out var action);

        action?.Invoke();

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
        _renderer.SendViewProjectionToBuffer(_currentScene.Camera.ViewMatrix, _currentScene.Camera.ProjectionMatrix);
        //First render pass to picking buffer
        _renderer.ClearPickingBuffer(_mainViewport);
        _renderer.RenderToPickingBuffer(_mainViewport);
        GL.Disable(EnableCap.Blend);
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.DrawPickingId();
        GL.Enable(EnableCap.Blend);

        if (_isViewportHovered)
            StorePickingId(_localMousePos);

        _renderer.StopRenderToBuffer();


        //Second render pass to viewport buffer
        _renderer.ClearViewportBuffer(_mainViewport);
        _renderer.RenderToViewportBuffer(_mainViewport);
        _currentScene?.Grid.Draw();
        foreach (var renderable in _currentScene.GetRenderables())
            renderable.Draw();

        _renderer.StopRenderToBuffer();
    }

    private void StorePickingId(System.Numerics.Vector2 localMousePos)
    {
        var localX = (int)_localMousePos.X;
        var localY = (int)_localMousePos.Y;

        localX = Math.Max(0, Math.Min(localX, _mainViewport.FullRenderView.Width - 1));
        localY = Math.Max(0, Math.Min(localY, _mainViewport.FullRenderView.Height - 1));

        _readPickingIndex ^= 1;
        GL.BindBuffer(BufferTarget.PixelPackBuffer, _mainViewport.SelectionRenderView.PixelBuffers[_readPickingIndex]);
        GL.ReadPixels((int)localX, (int)localY, 1, 1, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
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
        }
    }


    private void CommandPanel()
    {
        ImGui.Begin("Chaos Engine");

        ImGui.Text("Camera Settings");
        ImGui.Separator();

        if (_currentScene?.Camera != null)
        {
            var pos = _currentScene.Camera.Position;
            ImGui.Text($"Position: ({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");

            // Example: editable values
            var fov = _currentScene.Camera.Fov.ToDegrees(); // Get from your camera
            if (ImGui.SliderFloat("FOV", ref fov, 30.0f, 120.0f))
                try
                {
                    _currentScene.Camera.Fov = fov.ToRadians();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }

        ImGui.Separator();

        if (ImGui.Button("Create box"))
            Actions.Enqueue(() =>
            {
                var box = new Box();
                box.ApplyShader("flat");
                _currentScene?.AddRenderable(box);
            });

        var mousePos = ImGui.GetCursorScreenPos();
        ImGui.Text("Object Hovering: " + _objectHoveringId);
        ImGui.Text("Mouse Position X: " + _localMousePos.X.ToString("F2") + " Y: " + _localMousePos.Y.ToString("F2"));
        ;
        ImGui.End();
    }

    private void MainViewPortPanel()
    {
        var style = ImGui.GetStyle();
        var originalpadding = style.WindowPadding;
        style.WindowPadding = new System.Numerics.Vector2(0, 0);

        ImGui.Begin("Viewport", ImGuiWindowFlags.NoTitleBar);
        var viewportSize = ImGui.GetContentRegionAvail();
        var viewportPos = ImGui.GetCursorScreenPos();


        if ((int)viewportSize.X != _mainViewport.Width || (int)viewportSize.Y != _mainViewport.Height)
            if (viewportSize.X > 0 && viewportSize.Y > 0)
            {
                _renderer.ResizeViewportBuffers(_mainViewport, (int)viewportSize.X, (int)viewportSize.Y);
                _currentScene.Camera.AspectRatio = viewportSize.X / viewportSize.Y;
            }

        ImGui.Image(_mainViewport.FullRenderView.TextureColorBufferId,
            new System.Numerics.Vector2(viewportSize.X, viewportSize.Y), _uv0,
            _uv1);

        _isViewportHovered = ImGui.IsItemHovered();
        _isViewportFocused = ImGui.IsWindowFocused();

        if (_isViewportHovered)
        {
            _localMousePos.X = ImGui.GetMousePos().X - ImGui.GetCursorScreenPos().X;
            _localMousePos.Y = ImGui.GetCursorScreenPos().Y - ImGui.GetMousePos().Y;
        }
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
        style.WindowPadding = originalpadding;
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
        var overlayFlags =
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


    public static readonly GLDebugProc DebugProcCallback = Window_DebugProc;

    private static void Window_DebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity,
        int length, IntPtr messagePtr, IntPtr userParam)
    {
        var message = Marshal.PtrToStringAnsi(messagePtr, length);

        bool showMessage;

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