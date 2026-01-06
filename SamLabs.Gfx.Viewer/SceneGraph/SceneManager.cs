using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.SceneGraph;

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