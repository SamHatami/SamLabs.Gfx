using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Camera;

public struct CameraTransitionDataComponent:IComponent
{
    public Vector3 StartPosition { get; set; }
    public Vector3 EndPosition { get; set; }
    public Vector3 StartTarget { get; set; }
    public Vector3 EndTarget { get; set; }
    public float StartPitch { get; set; }
    public float EndPitch { get; set; }
    public float StartYaw { get; set; }
    public float EndYaw { get; set; }
    public float StartDistance { get; set; }
    public float EndDistance { get; set; }
    public int CurrentFrame { get; set; }
    public int TotalFrames { get; set; }
}