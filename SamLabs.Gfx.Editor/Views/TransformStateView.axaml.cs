using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using SamLabs.Gfx.Editor.ViewModels;

namespace SamLabs.Gfx.Editor.Views;

public partial class TransformStateView : UserControl
{
    private Border? _titleBar;
    private bool _isDragging;
    private Point _dragStartPoint;
    private Point _initialPosition;

    public TransformStateView()
    {
        InitializeComponent();
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // Find the title bar in the visual tree
        _titleBar = this.FindControl<Border>("TitleBar");

        if (_titleBar != null)
        {
            _titleBar.PointerPressed += OnTitleBarPointerPressed;
            _titleBar.PointerMoved += OnTitleBarPointerMoved;
            _titleBar.PointerReleased += OnTitleBarPointerReleased;
        }
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && DataContext is TransformStateViewModel viewModel)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this.Parent as Visual);
            _initialPosition = viewModel.Position;
            e.Pointer.Capture(_titleBar);
        }
    }

    private void OnTitleBarPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && DataContext is TransformStateViewModel viewModel)
        {
            var currentPoint = e.GetPosition(this.Parent as Visual);
            var delta = currentPoint - _dragStartPoint;

            // Update the ViewModel's Position property
            viewModel.Position = new Point(
                _initialPosition.X + delta.X,
                _initialPosition.Y + delta.Y
            );
        }
    }

    private void OnTitleBarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
        }
    }
}

