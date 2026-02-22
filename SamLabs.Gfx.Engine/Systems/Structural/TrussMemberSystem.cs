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
/// Manages member cleanup operations, including removing duplicate members
/// (members with the same start/end nodes).
/// </summary>
public class TrussMemberSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate + 1;

    public TrussMemberSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        var mergedNodes = _entityRegistry.Query.With<TrussNodeComponent>().With<NodesMergedFlag>().GetSpan();
        if (mergedNodes.IsEmpty())
            return;

        RemoveDuplicateMembers();

        foreach (var nodeId in mergedNodes)
            ComponentRegistry.RemoveComponentFromEntity<NodesMergedFlag>(nodeId);
    }

    private void RemoveDuplicateMembers()
    {
        var memberEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussMemberComponent>();
        if (memberEntities.IsEmpty())
            return;

        var nodePairToMemberIds = new Dictionary<(int, int), List<int>>();

        foreach (var memberId in memberEntities)
        {
            if (ComponentRegistry.HasComponent<PendingRemovalFlag>(memberId))
                continue;

            var memberComponent = ComponentRegistry.GetComponent<TrussMemberComponent>(memberId);
            var nodeA = memberComponent.StartNodeEntityId;
            var nodeB = memberComponent.EndNodeEntityId;
            var nodePair = nodeA < nodeB ? (nodeA, nodeB) : (nodeB, nodeA);

            if (!nodePairToMemberIds.ContainsKey(nodePair))
                nodePairToMemberIds[nodePair] = new List<int>();

            nodePairToMemberIds[nodePair].Add(memberId);
        }

        foreach (var (_, memberIds) in nodePairToMemberIds)
        {
            if (memberIds.Count <= 1)
                continue;

            for (var i = 1; i < memberIds.Count; i++)
            {
                var duplicateMemberId = memberIds[i];
                var memberComponent = ComponentRegistry.GetComponent<TrussMemberComponent>(duplicateMemberId);

                RemoveMemberFromNode(memberComponent.StartNodeEntityId, duplicateMemberId);
                RemoveMemberFromNode(memberComponent.EndNodeEntityId, duplicateMemberId);
                ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), duplicateMemberId);
            }
        }
    }

    private void RemoveMemberFromNode(int nodeId, int memberId)
    {
        if (!ComponentRegistry.HasComponent<TrussNodeComponent>(nodeId))
            return;

        ref var nodeComponent = ref ComponentRegistry.GetComponent<TrussNodeComponent>(nodeId);
        nodeComponent.ConnectedMemberIds.Remove(memberId);
    }
}
