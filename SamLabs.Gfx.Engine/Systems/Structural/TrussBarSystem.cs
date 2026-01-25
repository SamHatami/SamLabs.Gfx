using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Math;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

public class TrussBarSystem:UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.TransformUpdate;

    private readonly Dictionary<int, Quaternion> _previousRotations = new();

    public TrussBarSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var barEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussBarComponent>();
        
        foreach (var barId in barEntities)
        {
            //TODO: The cylinder needs to be its own child of the bar entity. 
            //TODO: Selecting and trying to move the bar itself, should move the nodes move the nodes simultaneously, so they follow the bar.
            
            var bar = ComponentRegistry.GetComponent<TrussBarComponent>(barId);
            ref var barTransform = ref ComponentRegistry.GetComponent<TransformComponent>(barId);
            
            ref var startTransform = ref ComponentRegistry.GetComponent<TransformComponent>(bar.StartNodeEntityId);
            ref var endTransform = ref ComponentRegistry.GetComponent<TransformComponent>(bar.EndNodeEntityId);
            
            var direction = startTransform.Position - endTransform.Position; //the direction flips when the bar is rotated
            var length = direction.Length;

            if (length > 0.0001f)
                direction = Vector3.Normalize(direction);
            
            barTransform.Position = startTransform.Position;
            barTransform.Rotation = CalculateRotationFromDirection(direction, barId);
            barTransform.Scale = new Vector3(barTransform.Scale.X, barTransform.Scale.Y, length);
            barTransform.IsDirty = true;

            // // Nodes look at each other
            // startTransform.Rotation = CalculateRotationFromDirection(direction, bar.StartNodeEntityId);
            // startTransform.IsDirty = true;
            //
            // endTransform.Rotation = CalculateRotationFromDirection(-direction, bar.EndNodeEntityId);
            // endTransform.IsDirty = true;
        }
    }
    
    private Quaternion CalculateRotationFromDirection(Vector3 direction, int entityId)
    {
        var dot = Vector3.Dot(direction, Vector3.UnitZ);

        Quaternion newRotation;

        if (dot > 0.9999999f)
        {
            newRotation = Quaternion.Identity;
        }
        else if (dot < -0.9999999f)
        {
            newRotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathF.PI);
        }
        else
        {
            var axis = Vector3.Cross(Vector3.UnitZ, direction);
            axis = Vector3.Normalize(axis);
            var angle = MathF.Acos(MathExtensions.Clamp(dot, -1.0f, 1.0f));
            newRotation = Quaternion.FromAxisAngle(axis, angle);
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
}