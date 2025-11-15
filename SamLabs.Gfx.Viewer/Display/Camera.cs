using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;

namespace SamLabs.Gfx.Viewer.Display;

public class Camera : ICamera
{

    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; } = Vector3.UnitY; 
    public float AspectRatio { get; set; } = 16f/9f;
    public float Fov { get; set; } = MathF.PI / 3f; // 60 degrees
    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 10000f;

    private float _distance;
    private float _yaw = 0;
    private float _pitch = 0;

    // Implementation of ICamera's read-only computed properties (optional in concrete class, 
    // but useful for consistency if the interface defines a default implementation)
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, Near, Far);
    
    public Camera(Vector3 position, Vector3 target, Vector3 up)
    {
        Position = position;
        Target = target;
        Up = up;
        
        // Initialization logic for internal state (same as original)
        _distance = (position - target).Length;
        // derive yaw/pitch from position:
        var dir = (position - target).Normalized();
        _pitch = MathF.Asin(dir.Y);
        _yaw = MathF.Atan2(dir.X, dir.Z);
    }
    
    // Implementation of ICamera methods
    
    public void Orbit(float yawDeltaDegrees, float pitchDeltaDegrees)
    {
        _yaw += MathHelper.DegreesToRadians(yawDeltaDegrees);
        _pitch += MathHelper.DegreesToRadians(pitchDeltaDegrees);
        // Clamp pitch to prevent gimbal lock (looking straight up or down)
        _pitch = MathHelper.ClampRadians(_pitch);
        UpdatePositionFromSpherical();
    }

    public void Pan(Vector3 delta)
    {
        // Simple pan in camera local space (right, up)
        // Note: The cross product ensures 'right' is always perpendicular to 'Up' and 'Forward'
        var right = Vector3.Normalize(Vector3.Cross((Target - Position), Up));
        var up = Up;
        
        // Move Target and Position by the same delta
        Target += right * delta.X + up * delta.Y;
        Position += right * delta.X + up * delta.Y;
    }

    public void Zoom(float delta)
    {
        // Decrease distance, ensuring it doesn't drop below a minimum threshold
        _distance = MathF.Max(0.1f, _distance - delta);
        UpdatePositionFromSpherical();
    }

    public static ICamera CreateDefault()
    {
        return new Camera(new Vector3(5, 5, 5), new Vector3(0, 0, 0), Vector3.UnitY);
    }

    // Private helper method to calculate new Position based on Target and spherical coordinates
    private void UpdatePositionFromSpherical()
    {
        // Convert spherical (distance, pitch, yaw) coordinates relative to Target back to Cartesian (x, y, z)
        var x = _distance * MathF.Cos(_pitch) * MathF.Sin(_yaw);
        var y = _distance * MathF.Sin(_pitch);
        var z = _distance * MathF.Cos(_pitch) * MathF.Cos(_yaw);
        
        Position = Target + new Vector3(x, y, z);
    }


}