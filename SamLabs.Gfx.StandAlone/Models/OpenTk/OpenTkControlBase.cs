using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.StandAlone.Models.OpenTk;

public class OpenTkControlBase : OpenGlControlBase, ICustomHitTest
{
    private GlInterface? _gl;
    public AvaloniaKeyboardState KeyboardState = new();
    private AvaloniaTkContext? _avaloniaTkContext;

    /// <summary>
    /// OpenTkRender is called once a frame to draw to the control.
    /// You can do anything you want here, but make sure you undo any configuration changes after, or you may get weirdness with other controls.
    /// </summary>
    protected virtual void OpenTkRender(int mainScreenFrameBuffer, int width, int height)
    {
        //Main rendering logic goes here
    }
    
    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        _gl = gl;
        KeyboardState.OnFrame();

        var size = GetPlatformSpecificBounds();

        //Set up the aspect ratio so shapes aren't stretched.
        GL.Viewport(0, 0, size.width, size.height);

        //Tell our subclass to render
        if (Bounds.Width != 0 && Bounds.Height != 0)
        {
            OpenTkRender(fb, size.width, size.height);
        }

        //Schedule next UI update with avalonia
        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Render);
    }
    
    
    private static readonly bool OnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private double? _renderScaling = null;
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

    /// <summary>
    /// OpenTkTeardown is called once when the control is destroyed.
    /// Though GL bindings are still valid, as OpenTK provides no way to clear them, you should not invoke GL functions after this function finishes executing.
    /// At best, they will do nothing, at worst, something could go wrong.
    /// You should use this function as a last chance to clean up any GL resources you have allocated - delete buffers, vertex arrays, programs, and textures.
    /// </summary>
    protected virtual void OpenTkTeardown()
    {
    }

    /// <summary>
    /// OpenTkInit is called once when the control is first created.
    /// At this point, the GL bindings are initialized and you can invoke GL functions.
    /// You could use this function to load and compile shaders, load textures, allocate buffers, etc.
    /// </summary>

    protected override void OnOpenGlInit(GlInterface gl)
    {
        _avaloniaTkContext = new AvaloniaTkContext(gl);
        GLLoader.LoadBindings(_avaloniaTkContext);

        InitializeOpenTk();
    }


    //Simply call the subclass' teardown function
    protected sealed override void OnOpenGlDeinit(GlInterface gl)
    {
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

    public bool HitTest(Point point) => Bounds.Contains(point);


    public GlInterface? GetGlInterface()
    {
        return _gl;
    }

    private PixelSize GetPixelSize()
    {
        var scaling = TopLevel.GetTopLevel(this)!.RenderScaling;
        return new PixelSize(Math.Max(1, (int)(Bounds.Width * scaling)),
            Math.Max(1, (int)(Bounds.Height * scaling)));
    }
}