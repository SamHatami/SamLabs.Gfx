using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.SceneGraph;

public interface ICamera
{
    Vector3 Position { get; set; }
    Vector3 Target { get; set; }
    Vector3 Up { get; set; }
    
    float AspectRatio { get; set; }
    float Fov { get; set; }
    float Near { get; set; }
    float Far { get; set; }
    
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);

    public static abstract ICamera CreateDefault();

}