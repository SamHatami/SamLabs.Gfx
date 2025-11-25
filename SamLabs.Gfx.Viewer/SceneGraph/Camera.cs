using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.SceneGraph;

public class Camera : ICamera
{
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; } = Vector3.UnitY;
    public float AspectRatio { get; set; } = 16f / 9f;
    public float Fov { get; set; } = MathF.PI / 3f; // 60 degrees
    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 10000f;
    public float DistanceToTarget;
    public float Yaw { get; set; } = 0;
    public float Pitch { get; set; } = 0;
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);

    public Camera(Vector3 position, Vector3 target, Vector3 up)
    {
        Position = position;
        Target = target;
        Up = up;

        // Initialization logic for internal state (same as original)
        DistanceToTarget = (position - target).Length;
        // derive yaw/pitch from position:
        var dir = (position - target).Normalized();
        Pitch = MathF.Asin(dir.Y);
        Yaw = MathF.Atan2(dir.X, dir.Z);
    }

    // Implementation of ICamera methods


    public static ICamera CreateDefault()
    {
        return new Camera(new Vector3(5, 5, 5), new Vector3(0, 0, 0), Vector3.UnitY);
    }

    // Private helper method to calculate new Position based on Target and spherical coordinates
}