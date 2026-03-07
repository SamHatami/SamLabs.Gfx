﻿using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Structural.Flags;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

/// <summary>
/// Manages node merging in the frame system. Detects when frame nodes are close together
/// and merges them when no shared members exist.
/// </summary>
public class FrameMergeSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    private Dictionary<int, Vector3> _nodePositionMap = new();
    public override int SystemPosition { get; } = SystemOrders.TransformUpdate + 2; // Run after FrameGeometrySystem

    public FrameMergeSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        RebuildNodePositionMap();
        
        var movedNodes = _entityRegistry.Query.With<FrameNodeTag>().With<NodeMovedFlag>().GetSpan();
        if (movedNodes.IsEmpty()) return;

        MergeNodes(movedNodes);
    }

    /// <summary>
    /// Merges frame nodes that are within 0.1f distance of each other and have no shared members.
    /// </summary>
    private void MergeNodes(ReadOnlySpan<int> movedNodeIds)
    {
        foreach (var sourceNodeId in movedNodeIds)
        {
            var nearestNodeId = FindNearestNodeId(sourceNodeId, 0.1f);
            if (nearestNodeId == -1) continue;

            if (!CanMergeNodes(sourceNodeId, nearestNodeId)) continue;

            // Redirect all members from source node to target node
            var allMembers = ComponentRegistry.GetEntityIdsForComponentType<FrameMemberComponent>();
            foreach (var memberId in allMembers)
            {
                ref var memberComponent = ref ComponentRegistry.GetComponent<FrameMemberComponent>(memberId);
                if (memberComponent.StartNodeEntityId == sourceNodeId)
                    memberComponent.StartNodeEntityId = nearestNodeId;
                else if (memberComponent.EndNodeEntityId == sourceNodeId)
                    memberComponent.EndNodeEntityId = nearestNodeId;
            }

            // Set flag on target node to update its members next frame
            ComponentRegistry.SetComponentToEntity(new NodeMovedFlag(), nearestNodeId);

            // Mark source node for removal
            ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), sourceNodeId);
            _nodePositionMap.Remove(sourceNodeId);
        }
    }

    /// <summary>
    /// Checks if two nodes can be merged (no shared members).
    /// </summary>
    private bool CanMergeNodes(int nodeA, int nodeB)
    {
        var allMembers = ComponentRegistry.GetEntityIdsForComponentType<FrameMemberComponent>();
        
        foreach (var memberId in allMembers)
        {
            var memberComponent = ComponentRegistry.GetComponent<FrameMemberComponent>(memberId);
            
            // Check if this member is already shared
            var membersA = (memberComponent.StartNodeEntityId == nodeA || memberComponent.EndNodeEntityId == nodeA);
            var membersB = (memberComponent.StartNodeEntityId == nodeB || memberComponent.EndNodeEntityId == nodeB);
            
            if (membersA && membersB) return false; // Shared member found
        }

        return true;
    }

    /// <summary>
    /// Rebuilds the node position map with current positions of all frame nodes.
    /// Ensures the map is always fresh and no positions are stale.
    /// </summary>
    private void RebuildNodePositionMap()
    {
        _nodePositionMap.Clear();
        var allNodes = ComponentRegistry.GetEntityIdsForComponentType<FrameNodeTag>();
        foreach (var nodeEntity in allNodes)
        {
            var transform = ComponentRegistry.GetComponent<TransformComponent>(nodeEntity);
            _nodePositionMap[nodeEntity] = transform.Position;
        }
    }

    /// <summary>
    /// Finds the nearest frame node to a source node within the specified search distance.
    /// </summary>
    private int FindNearestNodeId(int sourceNodeId, float searchDistance)
    {
        int nearestNodeId = -1;
        var minDistance = float.MaxValue;
        var sourceNodePosition = _nodePositionMap[sourceNodeId];

        foreach (var nextNodeId in _nodePositionMap.Keys)
        {
            if (nextNodeId == sourceNodeId) continue;
            var distance = Vector3.Distance(_nodePositionMap[nextNodeId], sourceNodePosition);
            if (distance >= searchDistance) continue;
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNodeId = nextNodeId;
            }
        }

        return nearestNodeId;
    }
}
