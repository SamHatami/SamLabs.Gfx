using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Silk.NET.OpenGL;

namespace SamLabs.Gfx.Editor.Controls.OpenTk;

public class OpenTkControlBase : OpenGlControlBase, ICustomHitTest
{
    private GlInterface? _gl;
    public AvaloniaKeyboardState KeyboardState = new();
    private AvaloniaTkContext? _avaloniaTkContext;
    private GL? _silkGl;

    protected virtual void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        // Main rendering logic goes here
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        _gl = gl;
        var size = GetPlatformSpecificBounds();

        _silkGl?.Viewport(0, 0, (uint)size.width, (uint)size.height);

        if (Bounds.Width != 0 && Bounds.Height != 0)
        {
            OpenTkRender(fb, size.width, size.height);
        }
    }

    private static readonly bool OnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private double? _renderScaling;
    private Action _nextFrameAction = null!;

    private double RenderScaling => (_renderScaling ??= TopLevel.GetTopLevel(this)?.RenderScaling)
                                    ?? throw new PlatformNotSupportedException("Could not obtain TopLevel");

    private (int width, int height) GetPlatformSpecificBounds()
        => OnLinux
            ? ((int)Bounds.Width, (int)Bounds.Height)
            : (Math.Max(1, (int)(Bounds.Width * RenderScaling)),
                Math.Max(1, (int)(Bounds.Height * RenderScaling)));

    protected virtual void InitializeOpenTk()
    {
    }

    protected virtual void OpenTkTeardown()
    {
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        _avaloniaTkContext = new AvaloniaTkContext(gl);
        _silkGl = GL.GetApi(_avaloniaTkContext.GetProcAddress);
        _nextFrameAction = RequestNextFrameRendering;
        InitializeOpenTk();
    }

    protected sealed override void OnOpenGlDeinit(GlInterface gl)
    {
        _silkGl?.Dispose();
        _silkGl = null;
        OpenTkTeardown();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!IsEffectivelyVisible)
            return;

        KeyboardState.SetKey(e.Key, true);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (!IsEffectivelyVisible)
            return;

        KeyboardState.SetKey(e.Key, false);
    }

    public bool HitTest(Point point) => new Rect(Bounds.Size).Contains(point);

    public GlInterface? GetGlInterface()
    {
        return _gl;
    }
}
