using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.ECS.Entities.Primitives;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ComponentManager _componentManager;
    private Scene _scene;
    private Grid _grid;
    public EcsRoot EcsRoot { get; }
    public CommandManager CommandManager { get; }
    public ISceneManager SceneManager { get; }
    
    public string Greeting { get; } = "Welcome to Avalonia!";

    [ObservableProperty] private int _objectId;
    private readonly EntityCreator _entityCreator;

    public MainWindowViewModel(ISceneManager sceneManager, EcsRoot ecsRoot, CommandManager commandManager)
    {
        EcsRoot = ecsRoot;
        _entityCreator = ecsRoot.EntityCreator;
        CommandManager = commandManager;
        SceneManager = sceneManager;

        InitializeMainScene();
    }
    private void InitializeMainScene()
    {
        SceneManager.CreateDefaultScene();
        _grid = new Grid();
        SceneManager.AddRenderable(_grid);
        
        var camera = _entityCreator.Create(EntityNames.MainCamera); //make into generic method?
        
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