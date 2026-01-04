using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Utility;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;

public class RotateStrategy:ITransformStrategy
{
    private Vector3 _lastHitPoint = Vector3.Zero;

    public void Apply(FrameInput input, ref TransformComponent target, ref TransformComponent gizmoTransform,
        GizmoChildComponent gizmoChild, bool isGlobalMode = true)
    {

        // 1. Get the rotation amount (angle) and the axis from input
        float angle = GetRotateDelta(input, gizmoTransform,  gizmoChild, true); 
        Vector3 axis = gizmoChild.Axis.ToVector3(); // e.g., Vector3.UnitX

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

    private float GetRotateDelta(FrameInput input, TransformComponent gizmoTransform, GizmoChildComponent gizmoChild, bool constrainDelta = false)
    {
        var cameraId = GetEntityIds.With<CameraComponent>().First();
        if (cameraId == -1) return 0;

        //Get cameraData (still only one camera)
        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraId);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraId);
        var cameraDir = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        
        //Cast ray from camera to plane perpendicualar to camera foward direction, with origin at gizmo position
        var mouseRay =
            cameraData.ScreenPointToWorldRay(
                new Vector2((float)input.MousePosition.X, (float)input.MousePosition.Y),
                input.ViewportSize);
        
        var projectionPlane = new Plane(gizmoTransform.Position,cameraDir);

        if (!projectionPlane.RayCast(mouseRay, out var hit))
            return 0f;

        var currentHitPoint = mouseRay.GetPoint(hit);
        if (_lastHitPoint == Vector3.Zero)
        {
            _lastHitPoint = currentHitPoint;
            return 0f;
        }

        //A bit of claude magic
        var axisVector = gizmoChild.Axis.ToVector3();
        var gizmoPos = gizmoTransform.Position; 

        var lastDir = Vector3.Normalize(_lastHitPoint - gizmoPos);
        var currentDir = Vector3.Normalize(currentHitPoint - gizmoPos);

        var cross = Vector3.Cross(lastDir, currentDir);
        var sign = MathF.Sign(Vector3.Dot(cross, axisVector));
        var angle = MathF.Acos(Math.Clamp(Vector3.Dot(lastDir, currentDir), -1, 1));

        _lastHitPoint = currentHitPoint;
        return angle * sign;
    }

}