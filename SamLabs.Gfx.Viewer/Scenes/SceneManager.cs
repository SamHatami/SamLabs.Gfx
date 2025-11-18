using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Viewer.Interfaces;

namespace SamLabs.Gfx.Viewer.Scenes;

public class SceneManager : ISceneManager
{
    private readonly ILogger<SceneManager> _logger;
    private Scene? _currentScene;

    public SceneManager(ILogger<SceneManager> logger)
    {
        _logger = logger;
    }


    public Scene GetCurrentScene()
    {
        return _currentScene;
    }

    public void AddRenderable(IRenderable renderable)
    {
        _currentScene?.AddRenderable(renderable);
    }


    private Scene DefaultScene(IGrid grid)
    {
        return new Scene
        {
            Camera = Camera.CreateDefault(),
            Grid = grid
        };
    }
}