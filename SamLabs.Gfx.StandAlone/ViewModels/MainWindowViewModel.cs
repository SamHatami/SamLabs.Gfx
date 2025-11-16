using Avalonia.OpenGL;
using SamLabs.Gfx.Core.Framework.Display;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IRenderer Renderer {get;}
    public ISceneManager SceneManager { get; }
    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel(ISceneManager sceneManager, IRenderer renderer)
    {
        Renderer = renderer;
        SceneManager = sceneManager;        
    }

}