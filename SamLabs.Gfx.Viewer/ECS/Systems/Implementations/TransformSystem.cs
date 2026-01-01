using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Utility;
using Plane = SamLabs.Gfx.Geometry.Plane;
using Ray = SamLabs.Gfx.Geometry.Ray;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class TransformSystem : UpdateSystem
{
    public override int SystemPosition => SystemOrders.TransformUpdate;
    private bool _isTransforming;
    private int _selectedGizmoSubEntity;
    private Vector3 _lastHitPoint;

    public TransformSystem(EntityManager entityManager) : base(entityManager)
    {
        _lastHitPoint = Vector3.Zero;
    }

    public override void Update(FrameInput frameInput)
    {
        var activeGizmo = GetEntitiesIds.With<ActiveGizmoComponent>().First();
        if (activeGizmo == -1) return;

        var selectedEntities = GetEntitiesIds.With<SelectedComponent>().AndWith<TransformComponent>()
            .Without<GizmoComponent>().Without<GizmoChildComponent>();

        if (selectedEntities.IsEmpty) return;

        ref var entityTransform = ref ComponentManager.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var gizmoTransform = ref ComponentManager.GetComponent<TransformComponent>(activeGizmo);
        var gizmoComponent = ComponentManager.GetComponent<GizmoComponent>(activeGizmo);
        
        gizmoTransform.Position = entityTransform.Position;
        var pickingEntities = GetEntitiesIds.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;

        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (ComponentManager.HasComponent<GizmoChildComponent>(pickingData.HoveredEntityId))
            {
                _isTransforming = true;
                _selectedGizmoSubEntity = pickingData.HoveredEntityId;
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            ref var gizmoChild = ref ComponentManager.GetComponent<GizmoChildComponent>(_selectedGizmoSubEntity);
            
            switch (gizmoComponent.Type)
            {
                case GizmoType.Translate:
                    Translate(frameInput, ref gizmoTransform, ref entityTransform, gizmoChild);
                    break;
                case GizmoType.Scale:
                    Scale(frameInput, gizmoTransform, ref entityTransform, gizmoChild);
                    break;
                case GizmoType.Rotate:
                    Rotate(frameInput, gizmoTransform, ref entityTransform, gizmoChild);
                    break;
            }
        }

        if (frameInput.IsMouseLeftButtonDown || !_isTransforming) return;

        _isTransforming = false;
        _selectedGizmoSubEntity = -1;
        _lastHitPoint = Vector3.Zero;
    }

    private void Rotate(FrameInput frameInput, TransformComponent gizmoTransform, ref TransformComponent entityTransform,
        GizmoChildComponent gizmoChild)
    {
        var delta = GetTransformDelta(frameInput, gizmoTransform,  gizmoChild, true).Length;
        var rotationSpeed = 1f;
        entityTransform.Rotation *= Quaternion.FromAxisAngle(gizmoChild.Axis.ToVector3(),  delta);
    }

    private void Scale(FrameInput frameInput, TransformComponent gizmoTransform, ref TransformComponent entityTransform,
        GizmoChildComponent gizmoChild)
    {
        var delta = GetTransformDelta(frameInput, gizmoTransform,  gizmoChild);
        entityTransform.Scale *= delta;
    }

    private void Translate(FrameInput frameInput, ref TransformComponent gizmoTransform, ref TransformComponent entityTransform,
        GizmoChildComponent gizmoChild)
    {
        var delta = GetTransformDelta(frameInput, gizmoTransform,  gizmoChild);
        entityTransform.Position += delta;
        gizmoTransform.Position = entityTransform.Position;
    }

    private void UpdateChildrenWorldTransforms(ReadOnlySpan<int> childEntities, TransformComponent parentTransform)
    {
        foreach (var childId in childEntities)
        {
            ref var childTransform = ref ComponentManager.GetComponent<TransformComponent>(childId);
            childTransform.Position = parentTransform.Position + childTransform.LocalPosition;
            childTransform.Rotation = parentTransform.Rotation * childTransform.LocalRotation;
            childTransform.Scale = parentTransform.Scale * childTransform.LocalScale;
        }
    }

    private Vector3 GetTransformDelta(FrameInput frameInput, TransformComponent gizmoTransform, GizmoChildComponent gizmoChild, bool constrainDelta = false)
    {
        var cameraEntities = GetEntitiesIds.With<CameraComponent>();
        if (cameraEntities.IsEmpty) return Vector3.Zero;

        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);

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

        var delta = currentHitPoint - _lastHitPoint;
        var transformDelta = constrainDelta ? delta : ConstrainedTransform(delta, gizmoChild.Axis);
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