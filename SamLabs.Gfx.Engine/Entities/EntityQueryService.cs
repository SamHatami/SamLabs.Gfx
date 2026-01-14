using System;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Entities;

public class EntityQueryService
{
    private readonly IComponentRegistry _components;
    private readonly int[] _queryBuffer = new int[EditorSettings.MaxEntities];

    public EntityQueryService(IComponentRegistry components)
    {
        _components = components;
    }

    public ReadOnlySpan<int> With<T>() where T : IComponent
    {
        return _components.GetEntityIdsForComponentType<T>();
    }

    public ReadOnlySpan<int> AndWith<T>(ReadOnlySpan<int> entities) where T : IComponent
    {
        var count = 0;
        foreach (var e in entities)
        {
            if (_components.HasComponent<T>(e))
                _queryBuffer[count++] = e;
        }
        return _queryBuffer.AsSpan(0, count);
    }

    public ReadOnlySpan<int> OrWith<T>(ReadOnlySpan<int> entities) where T : IComponent
    {
        var entsWithT = _components.GetEntityIdsForComponentType<T>();
        if (entities.IsEmpty) return entsWithT;
        if (entsWithT.IsEmpty) return entities;

        // copy original entities
        var count = 0;
        for (var i = 0; i < entities.Length; i++) _queryBuffer[count++] = entities[i];

        // append those not present
        for (var i = 0; i < entsWithT.Length; i++)
        {
            var candidate = entsWithT[i];
            var found = false;
            for (var j = 0; j < entities.Length; j++) if (entities[j] == candidate) { found = true; break; }
            if (!found) _queryBuffer[count++] = candidate;
        }

        return _queryBuffer.AsSpan(0, count);
    }

    public ReadOnlySpan<int> Without<T>(ReadOnlySpan<int> entities) where T : IComponent
    {
        var count = 0;
        foreach (var e in entities)
        {
            if (!_components.HasComponent<T>(e))
                _queryBuffer[count++] = e;
        }
        return _queryBuffer.AsSpan(0, count);
    }

    public int First(ReadOnlySpan<int> entities) => entities.IsEmpty ? -1 : entities[0];
}

