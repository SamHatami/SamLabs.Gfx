using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Transform.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

/// <summary>
/// Controls the moving behavior of truss nodes, including merging and splitting nodes based on proximity and connection status.
/// Updates connected bars when nodes are moved.
/// </summary>
public class TrussNodeSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly Dictionary<int, Quaternion> _previousRotations = new();

    //Responsible for merging truss nodes that are within a certain threshold distance or splitting them
    //FLags required for this
    public TrussNodeSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        //We should get the selected nodes only, if we dont have any selected nodes we are not doing anything
        //Get nodes flagged with EntityTranslated
        var nodeEntities = _entityRegistry.Query.With<TrussNodeComponent>().With<TranslateChangedFlag>().Get();
        
        if(nodeEntities.IsEmpty()) return;         

        foreach (var nodeEntity in nodeEntities)
        {
            var nodeComponent = ComponentRegistry.GetComponent<TrussNodeComponent>(nodeEntity);
            UpdateConnectedBars()
        }
        //Split into two arrays, those that can be merged and those that cannot. Nodes that are connected to the same bar cannot be merged.  
    }

    private void UpdateConnectedBars(TrussNodeComponent nodeComponent, int nodeEntityId)
    {
        var transformComponent = ComponentRegistry.GetComponent<TransformComponent>(nodeEntityId);
        var nodePosition = transformComponent.Position;

        foreach (var barEntityId in nodeComponent.ConnectedBarIds)
        {
            var barComponent = ComponentRegistry.GetComponent<TrussBarComponent>(barEntityId);
            int otherNodeEntityId = barComponent.StartNodeEntityId == nodeEntityId
                ? barComponent.EndNodeEntityId
                : barComponent.StartNodeEntityId;

            var otherNodeTransform = ComponentRegistry.GetComponent<TransformComponent>(otherNodeEntityId);
            var otherNodePosition = otherNodeTransform.Position;

            var direction = Vector3.Normalize(otherNodePosition - nodePosition);
            var length = Vector3.Distance(nodePosition, otherNodePosition);

            // Update bar transform
            var barTransform = ComponentRegistry.GetComponent<TransformComponent>(barEntityId);
            barTransform.Position = nodePosition + direction * (length / 2.0f);
            barTransform.Rotation = CalculateRotationFromDirection(direction, barEntityId);
            barTransform.Scale = new Vector3(barTransform.Scale.X, barTransform.Scale.Y, length);

            ComponentRegistry.SetComponentToEntity(barTransform, barEntityId);

            // Update bar component length
            barComponent.Length = length;
            ComponentRegistry.SetComponentToEntity(barComponent, barEntityId);
        }
    }

    private Quaternion CalculateRotationFromDirection(Vector3 direction, int entityId)
    {
        var dot = Vector3.Dot(direction, Vector3.UnitZ);

        Quaternion newRotation;

        switch (dot)
        {
            case > 0.9999999f:
                newRotation = Quaternion.Identity;
                break;
            case < -0.9999999f:
                newRotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathF.PI);
                break;
            default:
            {
                var axis = Vector3.Cross(Vector3.UnitZ, direction);
                axis = Vector3.Normalize(axis);
                var angle = MathF.Acos(MathExtensions.Clamp(dot, -1.0f, 1.0f));
                newRotation = Quaternion.FromAxisAngle(axis, angle);
                break;
            }
        }

        if (_previousRotations.TryGetValue(entityId, out var previousRotation))
        {
            // Choose the rotation that has the shortest path from the previous rotation
            // Quaternion dot product: q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W
            var dotProduct = previousRotation.X * newRotation.X +
                             previousRotation.Y * newRotation.Y +
                             previousRotation.Z * newRotation.Z +
                             previousRotation.W * newRotation.W;

            // If the dot product is negative, the quaternions represent rotations more than 180 degrees apart
            // In this case, negate the new rotation to take the shorter path
            if (dotProduct < 0)
            {
                newRotation = new Quaternion(-newRotation.X, -newRotation.Y, -newRotation.Z, -newRotation.W);
            }
        }

        // Store this rotation for next frame
        _previousRotations[entityId] = newRotation;

        return newRotation;
    }

    //Move to NodeUtility
    private bool CanMergeNodes(TrussNodeComponent targetNode, TrussNodeComponent nodeToMerge)
    {
        foreach (var elementId in targetNode.ConnectedBarIds)
            if (nodeToMerge.ConnectedBarIds.Contains(elementId))
                return false;

        return true;
    }

    // private TrussNodeComponent FindNearestNode(TrussNodeComponent node, Vector3 proposedPosition, float tolerance)
    // {
    //     TrussNodeComponent nearestNode = default;
    //     var minDistance = tolerance;
    //
    //     var allNodes = ComponentRegistry.GetEntityIdsForComponentType<TrussNodeComponent>();
    //
    //     foreach (var otherNodeEntity in allNodes)
    //     {
    //         var otherNode = ComponentRegistry.GetComponent<TrussNodeComponent>(otherNodeEntity);
    //         if (otherNodeEntity == node) continue;
    //
    //         var otherTransform = ComponentRegistry.GetComponent<TransformComponent>(otherNodeEntity);
    //         var distance = Vector3.Distance(otherTransform.Position, proposedPosition);
    //         if (!(distance < minDistance)) continue;
    //
    //         minDistance = distance;
    //         nearestNode = otherNode;
    //     }
    //
    //     return nearestNode;
    // }
}