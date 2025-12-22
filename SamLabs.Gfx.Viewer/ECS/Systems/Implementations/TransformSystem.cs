using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

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
        //TODO: How to attach the gizmo to the selected entity?

        //which axis or plane is selected as the transfom origin?
        //is the user currently trying to transform?

        ref var entityTransform = ref ComponentManager.GetComponent<TransformComponent>(selectedEntities[0]);
        ref var gizmoTransform = ref ComponentManager.GetComponent<TransformComponent>(activeGizmo);

        // Keep gizmo at selected entity position
        gizmoTransform.Position = entityTransform.Position;

        // Update child gizmo world transforms based on parent
        Span<int> childBuffer = stackalloc int[12];
        // var subGizmoEntities = ComponentManager.GetChildEntitiesForParent(activeGizmo, childBuffer);
        // // UpdateChildrenWorldTransforms(subGizmoEntities, gizmoTransform);

        if (frameInput.IsMouseLeftButtonDown && !_isTransforming)
        {
            // Check if user clicked on gizmo child (axis)
            var pickingEntities = GetEntitiesIds.With<PickingDataComponent>();
            if (!pickingEntities.IsEmpty)
            {
                ref var pickingData = ref ComponentManager.GetComponent<PickingDataComponent>(pickingEntities[0]);

                // Check if hovered entity is a gizmo child
                if (ComponentManager.HasComponent<GizmoChildComponent>(pickingData.HoveredEntityId))
                {
                    _isTransforming = true;
                    _selectedGizmoSubEntity = pickingData.HoveredEntityId;
                    _transformStartPoint = entityTransform.Position;
                }
            }
        }

        if (_isTransforming && frameInput.IsMouseLeftButtonDown)
        {
            // Apply transformation based on mouse delta
            var delta = CalculateTransformDelta(frameInput);
            entityTransform.Position += delta;
            gizmoTransform.Position = entityTransform.Position;
        }

        if (!frameInput.IsMouseLeftButtonDown && _isTransforming)
        {
            _isTransforming = false;
            _selectedGizmoSubEntity = -1;
        }
    }

    private void UpdateChildrenWorldTransforms(ReadOnlySpan<int> childEntities, TransformComponent parentTransform)
    {
        foreach (var childId in childEntities)
        {
            ref var childTransform = ref ComponentManager.GetComponent<TransformComponent>(childId);
            childTransform.Position = parentTransform.Position + childTransform.LocalPosition ;
            childTransform.Rotation = parentTransform.Rotation * childTransform.LocalRotation;
            childTransform.Scale = parentTransform.Scale * childTransform.LocalScale;
        }
    }

    private Vector3 CalculateTransformDelta(FrameInput frameInput)
    {
        // Get camera for ray casting
        var cameraEntities = GetEntitiesIds.With<CameraComponent>();
        if (cameraEntities.IsEmpty) return Vector3.Zero;

        ref var cameraTransform = ref ComponentManager.GetComponent<TransformComponent>(cameraEntities[0]);
        ref var gizmoChild = ref ComponentManager.GetComponent<GizmoChildComponent>(_selectedGizmoSubEntity);

        var mouseDelta = frameInput.DeltaMouseMove;
        var sensitivity = 0.01f;

        // Constrain movement based on selected axis
        // Note: Y is inverted because screen Y goes down but world Y goes up
        return gizmoChild.Axis switch
        {
            GizmoAxis.X => new Vector3(mouseDelta.X * sensitivity, 0, 0),
            GizmoAxis.Y => new Vector3(0, -mouseDelta.Y * sensitivity, 0),
            GizmoAxis.Z => new Vector3(0, 0, -mouseDelta.X * sensitivity),
            GizmoAxis.XY => new Vector3(mouseDelta.X * sensitivity, -mouseDelta.Y * sensitivity, 0),
            GizmoAxis.XZ => new Vector3(mouseDelta.X * sensitivity, 0, -mouseDelta.Y * sensitivity),
            GizmoAxis.YZ => new Vector3(0, -mouseDelta.X * sensitivity, mouseDelta.Y * sensitivity),
            _ => Vector3.Zero
        };
    }
}