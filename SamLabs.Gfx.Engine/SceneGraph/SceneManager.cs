using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.SceneGraph;

public class SceneManager : ISceneManager
{
    private readonly ILogger<SceneManager> _logger;
    private Scene? _currentScene;

    public SceneManager(EditorEvents editorEvents, ILogger<SceneManager> logger)
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


    public void CreateDefaultScene()
    {
        var defScne =new Scene
        {
            Camera = Camera.CreateDefault(),
        };
        
        _currentScene = defScne;
    }
}