using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.SceneGraph;

public class CameraController
{
    private readonly Camera _camera;

    public CameraController(Camera camera)
    {
        _camera = camera;
    }
    public void Pan(Vector3 delta)
    {
        var right = Vector3.Normalize(Vector3.Cross(_camera.Target - _camera.Position, _camera.Up));
        var up = _camera.Up;

        _camera.Target += right * delta.X + up * delta.Y;
        _camera.Position += right * delta.X + up * delta.Y;
    }

    public void Orbit(float yawDeltaDegrees, float pitchDeltaDegrees)
    {
        _camera.Yaw += MathHelper.DegreesToRadians(yawDeltaDegrees);
        _camera.Pitch += MathHelper.DegreesToRadians(pitchDeltaDegrees);
        // Clamp pitch to prevent gimbal lock (looking straight up or down)
        _camera.Pitch = MathHelper.ClampRadians(_camera.Pitch);
        UpdatePositionFromSpherical();
    }

    public void Zoom(float delta)
    {
        // Decrease distance, ensuring it doesn't drop below a minimum threshold
        _camera.DistanceToTarget = MathF.Max(0.1f, _camera.DistanceToTarget - delta);
        UpdatePositionFromSpherical();
    }

    private void UpdatePositionFromSpherical()
    {
        var x = _camera.DistanceToTarget * MathF.Cos(_camera.Pitch) * MathF.Sin(_camera.Yaw);
        var y = _camera.DistanceToTarget * MathF.Sin(_camera.Pitch);
        var z = _camera.DistanceToTarget * MathF.Cos(_camera.Pitch) * MathF.Cos(_camera.Yaw);

        _camera.Position = _camera.Target + new Vector3(x, y, z);
    }
}