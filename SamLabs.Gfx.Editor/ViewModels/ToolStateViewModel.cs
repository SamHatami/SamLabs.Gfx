﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Tools;
using SamLabs.Gfx.Engine.Tools.Transforms;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class ToolStateViewModel : ViewModelBase
{
    private readonly ToolManager _toolManager;
    private readonly EditorEvents _editorEvents;

    [ObservableProperty] private bool _isToolActive;
    [ObservableProperty] private string _toolName = string.Empty;
    [ObservableProperty] private string _mode = string.Empty;

    public double XValue => _toolManager.ActiveTool switch
    {
        TranslateTool translateTool => translateTool.CurrentX,
        RotateTool rotateTool => rotateTool.CurrentAngle,
        ScaleTool scaleTool => scaleTool.CurrentX,
        _ => 0.0
    };

    public double YValue => _toolManager.ActiveTool switch
    {
        TranslateTool translateTool => translateTool.CurrentY,
        ScaleTool scaleTool => scaleTool.CurrentY,
        _ => 0.0
    };

    public double ZValue => _toolManager.ActiveTool switch
    {
        TranslateTool translateTool => translateTool.CurrentZ,
        ScaleTool scaleTool => scaleTool.CurrentZ,
        _ => 0.0
    };

    public bool IsRelativeMode => _toolManager.ActiveTool is TranslateTool translateTool && translateTool.IsRelativeMode;

    public ToolStateViewModel(ToolManager toolManager, EditorEvents editorEvents)
    {
        _toolManager = toolManager;
        _editorEvents = editorEvents;

        _editorEvents.ToolActivated += OnToolActivated;
        _editorEvents.ToolDeactivated += OnToolDeactivated;
    }

    private void OnToolActivated(object? sender, ToolEventArgs e)
    {
        IsToolActive = true;
        ToolName = e.ToolName;
        Mode = e.ToolName;
        
        if (_toolManager.ActiveTool != null)
        {
            _toolManager.ActiveTool.PropertyChanged += OnToolPropertyChanged;
        }
    }

    private void OnToolDeactivated(object? sender, ToolEventArgs e)
    {
        if (_toolManager.ActiveTool != null)
        {
            _toolManager.ActiveTool.PropertyChanged -= OnToolPropertyChanged;
        }
        
        IsToolActive = false;
        ToolName = string.Empty;
        Mode = string.Empty;
        OnPropertyChanged(nameof(XValue));
        OnPropertyChanged(nameof(YValue));
        OnPropertyChanged(nameof(ZValue));
        OnPropertyChanged(nameof(IsRelativeMode));
    }

    private void OnToolPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "CurrentX")
            OnPropertyChanged(nameof(XValue));
        else if (e.PropertyName == "CurrentY")
            OnPropertyChanged(nameof(YValue));
        else if (e.PropertyName == "CurrentZ")
            OnPropertyChanged(nameof(ZValue));
        else if (e.PropertyName == "CurrentAngle")
            OnPropertyChanged(nameof(XValue));
    }
}

