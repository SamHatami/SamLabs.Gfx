using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;

namespace SamLabs.Gfx.Engine.Systems.Structural;

public static class TrussNodeUtility
{
    public static bool CanMergeNodes(TrussNodeComponent targetNode, TrussNodeComponent nodeToMerge)
    {
        foreach (var elementId in targetNode.ConnectedBarIds)
            if (nodeToMerge.ConnectedBarIds.Contains(elementId))
                return false;
        return true;
    }
    
    public static void UpdateConnectedBars(IComponentRegistry componentRegistry, TrussNodeComponent nodeComponent, int nodeEntityId)
    {
        foreach (var barEntityId in nodeComponent.ConnectedBarIds)
        {
            UpdateBarTransform(componentRegistry, barEntityId);
        }
    }

    public static void UpdateBarTransform(IComponentRegistry componentRegistry, int barId)
    {
        ref var barComponent = ref componentRegistry.GetComponent<TrussBarComponent>(barId);
        var startNodeId = barComponent.StartNodeEntityId;
        var endNodeId = barComponent.EndNodeEntityId;
        
        var startTransform = componentRegistry.GetComponent<TransformComponent>(startNodeId);
        var endTransform = componentRegistry.GetComponent<TransformComponent>(endNodeId);
        
        var direction = Vector3.Normalize(endTransform.Position - startTransform.Position);
        var length = Vector3.Distance(startTransform.Position, endTransform.Position);
        var barPosition = startTransform.Position + direction * (length / 2.0f);

        ref var barTransform = ref componentRegistry.GetComponent<TransformComponent>(barId);
        barTransform.Position = barPosition;
        barTransform.Rotation = CalculateRotationFromDirection(direction);
        barTransform.Scale = new Vector3(barTransform.Scale.X, barTransform.Scale.Y, length);
        barTransform.WorldMatrix = barTransform.LocalMatrix;
        barComponent.Length = length;
    }

    public static Quaternion CalculateRotationFromDirection(Vector3 direction)
    {
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