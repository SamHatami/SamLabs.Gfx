using Assimp;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
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
    override public int SystemPosition => SystemOrders.TransformUpdate;
    private bool _isTransforming;
    private Vector3 _transformStartPoint;
    private int _selectedGizmoSubEntity;
    private TransformComponent _selectedGizmoTransform;

    public TransformSystem(EntityManager entityManager) : base(entityManager)
    {
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

        gizmoTransform.Position = entityTransform.Position;

        // Update child gizmo world transforms based on parent
        Span<int> childBuffer = stackalloc int[12];
        // var subGizmoEntities = ComponentManager.GetChildEntitiesForParent(activeGizmo, childBuffer);
        // // UpdateChildrenWorldTransforms(subGizmoEntities, gizmoTransform);

        var pickingEntities = GetEntitiesIds.With<PickingDataComponent>();
        if (pickingEntities.IsEmpty) return;

        ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(pickingEntities[0]);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            if (ComponentManager.HasComponent<GizmoChildComponent>(pickingData.HoveredEntityId))
            {
                _isTransforming = true;
                _selectedGizmoSubEntity = pickingData.HoveredEntityId;
                _transformStartPoint = entityTransform.Position;
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            var delta = CalculateTransformDelta(frameInput, gizmoTransform);
            entityTransform.Position += delta;
            gizmoTransform.Position = entityTransform.Position;
        }

        if (frameInput.IsMouseLeftButtonDown || !_isTransforming) return;

        _isTransforming = false;
        _selectedGizmoSubEntity = -1;
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

    private Vector3 CalculateTransformDelta(FrameInput frameInput, TransformComponent gizmoTransform)
    {
        // Get camera for ray casting
        var cameraEntities = GetEntitiesIds.With<CameraComponent>();
        if (cameraEntities.IsEmpty) return Vector3.Zero;

        ref var cameraData = ref ComponentManager.GetComponent<CameraDataComponent>(cameraEntities[0]);
        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        ref var gizmoChild = ref ComponentManager.GetComponent<GizmoChildComponent>(_selectedGizmoSubEntity);

        var cameraDir = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
        
        var mouseRay =
            cameraData.ScreenPointToWorldRay(
                new Vector2((float)frameInput.MousePosition.X, (float)frameInput.MousePosition.Y),
                frameInput.ViewportSize);

        var projectionPlane = new Plane(gizmoTransform.Position,cameraDir);

        if (!projectionPlane.RayCast(mouseRay, out var hit))
            return Vector3.Zero;

        return CalculateConstrainedDelta(mouseRay.GetPoint(hit), gizmoTransform.Position, gizmoChild.Axis);
    }

   private Vector3 CalculateConstrainedDelta(Vector3 currentPoint, Vector3 gizmoPosition,
        GizmoAxis axis)
    {
        var totalDelta = currentPoint - gizmoPosition;
        var totalDistance = Vector3.Distance(currentPoint, gizmoPosition);
        if(totalDistance < 0.00001f)
            return Vector3.Zero;
        return axis switch
        {
            // Single axis: project delta onto axis
            GizmoAxis.X => new Vector3(totalDelta.X, 0, 0),
            GizmoAxis.Y => new Vector3(0, totalDelta.Y, 0),
            GizmoAxis.Z => new Vector3(0, 0, totalDelta.Z),

            // Plane handles: allow movement in both axes
            GizmoAxis.XY => new Vector3(totalDelta.X, totalDelta.Y, 0),
            GizmoAxis.XZ => new Vector3(totalDelta.X, 0, totalDelta.Z),
            GizmoAxis.YZ => new Vector3(0, totalDelta.Y, totalDelta.Z),

            _ => Vector3.Zero
        };
    }
}