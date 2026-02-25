using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Transform;

namespace SamLabs.Gfx.Engine.Systems.Structural;

/// <summary>
/// Utility methods for frame geometry calculations and transformations.
/// </summary>
public static class FrameGeometryUtility
{
    /// <summary>
    /// Updates the transform of a frame member based on its connected nodes.
    /// </summary>
    public static void UpdateMemberTransform(IComponentRegistry componentRegistry, int memberId)
    {
        ref var memberComponent = ref componentRegistry.GetComponent<FrameMemberComponent>(memberId);
        var startNodeId = memberComponent.StartNodeEntityId;
        var endNodeId = memberComponent.EndNodeEntityId;
        
        var startTransform = componentRegistry.GetComponent<TransformComponent>(startNodeId);
        var endTransform = componentRegistry.GetComponent<TransformComponent>(endNodeId);
        
        var direction = Vector3.Normalize(endTransform.Position - startTransform.Position);
        var length = Vector3.Distance(startTransform.Position, endTransform.Position);
        var memberPosition = startTransform.Position + direction * (length / 2.0f);

        ref var memberTransform = ref componentRegistry.GetComponent<TransformComponent>(memberId);
        memberTransform.Position = memberPosition;
        memberTransform.Rotation = CalculateRotationFromDirection(direction);
        memberTransform.Scale = new Vector3(memberTransform.Scale.X, memberTransform.Scale.Y, length);
        memberTransform.WorldMatrix = memberTransform.LocalMatrix;
        memberComponent.Length = length;
    }

    /// <summary>
    /// Calculates a rotation quaternion from a direction vector.
    /// </summary>
    public static Quaternion CalculateRotationFromDirection(Vector3 direction)
    {
        if (direction.Z < 0 || (direction.Z == 0 && direction.X < 0))
            direction = -direction;
        
        var dot = Vector3.Dot(direction, Vector3.UnitZ);
        switch (dot)
        {
            case > 0.9999999f:
                return Quaternion.Identity;
            case < -0.9999999f:
                return Quaternion.FromAxisAngle(Vector3.UnitX, MathF.PI);
            default:
                var axis = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, direction));
                var angle = MathF.Acos(Math.Clamp(dot, -1.0f, 1.0f));
                return Quaternion.FromAxisAngle(axis, angle);
        }
    }
}
