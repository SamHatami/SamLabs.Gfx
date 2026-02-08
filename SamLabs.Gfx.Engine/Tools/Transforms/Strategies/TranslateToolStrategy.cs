using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Utility;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Tools.Transforms.Strategies;

public class TranslateToolStrategy : ITransformToolStrategy
{
    private Vector3 _lastHitPoint = Vector3.Zero;
    private readonly IComponentRegistry _componentRegistry;
    private readonly EntityRegistry _entityRegistry;

    public TranslateToolStrategy(IComponentRegistry componentRegistry, EntityRegistry entityRegistry)
    {
        _componentRegistry = componentRegistry;
        _entityRegistry = entityRegistry;
    }

    public void Apply(FrameInput input, ref TransformComponent target, ref TransformComponent manipulatorTransform,
        ManipulatorChildComponent manipulatorChild, bool isGlobalMode = true)
    {
        var delta = GetTransformDelta(input, manipulatorTransform, manipulatorChild);
        var translationMatrix = Matrix4.CreateTranslation(delta);

        // 3. Apply it
        // Post-Multiplication (Matrix * Translation) simply adds the translation 
        // to the existing position, regardless of rotation or scale.
        target.LocalMatrix *= translationMatrix;

        // 4. Mark Dirty
        target.IsDirty = true;
        manipulatorTransform.Position = target.Position;
    }

    public void Reset()
    {
        _lastHitPoint = Vector3.Zero;
    }

    private Vector3 GetTransformDelta(FrameInput frameInput, TransformComponent manipulatorTransform,
        ManipulatorChildComponent manipulatorChild)
    {
        var cameraId = _entityRegistry.Query.With<CameraComponent>().First();
        if (cameraId == -1) return Vector3.Zero;

        //Get cameraData (still only one camera)
        ref var cameraData = ref _componentRegistry.GetComponent<CameraDataComponent>(cameraId);
        ref var cameraTransform = ref _componentRegistry.GetComponent<TransformComponent>(cameraId);
        var cameraDir = Vector3.Normalize(cameraData.Target - cameraTransform.Position);

        //Cast ray from camera to plane perpendicualar to camera foward direction, with origin at manipulator position
        var mouseRay =
            cameraData.ScreenPointToWorldRay(
                new Vector2((float)frameInput.MousePosition.X, (float)frameInput.MousePosition.Y),
                frameInput.ViewportSize);

        var projectionPlane = new Plane(manipulatorTransform.Position, cameraDir);

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
        var transformDelta = ConstrainedTransform(delta, manipulatorChild.Axis);
        _lastHitPoint = currentHitPoint;
        return transformDelta;
    }

    private Vector3 ConstrainedTransform(Vector3 transformDelta, ManipulatorAxis axis)
    {
        return axis switch
        {
            // Single axis: project delta onto axis
            ManipulatorAxis.X => new Vector3(transformDelta.X, 0, 0),
            ManipulatorAxis.Y => new Vector3(0, transformDelta.Y, 0),
            ManipulatorAxis.Z => new Vector3(0, 0, transformDelta.Z),

            // Plane handles: allow movement in both axes
            ManipulatorAxis.XY => new Vector3(transformDelta.X, transformDelta.Y, 0),
            ManipulatorAxis.XZ => new Vector3(transformDelta.X, 0, transformDelta.Z),
            ManipulatorAxis.YZ => new Vector3(0, transformDelta.Y, transformDelta.Z),

            _ => Vector3.Zero
        };
    }
}