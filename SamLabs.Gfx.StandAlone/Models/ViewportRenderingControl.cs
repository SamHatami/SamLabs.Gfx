using Avalonia;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.StandAlone.Models.OpenTk;

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


    public IRenderer Renderer
    {
        get;
        set => SetAndRaise(RendererProperty, ref field, value);
    }

    protected override void OpenTkRender()
    {
        base.OpenTkRender();
    }

    protected override void OpenTkInit()
    {
        if (Renderer == null)
            return;

        Renderer.Initialize();
        //Initalize the renderer
    }
}