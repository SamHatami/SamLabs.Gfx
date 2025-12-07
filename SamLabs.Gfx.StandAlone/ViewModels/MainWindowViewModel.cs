using System;
using System.IO;
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

    [ObservableProperty] private string _currentFpsString;

    public MainWindowViewModel(ISceneManager sceneManager, EcsRoot ecsRoot, CommandManager commandManager)
    {
        EcsRoot = ecsRoot;
        _componentManager = ecsRoot.ComponentManager;
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

        //this can be created here due no gl context needed, I goto think around this architecture a bit later on.
        _entityCreator.CreateFromBlueprint(EntityNames.MainCamera); 
    }

    public void SetObjectId(int id) => ObjectId = id;

    [RelayCommand]
    public void AddBox()
    {
        CommandManager.EnqueueCommand(new AddBoxCommand(CommandManager, SceneManager.GetCurrentScene(),
            _entityCreator));
    }

    public void AddImportedFile()
    { 
        var modelPath = Path.Combine(AppContext.BaseDirectory, "Models", "TestModel_1.obj");
        CommandManager.EnqueueCommand(new ImportObjAsyncCommand(modelPath,CommandManager, SceneManager.GetCurrentScene(),
            _entityCreator));
    }

    public void ToggleTranslateGizmos() => CommandManager.EnqueueCommand(new ToggleTranslateGizmoVisibilityCommand(CommandManager, _componentManager));

    public void UpdateFps(double fpsValue)
    {
        CurrentFpsString = $"FPS: {fpsValue:F2}";
    }
}