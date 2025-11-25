using Avalonia;
using Avalonia.Input;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.IO;

public ref struct FrameInput
{
    public Key KeyDown { get; set; }
    public Key KeyUp { get; set; }
    public Vector2 DeltaMouseMove { get; set; }
    public Point MousePosition { get; set; }
    public bool IsMouseLeftButtonDown { get; set; }
    public bool IsMouseRightButtonDown { get; set; }
    public bool IsMouseMiddleButtonDown { get; set; }
    public bool IsMouseCaptured { get; private set; }
    public float MouseWheelDelta { get; set; }

    public void CaptureMouse() => IsMouseCaptured = true;
    public void ReleaseMouse() => IsMouseCaptured = false;
    
}