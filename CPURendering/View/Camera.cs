using System.Numerics;

namespace CPURendering.View;

public class Camera
{
    public Vector3 Direction { get; private set;}
    public Vector3 Position { get;  }
    
    public float Rotation { get;  }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; }
    
    
    public Matrix4x4 LookAt(Vector3 target)
    {
        Direction = Vector3.Normalize(target - Position);
        
        return Matrix4x4.CreateLookAt(Vector3.Zero,target,Vector3.UnitY);
    }
}