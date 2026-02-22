using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Transform;

namespace SamLabs.Gfx.Engine.Systems.Structural;

public static class TrussNodeUtility
{
    public static bool CanMergeNodes(TrussNodeComponent targetNode, TrussNodeComponent nodeToMerge)
    {
        foreach (var elementId in targetNode.ConnectedMemberIds)
            if (nodeToMerge.ConnectedMemberIds.Contains(elementId))
                return false;
        return true;
    }

    public static void UpdateConnectedMembers(IComponentRegistry componentRegistry, TrussNodeComponent nodeComponent)
    {
        foreach (var memberEntityId in nodeComponent.ConnectedMemberIds)
            UpdateMemberTransform(componentRegistry, memberEntityId);
    }

    public static void UpdateMemberTransform(IComponentRegistry componentRegistry, int memberId)
    {
        ref var memberComponent = ref componentRegistry.GetComponent<TrussMemberComponent>(memberId);
        var startNodeId = memberComponent.StartNodeEntityId;
        var endNodeId = memberComponent.EndNodeEntityId;

        var startTransform = componentRegistry.GetComponent<TransformComponent>(startNodeId);
        var endTransform = componentRegistry.GetComponent<TransformComponent>(endNodeId);

        var delta = endTransform.Position - startTransform.Position;
        var length = delta.Length;
        if (length <= 1e-6f)
            return;

        var direction = delta / length;
        var memberPosition = startTransform.Position + direction * (length / 2.0f);

        ref var memberTransform = ref componentRegistry.GetComponent<TransformComponent>(memberId);
        memberTransform.Position = memberPosition;
        memberTransform.Rotation = CalculateRotationFromDirection(direction);
        memberTransform.Scale = new Vector3(memberTransform.Scale.X, memberTransform.Scale.Y, length);
        memberTransform.WorldMatrix = memberTransform.LocalMatrix;
        memberComponent.Length = length;
    }

    public static Quaternion CalculateRotationFromDirection(Vector3 direction)
    {
        var dot = Vector3.Dot(direction, Vector3.UnitZ);
        return dot switch
        {
            > 0.9999999f => Quaternion.Identity,
            < -0.9999999f => Quaternion.FromAxisAngle(Vector3.UnitX, MathF.PI),
            _ => Quaternion.FromAxisAngle(
                Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, direction)),
                MathF.Acos(Math.Clamp(dot, -1.0f, 1.0f)))
        };
    }
}
