﻿using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Blueprints.Primitives;
using SamLabs.Gfx.Engine.Commands;
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
    public ToolStateViewModel ToolStateViewModel { get; }

    [ObservableProperty] private int _objectId;
    private readonly EntityFactory _entityFactory;

    [ObservableProperty] private string _currentFpsString;
    private readonly IComponentRegistry _componentRegistry;

    public MainWindowViewModel(ISceneManager sceneManager, EngineContext engineContext, CommandManager commandManager, EditorService editorService, ToolStateViewModel toolStateViewModel)
    {
        _editorService = editorService;
        EngineContext = engineContext;
        _entityFactory = engineContext.EntityFactory;
        _componentRegistry = engineContext.ComponentRegistry;
        CommandManager = commandManager;
        SceneManager = sceneManager;
        ToolStateViewModel = toolStateViewModel;

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

    public void ToggleTranslateManipulators()
    {
        var toolManager = EngineContext.ToolManager;
        if (toolManager.ActiveTool?.ToolId == ToolIds.TransformTranslate)
            toolManager.DeactivateCurrentTool();
        else
            toolManager.ActivateTool(ToolIds.TransformTranslate);
    }
    
    public void ToggleRotateManipulator()
    {
        var toolManager = EngineContext.ToolManager;
        if (toolManager.ActiveTool?.ToolId == ToolIds.TransformRotate)
            toolManager.DeactivateCurrentTool();
        else
            toolManager.ActivateTool(ToolIds.TransformRotate);
    }
    
    public void ToggleScaleManipulator()
    {
        var toolManager = EngineContext.ToolManager;
        if (toolManager.ActiveTool?.ToolId == ToolIds.TransformScale)
            toolManager.DeactivateCurrentTool();
        else
            toolManager.ActivateTool(ToolIds.TransformScale);
    }
    public void AddPlane() => CommandManager.EnqueueCommand(new AddConstructionPlaneCommand(_entityFactory, _componentRegistry));

    public void UndoCommand() => CommandManager.UndoLatestCommand();
    public void RedoCommand() => CommandManager.RedoLatestCommand();

    public void UpdateFps(double fpsValue)
    {
        CurrentFpsString = $"FPS: {fpsValue:F2}";
    }
}