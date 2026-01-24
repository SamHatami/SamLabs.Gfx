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
            barTransform.Rotation = CalculateRotationFromDirection(direction);
            barTransform.Scale = new Vector3(barTransform.Scale.X, barTransform.Scale.Y, length);
            barTransform.IsDirty = true;

            // Nodes look at each other
            startTransform.Rotation = CalculateRotationFromDirection(direction);
            startTransform.IsDirty = true;

            endTransform.Rotation = CalculateRotationFromDirection(-direction);
            endTransform.IsDirty = true;
        }
    }
    
    private Quaternion CalculateRotationFromDirection(Vector3 direction)
    {
        // Bar length axis is always Z
        var dot = Vector3.Dot(direction, Vector3.UnitZ);
        
        if (dot > 0.9999999f)
            return Quaternion.Identity;
        
        if (dot < -0.9999999f)
            return Quaternion.FromAxisAngle(Vector3.UnitX, MathF.PI);
        
        var axis = Vector3.Cross(Vector3.UnitZ, direction);
        
        // if (axis.LengthSquared < 0.0001f)
        //     return Quaternion.Identity;
        
        axis = Vector3.Normalize(axis);
        var angle = MathF.Acos(MathExtensions.Clamp(dot, -1.0f, 1.0f));

        return Quaternion.FromAxisAngle(axis, angle);
    }
}