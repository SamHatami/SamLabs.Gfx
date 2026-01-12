using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Utility;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Systems.Tools.Transform.Strategies;

public class RotateToolStrategy:ITransformToolStrategy
{
    private Vector3 _lastHitPoint = Vector3.Zero;
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityQueryService _query;

    public RotateToolStrategy(IComponentRegistry componentRegistry, EntityQueryService query)
    {
        _componentRegistry = componentRegistry;
        _query = query;
    }

    public void Apply(FrameInput input, ref TransformComponent target, ref TransformComponent manipulatorTransform,
        ManipulatorChildComponent manipulatorChild, bool isGlobalMode = true)
    {

        // 1. Get the rotation amount (angle) and the axis from input
        float angle = GetRotateDelta(input, manipulatorTransform,  manipulatorChild, true); 
        Vector3 axis = manipulatorChild.Axis.ToVector3(); // e.g., Vector3.UnitX

        // 2. Create the Rotation Matrix for this frame's change
        Matrix4 rotationDelta = Matrix4.CreateFromAxisAngle(axis, angle);

        if (isGlobalMode)
        {
            // --- GLOBAL ROTATION ---
            // We want to rotate the object's ORIENTATION around the World Axes,
            // but keep it at the same POSITION.
            
            // A. Extract Position
            Vector3 pos = target.LocalMatrix.ExtractTranslation();
            
            // B. Remove Position (Move to Origin)
            target.LocalMatrix.Row3 = Vector4.UnitW; 
            
            // C. Apply Rotation (Pre-Multiply = Global Frame)
            target.LocalMatrix *= rotationDelta;
            
            // D. Restore Position
            target.LocalMatrix.Row3 = new Vector4(pos, 1.0f);
        }
        else
        {
            // --- LOCAL ROTATION ---
            // We want to rotate around the object's EXISTING axes.
            
            // Apply Rotation (Post-Multiply = Local Frame)
            // This appends the rotation to whatever rotation/scale already exists.
            target.LocalMatrix = rotationDelta * target.LocalMatrix;
        }

        target.IsDirty = true;
    }

    public void Reset()
    {
        _lastHitPoint = Vector3.Zero;
    }

    private float GetRotateDelta(FrameInput input, TransformComponent manipulatorTransform, ManipulatorChildComponent manipulatorChild, bool constrainDelta = false)
    {
        var cameraId = _query.First(_query.With<CameraComponent>());
        if (cameraId == -1) return 0;

        //Get cameraData (still only one camera)
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraId);
        ref var cameraTransform = ref _componentRegistry.GetComponent<TransformComponent>(cameraId);
        var cameraDir = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        
        //Cast ray from camera to plane perpendicualar to camera foward direction, with origin at manipulator position
        var mouseRay =
            cameraData.ScreenPointToWorldRay(
                new Vector2((float)input.MousePosition.X, (float)input.MousePosition.Y),
                input.ViewportSize);
        
        var projectionPlane = new Plane(manipulatorTransform.Position,cameraDir);

        if (!projectionPlane.RayCast(mouseRay, out var hit))
            return 0f;

        var currentHitPoint = mouseRay.GetPoint(hit);
        if (_lastHitPoint == Vector3.Zero)
        {
            _lastHitPoint = currentHitPoint;
            return 0f;
        }

        //A bit of claude magic
        var axisVector = manipulatorChild.Axis.ToVector3();
        var manipulatorPos = manipulatorTransform.Position; 

        var lastDir = Vector3.Normalize(_lastHitPoint - manipulatorPos);
        var currentDir = Vector3.Normalize(currentHitPoint - manipulatorPos);

        var cross = Vector3.Cross(lastDir, currentDir);
        var sign = MathF.Sign(Vector3.Dot(cross, axisVector));
        var angle = MathF.Acos(Math.Clamp(Vector3.Dot(lastDir, currentDir), -1, 1));

        _lastHitPoint = currentHitPoint;
        return angle * sign;
    }

}