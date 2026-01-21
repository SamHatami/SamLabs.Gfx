using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SamLabs.Gfx.Editor.ViewModels;

public abstract partial class ToolStateViewModelBase:ViewModelBase
{
    [ObservableProperty] private Point _position = new Point(600, 100); // Default position: center-right, away from tools
    [ObservableProperty] private Size _size;
    [ObservableProperty] private bool _isToolActive;
    [ObservableProperty] private string _toolName = string.Empty;
    [ObservableProperty] private string _mode = string.Empty;
}