using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Structural.Flags;
using SamLabs.Gfx.Engine.Components.Transform.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Structural;

/// <summary>
/// Tracks a map over where the nodes are for merging.
/// Updates connected bars when nodes are moved => no... not really, the translatetool calls this, which is not good.
/// </summary>
public class TrussNodeSystem : UpdateSystem
{
    private readonly EntityRegistry _entityRegistry;
    private readonly Dictionary<int, Quaternion> _previousRotations = new();
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate - 1; // Run before ScaleToScreenSystem

    private Dictionary<int, Vector3> _nodePositionMap = new();

    //Responsible for merging truss nodes that are within a certain threshold distance or splitting them
    //FLags required for this
    public TrussNodeSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput)
    {
        var nodeQuery = _entityRegistry.Query;
        var nodeEntities = nodeQuery.With<TrussNodeComponent>().With<TranslateChangedFlag>().GetSpan();
        UpdateNodePositionMap(nodeEntities); //Wonder if this is doing it in the correct order?
        if (nodeEntities.IsEmpty())
        {
            _entityRegistry.ReturnQuery(nodeQuery);
            return;
        }

        foreach (var nodeEntity in nodeEntities)
            ComponentRegistry.RemoveComponentFromEntity<TranslateChangedFlag>(nodeEntity);

        MergeNodes(nodeEntities);
        _entityRegistry.ReturnQuery(nodeQuery);
        
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

            foreach (var barId in sourceNodeComponent.ConnectedBarIds)
            {
                nearestNodeComponent.ConnectedBarIds.Add(barId);
                ref var barComponent = ref ComponentRegistry.GetComponent<TrussBarComponent>(barId);
                if (barComponent.StartNodeEntityId == sourceNodeId)
                    barComponent.StartNodeEntityId = nearestNodeId;
                else
                    barComponent.EndNodeEntityId = nearestNodeId;
            }

            //PRobably should be a event of some sort?
            var pickingEntity = _entityRegistry.Query.With<PickingDataComponent>().First();
            ref var picking = ref ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity);
            picking.SelectedEntityIds = [nearestNodeId];
            
            TrussNodeUtility.UpdateConnectedBars(ComponentRegistry, nearestNodeComponent, nearestNodeId);

            ComponentRegistry.SetComponentToEntity(new NodesMergedFlag(), nearestNodeId);
            ComponentRegistry.SetComponentToEntity(new PendingRemovalFlag(), sourceNodeId);
            _nodePositionMap.Remove(sourceNodeId);
            
            var transformComponent = ComponentRegistry.GetComponent<TransformComponent>(nearestNodeId);
            _nodePositionMap[nearestNodeId] = transformComponent.Position;
        }
    }

    private void UpdateNodePositionMap(ReadOnlySpan<int> updatedNodeEntities)
    {
        if (_nodePositionMap.Count == 0) //initial population
        {
            var allNodeEntities = ComponentRegistry.GetEntityIdsForComponentType<TrussNodeComponent>();
            foreach (var nodeEntity in allNodeEntities)
            {
                var transformComponent = ComponentRegistry.GetComponent<TransformComponent>(nodeEntity);
                _nodePositionMap[nodeEntity] = transformComponent.Position;
            }
        }

        if (updatedNodeEntities.Length == 0) return;

        foreach (var nodeEntity in updatedNodeEntities)
        {
            var transformComponent = ComponentRegistry.GetComponent<TransformComponent>(nodeEntity);
            _nodePositionMap[nodeEntity] = transformComponent.Position;
        }
    }

    private int FindNearestNodeId(int sourceNodeId, float searchDistance)
    {
        int nearestNodeId = -1;
        var minDistance = -1f;
        var sourceNodePosition = _nodePositionMap[sourceNodeId];
        foreach (var nextNodeId in _nodePositionMap.Keys)
        {
            if (nextNodeId == sourceNodeId) continue;
            var distance = Vector3.Distance(_nodePositionMap[nextNodeId], sourceNodePosition);
            if (!(distance < searchDistance)) continue;
            minDistance = distance;
            nearestNodeId = nextNodeId;
        }

        return nearestNodeId;
    }
}