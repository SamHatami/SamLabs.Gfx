using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;

namespace SamLabs.Gfx.Engine.Systems.Camera;

public static class CameraUtility
{
    public static void UpdatePositionFromSpherical(ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var heightDistance = cameraData.DistanceToTarget * MathF.Cos(cameraData.Pitch);
        var x = heightDistance * MathF.Sin(cameraData.Yaw);
        var y = cameraData.DistanceToTarget * MathF.Sin(cameraData.Pitch);
        var z = heightDistance * MathF.Cos(cameraData.Yaw);

        cameraTransform.Position = cameraData.Target + new Vector3(x, y, z);
    }

    public static void UpdateRotation(ref CameraDataComponent cameraData, ref TransformComponent cameraTransform)
    {
        var yawRotation = Quaternion.FromAxisAngle(Vector3.UnitY, cameraData.Yaw);
        var pitchRotation = Quaternion.FromAxisAngle(Vector3.UnitX, cameraData.Pitch);
        cameraTransform.Rotation = yawRotation * pitchRotation;
    }

    public static Vector3 CalculatePositionFromSpherical(Vector3 target, float distance, float pitch, float yaw)
    {
        var heightDistance = distance * MathF.Cos(pitch);
        var x = heightDistance * MathF.Sin(yaw);
        var y = distance * MathF.Sin(pitch);
        var z = heightDistance * MathF.Cos(yaw);

        return target + new Vector3(x, y, z);
    }
}
