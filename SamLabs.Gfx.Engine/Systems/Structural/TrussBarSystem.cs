using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Structural.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;
/// <summary>
/// Manages bar cleanup operations, including removing duplicate bars (bars with same start/end nodes)
/// </summary>
public class TrussBarSystem:UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate + 1; // Run after ScaleToScreenSystem

    public TrussBarSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        // Only check for duplicate bars when nodes have been merged
        var mergedNodes = _entityRegistry.Query.With<TrussNodeComponent>().With<NodesMergedFlag>().GetSpan();
        if (mergedNodes.IsEmpty()) return;

        RemoveDuplicateBars();

        foreach (var nodeId in mergedNodes)
        {
            ComponentRegistry.RemoveComponentFromEntity<NodesMergedFlag>(nodeId);
        }
    }

    private void RemoveDuplicateBars()
    {
        var barEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussBarComponent>();
        if (barEntities.IsEmpty()) return;

        // Build a map of node pairs to bar IDs
        var nodePairToBarIds = new Dictionary<(int, int), List<int>>();

        foreach (var barId in barEntities)
        {
            // Skip bars pending removal
            if (ComponentRegistry.HasComponent<PendingRemovalFlag>(barId)) continue;

            var barComponent = ComponentRegistry.GetComponent<TrussBarComponent>(barId);
            var nodeA = barComponent.StartNodeEntityId;
            var nodeB = barComponent.EndNodeEntityId;

            // Normalize the pair so (A, B) and (B, A) are treated the same
            var nodePair = nodeA < nodeB ? (nodeA, nodeB) : (nodeB, nodeA);

            if (!nodePairToBarIds.ContainsKey(nodePair))
                nodePairToBarIds[nodePair] = new List<int>();

            nodePairToBarIds[nodePair].Add(barId);
        }

        // Find and remove duplicate bars
        foreach (var kvp in nodePairToBarIds)
        {
            var barIds = kvp.Value;
            if (barIds.Count <= 1) continue; // No duplicates

            // Keep the first bar, mark the rest for removal
            for (int i = 1; i < barIds.Count; i++)
            {
                var duplicateBarId = barIds[i];
                
                // Remove from connected nodes' bar lists
                var barComponent = ComponentRegistry.GetComponent<TrussBarComponent>(duplicateBarId);
                RemoveBarFromNode(barComponent.StartNodeEntityId, duplicateBarId);
                RemoveBarFromNode(barComponent.EndNodeEntityId, duplicateBarId);

                // Mark for removal
                ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), duplicateBarId);
            }
        }
    }

    private void RemoveBarFromNode(int nodeId, int barId)
    {
        if (!ComponentRegistry.HasComponent<TrussNodeComponent>(nodeId)) return;
        
        ref var nodeComponent = ref ComponentRegistry.GetComponent<TrussNodeComponent>(nodeId);
        nodeComponent.ConnectedBarIds.Remove(barId);
    }
}
