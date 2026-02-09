﻿﻿using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Blueprints.Primitives;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Commands.Camera;
using SamLabs.Gfx.Engine.Commands.Construction;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Sketch;
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
    public ViewPresetViewModel ViewPresetViewModel { get; set; }
    public ProceduralGeometryViewModel ProceduralGeometryViewModel { get; set; }
    public SketchToolViewModel SketchToolViewModel { get; set; }

    [ObservableProperty] private bool _isGridSettingsVisible;
    [ObservableProperty] private bool _isViewPresetVisible;
    [ObservableProperty] private bool _isSketchToolPanelVisible;

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
        TransformStateViewModel = transformStateViewModel;
        GridSettingsViewModel = new GridSettingsViewModel(_componentRegistry, engineContext.EntityRegistry);
        ViewPresetViewModel = new ViewPresetViewModel(commandManager, _componentRegistry);
        ProceduralGeometryViewModel = new ProceduralGeometryViewModel(commandManager, _entityFactory);
        SketchToolViewModel = new SketchToolViewModel(_toolManager, engineContext.EditorEvents);
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

    [RelayCommand]
    public void AddDebugCubes() =>
        CommandManager.EnqueueCommand(new AddCubeBatchCommand(_entityFactory, _componentRegistry, 1000, 100f));

    public void AddBarElement() =>
        CommandManager.EnqueueCommand(new AddBarElementCommand(CommandManager, _entityFactory));
    
    public void AddPlane() =>
        CommandManager.EnqueueCommand(new AddConstructionPlaneCommand(_entityFactory, _componentRegistry));

    [RelayCommand]
    public void CreateSketch()
    {
        // For now, create a sketch on the first available construction plane
        // TODO: In future, this should be based on selected plane or show plane picker
        var planeEntities = _componentRegistry.GetEntityIdsForComponentType<PlaneDataComponent>();
        
        if (planeEntities.Length == 0)
        {
            // Create a default construction plane first
            var planeCommand = new AddConstructionPlaneCommand(_entityFactory, _componentRegistry);
            CommandManager.EnqueueCommand(planeCommand);
            planeCommand.Execute(); // Execute immediately to get the plane entity
            
            // Get the newly created plane
            planeEntities = _componentRegistry.GetEntityIdsForComponentType<PlaneDataComponent>();
        }

        if (planeEntities.Length > 0)
        {
            var planeEntityId = planeEntities[0];
            CommandManager.EnqueueCommand(new CreateSketchCommand(_entityFactory, _componentRegistry, planeEntityId));
        }
    }


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
    public void ToggleSketchLineTool()
    {
        if (_toolManager.ActiveTool?.ToolId == ToolIds.SketchLine)
        {
            _toolManager.DeactivateCurrentTool();
            IsSketchToolPanelVisible = false;
        }
        else
        {
            // Before activating the tool, transition camera to sketch view
            TransitionCameraToSketchView();
            
            _toolManager.ActivateTool(ToolIds.SketchLine);
            IsSketchToolPanelVisible = true;
        }
    }

    private void TransitionCameraToSketchView()
    {
        // Get sketch entity to get plane data
        var sketchEntities = _componentRegistry.GetEntityIdsForComponentType<SketchComponent>();
        if (sketchEntities.Length == 0) return;

        var sketchEntityId = sketchEntities[0];
        if (!_componentRegistry.HasComponent<PlaneDataComponent>(sketchEntityId))
            return;

        var planeData = _componentRegistry.GetComponent<PlaneDataComponent>(sketchEntityId);

        // Set camera to look at the plane with orthographic projection
        CommandManager.EnqueueCommand(new SetCameraToPlaneCommand(_componentRegistry, planeData.Origin, planeData.Normal));
    }

    [RelayCommand]
    private void ToggleGridSettings()
    {
        IsGridSettingsVisible = !IsGridSettingsVisible;
    }

    [RelayCommand]
    private void ToggleViewPreset()
    {
        IsViewPresetVisible = !IsViewPresetVisible;
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