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
/// Manages frame geometry transformations. Updates member transforms when nodes move,
/// and propagates member rigid-body transforms to connected nodes.
/// </summary>
public class FrameGeometrySystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition { get; } = SystemOrders.TransformUpdate + 1; // Run after transform/manipulator updates

    public FrameGeometrySystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        ProcessMemberTransformedPath();
        ProcessNodeMovedPath();

        // Catch any additional node flags raised during this update so connected members
        // are refreshed in the same frame and do not visually lag one frame behind drag input.
        var remainingMovedNodes = _entityRegistry.Query.With<FrameNodeTag>().With<NodeMovedFlag>().GetSpan();
        if (!remainingMovedNodes.IsEmpty())
        {
            ProcessNodeMovedPath();
        }
    }

    /// <summary>
    /// Updates all frame members connected to nodes that have moved.
    /// </summary>
    private void ProcessNodeMovedPath()
    {
        var movedNodes = _entityRegistry.Query.With<FrameNodeTag>().With<NodeMovedFlag>().GetSpan();
        if (movedNodes.IsEmpty()) return;

        var allMembers = ComponentRegistry.GetEntityIdsForComponentType<FrameMemberComponent>();
    
        foreach (var movedNodeId in movedNodes)
        {
            if (ComponentRegistry.HasComponent<PendingRemovalFlag>(movedNodeId))
            {
                ComponentRegistry.RemoveComponentFromEntity<NodeMovedFlag>(movedNodeId);
                continue;
            }

            var flag = ComponentRegistry.GetComponent<NodeMovedFlag>(movedNodeId);
        
            // Find all members connected to this node
            foreach (var memberId in allMembers)
            {
                if (memberId == flag.OriginatingMemberId) continue; // skip to avoid feedback loop
            
                var memberComponent = ComponentRegistry.GetComponent<FrameMemberComponent>(memberId);
                if (memberComponent.StartNodeEntityId == movedNodeId || memberComponent.EndNodeEntityId == movedNodeId)
                {
                    FrameGeometryUtility.UpdateMemberTransform(ComponentRegistry, memberId);
                }
            }

            // Clear the flag - we own this flag
            ComponentRegistry.RemoveComponentFromEntity<NodeMovedFlag>(movedNodeId);
        }
    }

    /// <summary>
    /// Propagates rigid-body transforms from members to their connected nodes.
    /// Sets NodeMovedFlag on affected nodes so connected members can be updated in this update cycle.
    /// </summary>
    private void ProcessMemberTransformedPath()
    {
        var transformedMembers = _entityRegistry.Query.With<FrameMemberComponent>().With<MemberTransformedFlag>().GetSpan();
        if (transformedMembers.IsEmpty()) return;
        foreach (var memberId in transformedMembers)
        {
            var memberComponent = ComponentRegistry.GetComponent<FrameMemberComponent>(memberId);
            var startNodeId = memberComponent.StartNodeEntityId;
            var endNodeId = memberComponent.EndNodeEntityId;

            // Get the delta from the flag
            ref var memberTransformedFlag = ref ComponentRegistry.GetComponent<MemberTransformedFlag>(memberId);
            var delta = memberTransformedFlag.Delta;
            var originatingMemberId = memberTransformedFlag.OriginatingMemberId;

            // Apply the delta to both nodes rigidly
            ref var startTransform = ref ComponentRegistry.GetComponent<TransformComponent>(startNodeId);
            startTransform.Position += delta;
            startTransform.WorldMatrix = startTransform.LocalMatrix;

            ref var endTransform = ref ComponentRegistry.GetComponent<TransformComponent>(endNodeId);
            endTransform.Position += delta;
            endTransform.WorldMatrix = endTransform.LocalMatrix;
            
            // Set NodeMovedFlag on both nodes so they update their other connected members this cycle
            ComponentRegistry.SetComponentToEntity(new NodeMovedFlag { OriginatingMemberId = originatingMemberId }, startNodeId);
            ComponentRegistry.SetComponentToEntity(new NodeMovedFlag { OriginatingMemberId = originatingMemberId }, endNodeId);

            // Clear the member's flag
            ComponentRegistry.RemoveComponentFromEntity<MemberTransformedFlag>(memberId);
        }
    }
}
