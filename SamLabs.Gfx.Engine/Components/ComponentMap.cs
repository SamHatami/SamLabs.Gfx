using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Components;

public class ComponentMap
{
    public Type ComponentType { get;}
    private int[] _entityIds = new int[GlobalSettings.MaxEntities]; //spare array for fast lookup
    
    private int[] _lookUpSpanSet = new int[GlobalSettings.MaxEntities]; //dense array for fast iteration when system ask for usages

    private int _entityCount = 0;

    public ComponentMap(Type componentType)
    {
        ComponentType = componentType;
        Array.Fill(_entityIds, -1);
    }

    public void AddUsage(int entityId)
    {
        if(Has(entityId)) return;
        
        _entityIds[entityId] = _entityCount; //just something else than -1 and save for later
        _lookUpSpanSet[_entityCount] = entityId;
        _entityCount++;
    }

    public void RemoveUsage(int entityId)
    {
        if (!Has(entityId)) return;
        
        //swap with last
        var positionToRemove = _entityIds[entityId];
        var lastInDense = _entityIds[_entityCount - 1];
        
        _lookUpSpanSet[positionToRemove] = lastInDense;
        _entityIds[entityId] = -1;

        _entityCount--;
    }

    public ReadOnlySpan<int> GetUsageIds() => _lookUpSpanSet.AsSpan().Slice(0, _entityCount);

    public bool Has(int entityId)
    {
        if(entityId >= _entityIds.Length || entityId < 0) return false;
        return _entityIds[entityId] != -1;
    }
}