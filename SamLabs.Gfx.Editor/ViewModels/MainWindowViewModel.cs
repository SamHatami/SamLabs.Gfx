using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Entities.Primitives;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly EditorService _editorService;
    private Scene _scene;
    private Grid _grid;
    public EngineContext EngineContext { get; }
    public CommandManager CommandManager { get; }
    public ISceneManager SceneManager { get; }

    [ObservableProperty] private int _objectId;
    private readonly EntityFactory _entityFactory;

    [ObservableProperty] private string _currentFpsString;
    private readonly IComponentRegistry _componentRegistry;

    public MainWindowViewModel(ISceneManager sceneManager, EngineContext engineContext, CommandManager commandManager, EditorService editorService)
    {
        _editorService = editorService;
        //use EditorService to convey the commands rather than directly using the commandmanager, the engineContext is needed for the editorcontrol
        EngineContext = engineContext;
        _entityFactory = engineContext.EntityFactory;
        _componentRegistry = engineContext.ComponentRegistry;
        CommandManager = commandManager;
        SceneManager = sceneManager;

        InitializeMainScene();
        
        //we also need sub-viewmodels that subscribe to whatever events they need
        //SceneViewModel
        //TransformViewModel
        //PropertiesViewmodel (perhaps includes the transform?)
    }

    private void InitializeMainScene()
    {
        SceneManager.CreateDefaultScene();
        _grid = new Grid();
        SceneManager.AddRenderable(_grid);

        //this can be created here due no gl context needed, I goto think around this architecture a bit later on.
        _entityFactory.CreateFromBlueprint(EntityNames.MainCamera); 
    }

    public void SetObjectId(int id) => ObjectId = id;

    [RelayCommand]
    public void AddBox()
    {
        CommandManager.EnqueueCommand(new AddBoxCommand(CommandManager, SceneManager.GetCurrentScene(),
            _entityFactory));
    }

    public void AddImportedFile()
    { 
        var modelPath = Path.Combine(AppContext.BaseDirectory, "Models", "TestModel_1.obj");
        CommandManager.EnqueueCommand(new ImportObjAsyncCommand(modelPath,CommandManager, SceneManager.GetCurrentScene(),
            _entityFactory));
    }

    public void ToggleTranslateManipulators() => CommandManager.EnqueueCommand(new ToggleManipulatorCommand(CommandManager, ManipulatorType.Translate, _componentRegistry));
    public void ToggleRotateManipulator() => CommandManager.EnqueueCommand(new ToggleManipulatorCommand(CommandManager, ManipulatorType.Rotate, _componentRegistry));
    public void ToggleScaleManipulator() => CommandManager.EnqueueCommand(new ToggleManipulatorCommand(CommandManager, ManipulatorType.Scale, _componentRegistry));

    public void UndoCommand() => CommandManager.UndoLatestCommand();
    public void RedoCommand() => CommandManager.RedoLatestCommand();

    public void UpdateFps(double fpsValue)
    {
        CurrentFpsString = $"FPS: {fpsValue:F2}";
    }
}