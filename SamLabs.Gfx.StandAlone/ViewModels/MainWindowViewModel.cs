using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Entities.Primitives;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Scene _scene;
    private Grid _grid;
    public IRenderer Renderer { get; }
    public EcsRoot EcsRoot { get; }
    public CommandManager CommandManager { get; }
    public ISceneManager SceneManager { get; }
    
    public string Greeting { get; } = "Welcome to Avalonia!";

    [ObservableProperty] private int _objectId;

    public MainWindowViewModel(ISceneManager sceneManager, IRenderer renderer, EcsRoot ecsRoot, CommandManager commandManager)
    {
        Renderer = renderer;
        EcsRoot = ecsRoot;
        CommandManager = commandManager;
        SceneManager = sceneManager;

        InitializeMainScene();
    }
    private void InitializeMainScene()
    {
        SceneManager.CreateDefaultScene();
        _grid = new Grid();
        SceneManager.AddRenderable(_grid);
    }

    public void SetObjectId(int id) => ObjectId = id;

    [RelayCommand]
    public void AddBox()
    {
        _scene.Actions.Enqueue(() =>
            {
                // var box = new Box();
                // box.ApplyShader("flat");
                // _scene.AddRenderable(box);
            }
        );
    }
}