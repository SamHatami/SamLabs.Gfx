using System.Numerics;

namespace CPURendering.View;

public class Camera
{
    public Vector3 Direction { get; }
    public Vector3 Position { get; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; }


    public Matrix4x4 LookAt(Vector3 target)
    {
        return Matrix4x4.CreateLookAt(Vector3.Zero,target,Vector3.UnitY);
    }
}