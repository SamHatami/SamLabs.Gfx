using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Blueprints.Primitives;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Commands.Construction;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.SceneGraph;
using SamLabs.Gfx.Engine.Tools;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly EditorService _editorService;
    private Scene _scene;
    private Grid _grid;
    public EngineContext EngineContext { get; }
    public CommandManager CommandManager { get; }
    public ISceneManager SceneManager { get; }
    public TransformStateViewModel TransformStateViewModel { get; }
    public GridSettingsViewModel GridSettingsViewModel { get; set; }

    [ObservableProperty] private bool _isGridSettingsVisible;

    [ObservableProperty] private int _objectId;
    private readonly EntityFactory _entityFactory;

    [ObservableProperty] private string _currentFpsString;
    private readonly IComponentRegistry _componentRegistry;
    private readonly ToolManager _toolManager;

    public MainWindowViewModel(ISceneManager sceneManager, EngineContext engineContext, CommandManager commandManager,
        EditorService editorService, TransformStateViewModel transformStateViewModel)
    {
        _editorService = editorService;
        EngineContext = engineContext;
        _toolManager = engineContext.ToolManager;
        _entityFactory = engineContext.EntityFactory;
        _componentRegistry = engineContext.ComponentRegistry;
        CommandManager = commandManager;
        SceneManager = sceneManager;
        TransformStateViewModel = transformStateViewModel; //this should just be the ActiveToolViewModel
        GridSettingsViewModel = new GridSettingsViewModel(_componentRegistry, engineContext.EntityRegistry);
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
    }

    public void SetObjectId(int id) => ObjectId = id;

    [RelayCommand]
    public void AddBox() =>
        CommandManager.EnqueueCommand(new AddBoxCommand(CommandManager, SceneManager.GetCurrentScene(),
            _entityFactory));

    public void AddBarElement() =>
        CommandManager.EnqueueCommand(new AddBarElementCommand(CommandManager, _entityFactory));
    
    public void AddPlane() =>
        CommandManager.EnqueueCommand(new AddConstructionPlaneCommand(_entityFactory, _componentRegistry));


    public void ToggleTranslateManipulators()
    {
        if (_toolManager.ActiveTool?.ToolId == ToolIds.TransformTranslate)
            _toolManager.DeactivateCurrentTool();
        else
            _toolManager.ActivateTool(ToolIds.TransformTranslate);
    }

    public void ToggleRotateManipulator()
    {
        if (_toolManager.ActiveTool?.ToolId == ToolIds.TransformRotate)
            _toolManager.DeactivateCurrentTool();
        else
            _toolManager.ActivateTool(ToolIds.TransformRotate);
    }

    public void ToggleScaleManipulator()
    {
        if (_toolManager.ActiveTool?.ToolId == ToolIds.TransformScale)
            _toolManager.DeactivateCurrentTool();
        else
            _toolManager.ActivateTool(ToolIds.TransformScale);
    }

    [RelayCommand]
    private void ToggleGridSettings()
    {
        IsGridSettingsVisible = !IsGridSettingsVisible;
    }

    public void ToggleCameraProjection() =>
        CommandManager.EnqueueCommand(new ToggleCameraProjectionCommand(_componentRegistry));

    public void UndoCommand() => CommandManager.UndoLatestCommand();
    public void RedoCommand() => CommandManager.RedoLatestCommand();

    public void UpdateFps(double fpsValue)
    {
        CurrentFpsString = $"FPS: {fpsValue:F2}";
    }

    partial void OnIsGridSettingsVisibleChanged(bool value)
    {
        if (value)
            EngineContext.WorkState.RequestContinuousUpdate();
        else
            EngineContext.WorkState.StopContinuousUpdate();
    }
}