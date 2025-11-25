using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class CameraControlSystem : UpdateSystem
{
    public CameraControlSystem(ComponentManager componentManager) : base(componentManager)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var cameraEntity = _componentManager.GetEntityIdsFor<CameraComponent>();
        var cameraData =
            _componentManager.TryGetComponentForEntity<CameraDataComponent>(cameraEntity[0]) is CameraDataComponent
                cameraDataComponent
                ? cameraDataComponent
                : default;

        if (frameInput.IsMouseLeftButtonDown)
            Orbit(frameInput.DeltaMouseMove, cameraData);

        else if (frameInput.IsMouseRightButtonDown)
            Pan(frameInput.DeltaMouseMove, cameraData);

        else if (frameInput.MouseWheelDelta != 0.0f)
            Zoom(frameInput.MouseWheelDelta, cameraData);
        
        
    }

    public void Pan(Vector2 delta, CameraDataComponent cameraData)
    {
        var right = Vector3.Normalize(Vector3.Cross(cameraData.Target - cameraData.Position, cameraData.Up));
        var up = cameraData.Up;
        var moveScale = 0.01f;
        cameraData.Target += right * delta.X * moveScale + up * delta.Y * moveScale;
        cameraData.Position += right * delta.X * moveScale + up * delta.Y * moveScale;
    }

    public void Orbit(Vector2 delta, CameraDataComponent cameraData)
    {
        var yawDeltaDegrees = delta.X * 0.2f;
        var pitchDeltaDegrees = delta.Y * 0.2f;

        cameraData.Yaw += MathHelper.DegreesToRadians(yawDeltaDegrees);
        cameraData.Pitch += MathHelper.DegreesToRadians(pitchDeltaDegrees);
        // Clamp pitch to prevent gimbal lock (looking straight up or down)
        cameraData.Pitch = MathHelper.ClampRadians(cameraData.Pitch);
        UpdatePositionFromSpherical(cameraData);
    }

    public void Zoom(float delta, CameraDataComponent cameraData)
    {
        // Decrease distance, ensuring it doesn't drop below a minimum threshold
        cameraData.DistanceToTarget = MathF.Max(0.1f, cameraData.DistanceToTarget - delta);
        UpdatePositionFromSpherical(cameraData);
    }

    private void UpdatePositionFromSpherical(CameraDataComponent cameraData)
    {
        var x = cameraData.DistanceToTarget * MathF.Cos(cameraData.Pitch) * MathF.Sin(cameraData.Yaw);
        var y = cameraData.DistanceToTarget * MathF.Sin(cameraData.Pitch);
        var z = cameraData.DistanceToTarget * MathF.Cos(cameraData.Pitch) * MathF.Cos(cameraData.Yaw);

        cameraData.Position = cameraData.Target + new Vector3(x, y, z);
    }
}