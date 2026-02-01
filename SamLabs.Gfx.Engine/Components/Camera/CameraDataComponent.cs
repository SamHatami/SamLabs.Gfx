using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Camera;

public struct CameraDataComponent : IComponent
{
    public ProjectionType ProjectionType { get; set; }
    public float OrthographicSize { get; set; } = 10f;
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; } = Vector3.UnitY;
    public float AspectRatio { get; set; } = 16f / 9f;
    public float Fov { get; set; } = MathF.PI / 3f; // 60 degrees
    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 10000f;
    public float DistanceToTarget;
    public float Pitch { get; set; } = 0;
    public float Yaw { get; set; } = 0;
    
    public Matrix4 ViewMatrix { get; set; }
    public Matrix4 ProjectionMatrix { get; set; }
    public bool IsActive { get; set; } = true; //If we ever gonna have more than one camera
    public bool IsTransitioning { get; set; }

    public CameraDataComponent()
    {
        DistanceToTarget = 0;
        ProjectionType = ProjectionType.Perspective;
        Target = default;
    }


}
