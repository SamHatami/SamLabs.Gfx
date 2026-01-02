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
        GizmoChildComponent gizmoChild)
    {
        var delta = GetTransformDelta(input, gizmoTransform,  gizmoChild, true).Length;
        var rotationSpeed = 1f;
        target.Rotation *= Quaternion.FromAxisAngle(gizmoChild.Axis.ToVector3(),  delta);
    }

    public void Reset()
    {
        _lastHitPoint = Vector3.Zero;
    }

    private Vector3 GetTransformDelta(FrameInput input, TransformComponent gizmoTransform, GizmoChildComponent gizmoChild, bool constrainDelta = false)
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
        _lastHitPoint = currentHitPoint;
        return delta;
    }

}