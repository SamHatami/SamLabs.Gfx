using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Structural.Flags;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Components.Transform.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

public class TrussNodeSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate - 1;
    private readonly Dictionary<int, Vector3> _nodePositionMap = new();

    public TrussNodeSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        HandleMovedMembers();

        var nodeQuery = _entityRegistry.Query;
        var movedNodes = nodeQuery.With<TrussNodeComponent>().With<TranslateChangedFlag>().GetSpan();
        UpdateNodePositionMap(movedNodes);
        if (movedNodes.IsEmpty())
        {
            _entityRegistry.ReturnQuery(nodeQuery);
            return;
        }

        foreach (var nodeEntity in movedNodes)
        {
            ComponentRegistry.RemoveComponentFromEntity<TranslateChangedFlag>(nodeEntity);
            TrussNodeUtility.UpdateConnectedMembers(ComponentRegistry, ComponentRegistry.GetComponent<TrussNodeComponent>(nodeEntity));
        }

        MergeNodes(movedNodes);
        _entityRegistry.ReturnQuery(nodeQuery);
    }

    private void HandleMovedMembers()
    {
        var movedMembers = _entityRegistry.Query.With<TrussMemberComponent>().With<TranslateChangedFlag>().GetSpan();
        foreach (var memberId in movedMembers)
        {
            ref var member = ref ComponentRegistry.GetComponent<TrussMemberComponent>(memberId);
            var memberTransform = ComponentRegistry.GetComponent<TransformComponent>(memberId);
            ref var startNodeTransform = ref ComponentRegistry.GetComponent<TransformComponent>(member.StartNodeEntityId);
            ref var endNodeTransform = ref ComponentRegistry.GetComponent<TransformComponent>(member.EndNodeEntityId);

            var currentCenter = (startNodeTransform.Position + endNodeTransform.Position) * 0.5f;
            var delta = memberTransform.Position - currentCenter;
            if (delta.LengthSquared > 1e-8f)
            {
                startNodeTransform.Position += delta;
                startNodeTransform.WorldMatrix = startNodeTransform.LocalMatrix;
                endNodeTransform.Position += delta;
                endNodeTransform.WorldMatrix = endNodeTransform.LocalMatrix;
            }

            TrussNodeUtility.UpdateMemberTransform(ComponentRegistry, memberId);
            ComponentRegistry.RemoveComponentFromEntity<TranslateChangedFlag>(memberId);
            _nodePositionMap[member.StartNodeEntityId] = startNodeTransform.Position;
            _nodePositionMap[member.EndNodeEntityId] = endNodeTransform.Position;
        }
    }

    private void MergeNodes(ReadOnlySpan<int> sourceNodeEntities)
    {
        foreach (var sourceNodeId in sourceNodeEntities)
        {
            var nearestNodeId = FindNearestNodeId(sourceNodeId, 0.1f);
            if (nearestNodeId == -1) continue;

            ref var nearestNodeComponent = ref ComponentRegistry.GetComponent<TrussNodeComponent>(nearestNodeId);
            var sourceNodeComponent = ComponentRegistry.GetComponent<TrussNodeComponent>(sourceNodeId);
            if (!TrussNodeUtility.CanMergeNodes(nearestNodeComponent, sourceNodeComponent)) continue;

            foreach (var memberId in sourceNodeComponent.ConnectedMemberIds)
            {
                nearestNodeComponent.ConnectedMemberIds.Add(memberId);
                ref var member = ref ComponentRegistry.GetComponent<TrussMemberComponent>(memberId);
                if (member.StartNodeEntityId == sourceNodeId)
                    member.StartNodeEntityId = nearestNodeId;
                else
                    member.EndNodeEntityId = nearestNodeId;
            }

            foreach (var memberId in nearestNodeComponent.ConnectedMemberIds)
                TrussNodeUtility.UpdateMemberTransform(ComponentRegistry, memberId);

            var pickingEntity = _entityRegistry.Query.With<PickingDataComponent>().First();
            ref var picking = ref ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity);
            picking.SelectedEntityIds = [nearestNodeId];

            ComponentRegistry.SetComponentToEntity(new NodesMergedFlag(), nearestNodeId);
            ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), sourceNodeId);
            _nodePositionMap.Remove(sourceNodeId);
        }
    }

    private void UpdateNodePositionMap(ReadOnlySpan<int> updatedNodeEntities)
    {
        if (_nodePositionMap.Count == 0)
        {
            var allNodeEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussNodeComponent>();
            foreach (var nodeEntity in allNodeEntities)
                _nodePositionMap[nodeEntity] = ComponentRegistry.GetComponent<TransformComponent>(nodeEntity).Position;
        }

        foreach (var nodeEntity in updatedNodeEntities)
            _nodePositionMap[nodeEntity] = ComponentRegistry.GetComponent<TransformComponent>(nodeEntity).Position;
    }

    private int FindNearestNodeId(int sourceNodeId, float searchDistance)
    {
        var nearestNodeId = -1;
        var sourceNodePosition = _nodePositionMap[sourceNodeId];
        foreach (var nextNodeId in _nodePositionMap.Keys)
        {
            if (nextNodeId == sourceNodeId) continue;
            var distance = Vector3.Distance(_nodePositionMap[nextNodeId], sourceNodePosition);
            if (distance >= searchDistance) continue;
            nearestNodeId = nextNodeId;
            searchDistance = distance;
        }

        return nearestNodeId;
    }
}
