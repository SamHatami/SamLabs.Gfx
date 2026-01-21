using CommunityToolkit.Mvvm.ComponentModel;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Tools;
using SamLabs.Gfx.Engine.Tools.Transforms;

namespace SamLabs.Gfx.Editor.ViewModels;

public partial class TransformStateViewModel : ToolStateViewModelBase
{
    private readonly ToolManager _toolManager;
    private readonly EditorEvents _editorEvents;
    private bool _isUpdatingFromTool;

    // Absolute values
    [ObservableProperty] private double _xValue;
    [ObservableProperty] private double _yValue;
    [ObservableProperty] private double _zValue;
    
    // Delta/Offset values
    [ObservableProperty] private double _deltaX;
    [ObservableProperty] private double _deltaY;
    [ObservableProperty] private double _deltaZ;
    
    [ObservableProperty] private bool _isRelativeMode;

    public TransformStateViewModel(ToolManager toolManager, EditorEvents editorEvents)
    {
        _toolManager = toolManager;
        _editorEvents = editorEvents;

        _editorEvents.ToolActivated += OnToolActivated;
        _editorEvents.ToolDeactivated += OnToolDeactivated;
    }

    // Partial methods called when absolute properties change
    partial void OnXValueChanged(double value) => UpdateToolFromUi();
    partial void OnYValueChanged(double value) => UpdateToolFromUi();
    partial void OnZValueChanged(double value) => UpdateToolFromUi();
    
    // Note: Delta values are read-only from UI perspective (for now)
    // They update automatically when the tool manipulates objects
    
    partial void OnIsRelativeModeChanged(bool value)
    {
        if (_isUpdatingFromTool) return;
        
        // Sync IsRelativeMode back to the tool
        if (_toolManager.ActiveTool is TranslateTool translateTool)
        {
            translateTool.IsRelativeMode = value;
        }
        else if (_toolManager.ActiveTool is ScaleTool scaleTool)
        {
            scaleTool.IsRelativeMode = value;
        }
        else if (_toolManager.ActiveTool is RotateTool rotateTool)
        {
            rotateTool.IsRelativeMode = value;
        }
    }

    private void UpdateToolFromUi()
    {
        if (_isUpdatingFromTool) return;
        
        // Update the tool with the new values from UI
        if (_toolManager.ActiveTool is TransformTool transformTool)
        {
            transformTool.UpdateValues(XValue, YValue, ZValue);
        }
    }

    private void OnToolActivated(object? sender, ToolEventArgs e)
    {
        IsToolActive = true;
        ToolName = e.ToolName;
        Mode = e.ToolName;
        
        if (_toolManager.ActiveTool != null)
        {
            _toolManager.ActiveTool.PropertyChanged += OnToolPropertyChanged;
            
            // Initialize values from the tool
            _isUpdatingFromTool = true;
            try
            {
                SyncFromTool();
            }
            finally
            {
                _isUpdatingFromTool = false;
            }
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
        
        // Reset values using direct property assignment
        _isUpdatingFromTool = true;
        try
        {
            XValue = 0;
            YValue = 0;
            ZValue = 0;
            DeltaX = 0;
            DeltaY = 0;
            DeltaZ = 0;
            IsRelativeMode = false;
        }
        finally
        {
            _isUpdatingFromTool = false;
        }
    }

    private void OnToolPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_toolManager.ActiveTool == null) return;
        
        _isUpdatingFromTool = true;
        try
        {
            // Update ViewModel properties from tool properties
            if (e.PropertyName == "CurrentX" || e.PropertyName == "CurrentY" || 
                e.PropertyName == "CurrentZ" || e.PropertyName == "DeltaX" || 
                e.PropertyName == "DeltaY" || e.PropertyName == "DeltaZ" || 
                e.PropertyName == "IsRelativeMode")
            {
                SyncFromTool();
            }
        }
        finally
        {
            _isUpdatingFromTool = false;
        }
    }

    private void SyncFromTool()
    {
        if (_toolManager.ActiveTool == null) return;
        
        if (_toolManager.ActiveTool is TranslateTool translateTool)
        {
            XValue = translateTool.CurrentX;
            YValue = translateTool.CurrentY;
            ZValue = translateTool.CurrentZ;
            DeltaX = translateTool.DeltaX;
            DeltaY = translateTool.DeltaY;
            DeltaZ = translateTool.DeltaZ;
            IsRelativeMode = translateTool.IsRelativeMode;
        }
        else if (_toolManager.ActiveTool is ScaleTool scaleTool)
        {
            XValue = scaleTool.CurrentX;
            YValue = scaleTool.CurrentY;
            ZValue = scaleTool.CurrentZ;
            DeltaX = scaleTool.DeltaX;
            DeltaY = scaleTool.DeltaY;
            DeltaZ = scaleTool.DeltaZ;
            IsRelativeMode = scaleTool.IsRelativeMode;
        }
        else if (_toolManager.ActiveTool is RotateTool rotateTool)
        {
            XValue = rotateTool.CurrentX;
            YValue = rotateTool.CurrentY;
            ZValue = rotateTool.CurrentZ;
            DeltaX = rotateTool.DeltaX;
            DeltaY = rotateTool.DeltaY;
            DeltaZ = rotateTool.DeltaZ;
            IsRelativeMode = rotateTool.IsRelativeMode;
        }
    }
}

