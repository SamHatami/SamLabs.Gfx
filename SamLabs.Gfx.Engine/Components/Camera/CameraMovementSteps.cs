using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Camera;

public struct CameraMovementSteps
{
    public Vector3 Position {get; set;}
    public Quaternion Rotation {get; set;}
    public Vector3 Target { get; set; }
}