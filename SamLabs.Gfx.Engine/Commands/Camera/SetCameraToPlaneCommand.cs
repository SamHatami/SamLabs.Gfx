using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands.Internal;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Transform;

namespace SamLabs.Gfx.Engine.Commands.Camera;

public class SetCameraToPlaneCommand : InternalCommand
{
    private readonly IComponentRegistry _componentRegistry;
    private readonly Vector3 _planeOrigin;
    private readonly Vector3 _planeNormal;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;
    private ProjectionType _previousProjection;
    private Vector3 _previousUp;

    public SetCameraToPlaneCommand(IComponentRegistry componentRegistry, Vector3 planeOrigin, Vector3 planeNormal)
    {
        _componentRegistry = componentRegistry;
        _planeOrigin = planeOrigin;
        _planeNormal = planeNormal;
    }

    public override void Execute()
    {
        var cameraEntities = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        var cameraEntityId = cameraEntities[0];
        
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraEntityId);
        ref var cameraTransform = ref _componentRegistry.GetComponent<TransformComponent>(cameraEntityId);
        
        _previousPosition = cameraTransform.Position;
        _previousRotation = cameraTransform.Rotation;
        _previousProjection = cameraData.ProjectionType;
        _previousUp = cameraData.Up;

        cameraData.ProjectionType = ProjectionType.Orthographic;

        float distanceFromPlane = 50f;
        var cameraPosition = _planeOrigin - (_planeNormal * distanceFromPlane);
        cameraTransform.Position = cameraPosition;

        var targetLookAt = _planeOrigin; // Look at plane origin
        var forwardDir = Vector3.Normalize(targetLookAt - cameraPosition);
        
        var upVector = Vector3.UnitY;
        if (Math.Abs(Vector3.Dot(forwardDir, upVector)) > 0.9f)
        {
            upVector = Vector3.UnitZ;
        }

        var rightVector = Vector3.Normalize(Vector3.Cross(upVector, forwardDir));
        var actualUpVector = Vector3.Cross(forwardDir, rightVector);
        
        cameraTransform.Rotation = QuaternionFromMatrix(rightVector, actualUpVector, -forwardDir);
        cameraData.Up = actualUpVector;
        cameraData.Target = targetLookAt;
    }

    public override void Undo()
    {
        var cameraEntities = _componentRegistry.GetEntityIdsForComponentType<CameraComponent>();
        if (cameraEntities.Length == 0) return;

        var cameraEntityId = cameraEntities[0];
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraEntityId);
        ref var cameraTransform = ref _componentRegistry.GetComponent<TransformComponent>(cameraEntityId);
        
        cameraTransform.Position = _previousPosition;
        cameraTransform.Rotation = _previousRotation;
        cameraData.ProjectionType = _previousProjection;
        cameraData.Up = _previousUp;
    }

    private Quaternion QuaternionFromMatrix(Vector3 right, Vector3 up, Vector3 forward)
    {
        float trace = right.X + up.Y + forward.Z;

        if (trace > 0)
        {
            float s = 0.5f / MathF.Sqrt(trace + 1.0f);
            return new Quaternion(
                (up.Z - forward.Y) * s,
                (forward.X - right.Z) * s,
                (right.Y - up.X) * s,
                0.25f / s
            );
        }

        if ((right.X > up.Y) && (right.X > forward.Z))
        {
            float s = 2.0f * MathF.Sqrt(1.0f + right.X - up.Y - forward.Z);
            return new Quaternion(
                0.25f * s,
                (up.X + right.Y) / s,
                (forward.X + right.Z) / s,
                (up.Z - forward.Y) / s
            );
        }

        if (up.Y > forward.Z)
        {
            float s = 2.0f * MathF.Sqrt(1.0f + up.Y - right.X - forward.Z);
            return new Quaternion(
                (up.X + right.Y) / s,
                0.25f * s,
                (forward.Y + up.Z) / s,
                (forward.X - right.Z) / s
            );
        }

        float s2 = 2.0f * MathF.Sqrt(1.0f + forward.Z - right.X - up.Y);
        return new Quaternion(
            (forward.X + right.Z) / s2,
            (forward.Y + up.Z) / s2,
            0.25f * s2,
            (right.Y - up.X) / s2
        );
    }
}



