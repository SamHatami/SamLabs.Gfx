using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Utility;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;

public class ScaleStrategy : ITransformStrategy
{
    private Vector3 _lastHitPoint = Vector3.Zero;

    public void Apply(FrameInput input, ref TransformComponent target, ref TransformComponent gizmoTransform,
        GizmoChildComponent gizmoChild, bool isGlobalMode = true)
    {
        var delta = GetTransformDelta(input, gizmoTransform, gizmoChild);
        var scaleFactor = Vector3.One + delta;
        var scaleMatrix = Matrix4.CreateScale(scaleFactor);

        if (isGlobalMode)
        {
            // --- GLOBAL MODE (Distortion) ---
            // Logic: Rotate(Old) -> Scale(World) -> Translate(Old)
        
            // A. Extract current position to preserve it
            var pos = target.LocalMatrix.ExtractTranslation();

            // B. Apply Scale in World Space (Post-Multiply logic)
            // 1. target.LocalMatrix:        Apply existing Scale/Rotation/Translation
            // 2. CreateTranslation(-pos):   Move Pivot to World Origin (0,0,0)
            // 3. scaleMatrix:               Apply World Scale (Shears if object is rotated)
            // 4. CreateTranslation(pos):    Move Pivot back to original spot
        
            target.LocalMatrix = target.LocalMatrix * Matrix4.CreateTranslation(-pos) * scaleMatrix * Matrix4.CreateTranslation(pos);
        }
        else
        {
            // --- LOCAL MODE (Standard) ---
            // Logic: Scale(Local) -> Rotate(Old) -> Translate(Old)
        
            // In OpenTK (Row-Major), Pre-Multiplying applies the transform "First" (conceptually local).
            // Since the ScaleMatrix is centered and diagonal, this scales along the object's 
            // internal axes without affecting the Position.
        
            target.LocalMatrix = scaleMatrix * target.LocalMatrix;
        }

        // Mark dirty so the system recalculates WorldMatrix for the Renderer
        target.IsDirty = true;
        
    }
    public void Reset()
    {
        _lastHitPoint = Vector3.Zero;
    }

    private Vector3 GetTransformDelta(FrameInput input, TransformComponent gizmoTransform,
        GizmoChildComponent gizmoChild)
    {
        var cameraId = GetEntityIds.With<CameraComponent>().First();
        if (cameraId == -1) return Vector3.Zero;

        //Get cameraData (still only one camera)
        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraId);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraId);
        var cameraDir = Vector3.Normalize(cameraData.Target - cameraTransform.Position);

        //Cast ray from camera to plane perpendicualar to camera foward direction, with origin at gizmo position
        var mouseRay =
            cameraData.ScreenPointToWorldRay(
                new Vector2((float)input.MousePosition.X, (float)input.MousePosition.Y),
                input.ViewportSize);

        var projectionPlane = new Plane(gizmoTransform.Position, cameraDir);

        if (!projectionPlane.RayCast(mouseRay, out var hit))
            return Vector3.Zero;

        var currentHitPoint = mouseRay.GetPoint(hit);
        var directionFromCenter = Vector3.Normalize(currentHitPoint - gizmoTransform.Position);
        if (_lastHitPoint == Vector3.Zero)
        {
            _lastHitPoint = currentHitPoint;
            return Vector3.Zero;
        }

        //Filter results and return
        var delta = currentHitPoint - _lastHitPoint;
        var transformDelta = ConstrainedTransform(delta, directionFromCenter,gizmoChild.Axis);
        _lastHitPoint = currentHitPoint;
        return transformDelta;
    }

    private Vector3 ConstrainedTransform(Vector3 delta, Vector3 directionFromCenter, GizmoAxis axis)
    {
        float dragDirection = MathF.Sign(Vector3.Dot(delta, directionFromCenter));
        return axis switch
        {
            GizmoAxis.X => new Vector3(delta.X, 0, 0),
            GizmoAxis.Y => new Vector3(0, delta.Y, 0),
            GizmoAxis.Z => new Vector3(0, 0, delta.Z),

            GizmoAxis.XY => CreatePlaneScale(delta.X, delta.Y, 0, dragDirection),
            GizmoAxis.XZ => CreatePlaneScale(delta.X, 0, delta.Z, dragDirection),
            GizmoAxis.YZ => CreatePlaneScale(0, delta.Y, delta.Z, dragDirection),

            _ => Vector3.One
        };
    }

    private static Vector3 CreatePlaneScale(float a, float b, float c, float sign)
    {
        var mag = MathF.Sqrt(a * a + b * b + c * c) * sign;
        return new Vector3(
            a != 0 ? mag : 0, 
            b != 0 ? mag : 0, 
            c != 0 ? mag : 0);
    }
}