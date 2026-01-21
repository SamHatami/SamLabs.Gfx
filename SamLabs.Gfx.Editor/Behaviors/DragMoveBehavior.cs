using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using SamLabs.Gfx.Editor.ViewModels;

namespace SamLabs.Gfx.Editor.Behaviors;

/// <summary>
/// Behavior that enables dragging a control within its parent Canvas by updating the ViewModel's Position property.
/// Attach to a title bar or drag handle element.
/// </summary>
public class DragMoveBehavior : Behavior<Control>
{
    private bool _isDragging;
    private Point _dragStartPoint;
    private Point _initialPosition;
    private Control? _targetControl;

    /// <summary>
    /// The target control to move. If null, moves the control's parent (the entire window/panel).
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<DragMoveBehavior, Control?>(nameof(TargetControl));

    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerReleased += OnPointerReleased;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerReleased -= OnPointerReleased;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed) return;
        
        // Determine which control to move
        _targetControl = TargetControl ?? AssociatedObject?.Parent as Control;
            
        if (_targetControl?.DataContext is ToolStateViewModelBase viewModel && 
            _targetControl.Parent is Visual parent)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(parent);
            _initialPosition = viewModel.Position;
            e.Pointer.Capture(AssociatedObject);
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && _targetControl?.DataContext is ToolStateViewModelBase viewModel &&
            _targetControl.Parent is Visual parent)
        {
            var currentPoint = e.GetPosition(parent);
            var delta = currentPoint - _dragStartPoint;

            var newX = _initialPosition.X + delta.X;
            var newY = _initialPosition.Y + delta.Y;


            var parentBounds = parent.Bounds;

            newX = Math.Max(0, Math.Min(newX, parentBounds.Width - _targetControl.Bounds.Width));
            newY = Math.Max(0, Math.Min(newY, parentBounds.Height - _targetControl.Bounds.Height));

            viewModel.Position = new Point(newX, newY);

            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging) return;
        
        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }
}
