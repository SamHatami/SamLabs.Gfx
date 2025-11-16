using Avalonia.OpenGL;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IScene _scene;
    private Grid _grid;
    public IRenderer Renderer {get;}
    public ISceneManager SceneManager { get; }
    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel(ISceneManager sceneManager, IRenderer renderer)
    {
        Renderer = renderer;
        SceneManager = sceneManager;        
        
        InitializeMainScene();
    }

    private void InitializeMainScene()
    {
        _scene = SceneManager.GetCurrentScene();
        _grid = new Grid();
        _scene.AddRenderable(_grid);
    }
}