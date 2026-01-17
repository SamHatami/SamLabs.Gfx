using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Camera;

public struct CameraTransitionDataComponent:IComponent
{
    public Vector3 Target { get; set; }
    public int CurrentFrame { get; set; }
    public int TotalFrames { get; set; }
}