using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.Viewer.Framework;

public class SceneManager : ISceneManager
{
    private readonly ILogger<SceneManager> _logger;
    private IScene? _currentScene;

    public SceneManager(ILogger<SceneManager> logger)
    {
        _logger = logger;
    }


    public IScene GetCurrentScene()
    {
        return _currentScene ??= DefaultScene();
    }

    public void AddRenderable(IRenderable renderable)
    {
        _currentScene?.AddRenderable(renderable);
    }


    private IScene DefaultScene()
    {
        return new Scene
        {
            Camera = Camera.CreateDefault(),
            Grid = new Grid(),
        };
    }
}

