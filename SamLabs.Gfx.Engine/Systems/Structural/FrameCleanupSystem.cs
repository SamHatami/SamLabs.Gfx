using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

/// <summary>
/// Removes duplicate frame members (members with identical start/end node pairs) after merges.
/// </summary>
public class FrameCleanupSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate + 1; // Run after FrameGeometrySystem

    public FrameCleanupSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        RemoveDuplicateMembers();
    }

    /// <summary>
    /// Removes duplicate frame members, keeping only the first of each node pair.
    /// </summary>
    private void RemoveDuplicateMembers()
    {
        var memberEntities = ComponentRegistry.GetEntityIdsForComponentType<FrameMemberComponent>();
        if (memberEntities.IsEmpty()) return;

        // Build a map of node pairs to member IDs
        var nodePairToMemberIds = new Dictionary<(int, int), List<int>>();

        foreach (var memberId in memberEntities)
        {
            // Skip members pending removal
            if (ComponentRegistry.HasComponent<PendingRemovalFlag>(memberId)) continue;

            var memberComponent = ComponentRegistry.GetComponent<FrameMemberComponent>(memberId);
            var nodeA = memberComponent.StartNodeEntityId;
            var nodeB = memberComponent.EndNodeEntityId;

            // Normalize the pair so (A, B) and (B, A) are treated the same
            var nodePair = nodeA < nodeB ? (nodeA, nodeB) : (nodeB, nodeA);

            if (!nodePairToMemberIds.ContainsKey(nodePair))
                nodePairToMemberIds[nodePair] = new List<int>();

            nodePairToMemberIds[nodePair].Add(memberId);
        }

        // Find and remove duplicate members
        foreach (var kvp in nodePairToMemberIds)
        {
            var memberIds = kvp.Value;
            if (memberIds.Count <= 1) continue; // No duplicates

            // Keep the first member, mark the rest for removal
            for (int i = 1; i < memberIds.Count; i++)
            {
                var duplicateMemberId = memberIds[i];
                ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), duplicateMemberId);
            }
        }
    }
}
