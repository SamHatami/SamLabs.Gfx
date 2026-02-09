using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Tools;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class SketchToolViewModel : ViewModelBase
{
    private readonly ToolManager _toolManager;
    private readonly EditorEvents _editorEvents;

    [ObservableProperty] private bool _isLineToolActive;
    [ObservableProperty] private bool _isFilletActive;
    
    // Constraint toggle states
    [ObservableProperty] private bool _isPerpendicularActive;
    [ObservableProperty] private bool _isAngularActive;
    [ObservableProperty] private bool _isParallelActive;
    [ObservableProperty] private bool _isEqualityActive;
    [ObservableProperty] private bool _isVerticalActive;
    [ObservableProperty] private bool _isHorizontalActive;
    [ObservableProperty] private bool _isFullLockActive;

    public SketchToolViewModel(ToolManager toolManager, EditorEvents editorEvents)
    {
        _toolManager = toolManager;
        _editorEvents = editorEvents;

        // Subscribe to tool manager events
        _editorEvents.ToolActivated += OnToolActivated;
        _editorEvents.ToolDeactivated += OnToolDeactivated;
    }

    private void OnToolActivated(object? sender, ToolEventArgs e)
    {
        IsLineToolActive = e.ToolId == ToolIds.SketchLine;
    }

    private void OnToolDeactivated(object? sender, ToolEventArgs e)
    {
        if (e.ToolId == ToolIds.SketchLine)
            IsLineToolActive = false;
    }

    [RelayCommand]
    public void ToggleLineTool()
    {
        if (_toolManager.ActiveTool?.ToolId == ToolIds.SketchLine)
            _toolManager.DeactivateCurrentTool();
        else
            _toolManager.ActivateTool(ToolIds.SketchLine);
    }

    [RelayCommand]
    public void ToggleFillet()
    {
        IsFilletActive = !IsFilletActive;
        // TODO: Implement fillet tool activation when ready
    }

    // Constraint Commands
    [RelayCommand]
    public void TogglePerpendicular()
    {
        IsPerpendicularActive = !IsPerpendicularActive;
        // TODO: Apply perpendicular constraint to selected segments
    }

    [RelayCommand]
    public void ToggleAngular()
    {
        IsAngularActive = !IsAngularActive;
        // TODO: Apply angular constraint to selected segments
    }

    [RelayCommand]
    public void ToggleParallel()
    {
        IsParallelActive = !IsParallelActive;
        // TODO: Apply parallel constraint to selected segments
    }

    [RelayCommand]
    public void ToggleEquality()
    {
        IsEqualityActive = !IsEqualityActive;
        // TODO: Apply equality constraint to selected segments
    }

    [RelayCommand]
    public void ToggleVertical()
    {
        IsVerticalActive = !IsVerticalActive;
        // TODO: Apply vertical constraint to selected segments
    }

    [RelayCommand]
    public void ToggleHorizontal()
    {
        IsHorizontalActive = !IsHorizontalActive;
        // TODO: Apply horizontal constraint to selected segments
    }

    [RelayCommand]
    public void ToggleFullLock()
    {
        IsFullLockActive = !IsFullLockActive;
        // TODO: Apply full lock constraint to selected segments
    }

    // Dimensioning Commands
    [RelayCommand]
    public void AddLinearDimension()
    {
        // TODO: Implement linear dimension creation
    }

    [RelayCommand]
    public void AddAngularDimension()
    {
        // TODO: Implement angular dimension creation
    }

    [RelayCommand]
    public void AddRadialDimension()
    {
        // TODO: Implement radial dimension creation
    }
}


