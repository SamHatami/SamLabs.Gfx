using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Utility;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Transform.Strategies;

public class TranslateStrategy:ITransformStrategy
{
    private Vector3 _lastHitPoint = Vector3.Zero;

    public void Apply(FrameInput input, ref TransformComponent target, ref TransformComponent gizmoTransform,
        GizmoChildComponent gizmoChild)
    {
        var delta = GetTransformDelta(input, gizmoTransform,  gizmoChild);
        target.Position += delta;
        gizmoTransform.Position = target.Position;
    }

    public void Reset()
    {
        _lastHitPoint = Vector3.Zero;
    }

    private Vector3 GetTransformDelta(FrameInput frameInput, TransformComponent gizmoTransform, GizmoChildComponent gizmoChild)
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
                new Vector2((float)frameInput.MousePosition.X, (float)frameInput.MousePosition.Y),
                frameInput.ViewportSize);
        
        var projectionPlane = new Plane(gizmoTransform.Position,cameraDir);

        if (!projectionPlane.RayCast(mouseRay, out var hit))
            return Vector3.Zero;

        var currentHitPoint = mouseRay.GetPoint(hit);
    
        if (_lastHitPoint == Vector3.Zero)
        {
            _lastHitPoint = currentHitPoint;
            return Vector3.Zero; 
        }
        //Filter results and return
        var delta = currentHitPoint - _lastHitPoint;
        var transformDelta = ConstrainedTransform(delta, gizmoChild.Axis);
        _lastHitPoint = currentHitPoint;
        return transformDelta;
    }

    private Vector3 ConstrainedTransform(Vector3 transformDelta, GizmoAxis axis)
    {
        return axis switch
        {
            // Single axis: project delta onto axis
            GizmoAxis.X => new Vector3(transformDelta.X, 0, 0),
            GizmoAxis.Y => new Vector3(0, transformDelta.Y, 0),
            GizmoAxis.Z => new Vector3(0, 0, transformDelta.Z),

            // Plane handles: allow movement in both axes
            GizmoAxis.XY => new Vector3(transformDelta.X, transformDelta.Y, 0),
            GizmoAxis.XZ => new Vector3(transformDelta.X, 0, transformDelta.Z),
            GizmoAxis.YZ => new Vector3(0, transformDelta.Y, transformDelta.Z),

            _ => Vector3.Zero
        };
    }
}