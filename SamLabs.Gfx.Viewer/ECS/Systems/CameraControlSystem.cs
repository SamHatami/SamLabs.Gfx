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
        var cameraEntities = _componentManager.GetEntityIdsFor<CameraComponent>();

        if (cameraEntities.IsEmpty) return;

        for (var i = 0; i < cameraEntities.Length; i++)
        {
            ref var cameraData = ref _componentManager.GetComponent<CameraDataComponent>(cameraEntities[i]);
            ref var cameraTransform = ref _componentManager.GetComponent<TransformComponent>(cameraEntities[i]);

            if (frameInput.IsMouseLeftButtonDown)
                Orbit(frameInput.DeltaMouseMove, ref cameraData, ref cameraTransform);

            else if (frameInput.IsMouseRightButtonDown)
                Pan(frameInput.DeltaMouseMove, ref cameraData, ref cameraTransform);

            else if (frameInput.MouseWheelDelta != 0.0f)
                Zoom(frameInput.MouseWheelDelta, ref cameraData, ref cameraTransform);
        }
    }

    private void Pan(Vector2 delta, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var right = Vector3.Normalize(Vector3.Cross(cameraData.Target - cameraTransform.Position, cameraData.Up));
        var up = cameraData.Up;
        var moveScale = 0.01f;
        cameraData.Target += right * delta.X * moveScale + up * delta.Y * moveScale;
        cameraTransform.Position += right * delta.X * moveScale + up * delta.Y * moveScale;
    }

    private void Orbit(Vector2 delta, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var yawDeltaDegrees = delta.X * 0.2f;
        var pitchDeltaDegrees = delta.Y * 0.2f;

        cameraData.Pitch += pitchDeltaDegrees;
        var rotation = cameraTransform.Rotation;
        rotation.Y += MathHelper.DegreesToRadians(yawDeltaDegrees);
        rotation.X += MathHelper.DegreesToRadians(pitchDeltaDegrees);
        // Clamp pitch to prevent gimbal lock (looking straight up or down)
        rotation.X = MathHelper.ClampRadians(rotation.X);
        cameraTransform.Rotation = rotation;

        UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
    }

    private void Zoom(float delta, ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        cameraData.DistanceToTarget = MathF.Max(0.1f, cameraData.DistanceToTarget - delta);
        UpdatePositionFromSpherical(ref cameraData, ref cameraTransform);
    }

    private void UpdatePositionFromSpherical(ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var x = cameraData.DistanceToTarget * MathF.Cos(cameraTransform.Rotation.X) *
                MathF.Sin(cameraTransform.Rotation.Y);
        var y = cameraData.DistanceToTarget * MathF.Sin(cameraTransform.Rotation.X);
        var z = cameraData.DistanceToTarget * MathF.Cos(cameraTransform.Rotation.X) *
                MathF.Cos(cameraTransform.Rotation.Y);

        cameraTransform.Position = cameraData.Target + new Vector3(x, y, z);
    }
}