using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Viewer.Interfaces;
using SamLabs.Gfx.Viewer.Primitives;
using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Scene _scene;
    private Grid _grid;
    public IRenderer Renderer { get; }
    public ISceneManager SceneManager { get; }
    public string Greeting { get; } = "Welcome to Avalonia!";

    [ObservableProperty] private int _objectId;

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

    public void SetObjectId(int id) => ObjectId = id;

    [RelayCommand]
    public void AddBox()
    {
        _scene.Actions.Enqueue(() =>
            {
                var box = new Box();
                box.ApplyShader("flat");
                _scene.AddRenderable(box);
            }
        );
    }
}