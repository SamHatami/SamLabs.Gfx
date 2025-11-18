using System.Numerics;
using Avalonia.Input;

namespace SamLabs.Gfx.Viewer.Systems;

public ref struct FrameInput
{
    public Key KeyDown { get; set; }
    public Key KeyUp { get; set; }
    public Vector2 DeltaMouseMove { get; set; }
    public Vector2 MousePosition { get; set; }
    public bool IsMouseLeftButtonDown { get; set; }
    public bool IsMouseRightButtonDown { get; set; }
    public bool IsMouseMiddleButtonDown { get; set; }
    public bool IsMouseCaptured { get; private set; }
    
    public void CaptureMouse() => IsMouseCaptured = true;
    public void ReleaseMouse() => IsMouseCaptured = false;
    
}