using SamLabs.Gfx.Viewer.Core;

namespace SamLabs.Gfx.Viewer.ECS.Core;

public struct ComponentMap
{
    public Type ComponentType { get;}
    private int[] _entityIds = new int[GlobalSettings.MaxEntities];

    private int _entityCount = 0;

    public ComponentMap(Type componentType)
    {
        ComponentType = componentType;
    }

    public void AddUsage(int entityId)
    {
        _entityIds[_entityCount] = entityId;
        _entityCount++;
    }

    public void RemoveUsage(int entityId)
    {
        for (int i = 0; i < GlobalSettings.MaxEntities; i++)
        {
            if (_entityIds[i] != entityId) continue;
            
            _entityIds[i] = _entityIds[_entityCount - 1];
            _entityCount--; 
        }
    }

    public ReadOnlySpan<int> GetUsageIds() => _entityIds.AsSpan().Slice(0, _entityCount);
}