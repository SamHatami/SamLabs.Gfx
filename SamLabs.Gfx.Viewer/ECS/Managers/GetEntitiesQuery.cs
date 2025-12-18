using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public static class GetEntities
{
    private static readonly int[] QueryBuffer = new int[GlobalSettings.MaxEntities];

    public static ReadOnlySpan<int> With<T>() where T : IDataComponent => ComponentManager.GetEntityIdsForComponentType<T>();

    public static ReadOnlySpan<int> AndWith<T>(this ReadOnlySpan<int> entities) where T : IDataComponent
    {
        var count = 0;
        foreach (var entity in entities)
        {
            if (ComponentManager.HasComponent<T>(entity))
            {
                QueryBuffer[count++] = entity;
            }
        }

        return QueryBuffer.AsSpan(0, count);
    }

    public static ReadOnlySpan<int> OrWith<T>(this ReadOnlySpan<int> entities) where T : IDataComponent
    {
        var entsWithT = ComponentManager.GetEntityIdsForComponentType<T>();
        if (entities.IsEmpty) return entsWithT;
        if (entsWithT.IsEmpty) return entities;

        entities.CopyTo(QueryBuffer);
        var count = entities.Length;

        foreach (var entity in entsWithT)
        {
            if (!entities.Contains(entity))
            {
                QueryBuffer[count++] = entity;
            }
        }

        return QueryBuffer.AsSpan(0, count);
    }

    public static ReadOnlySpan<int> Without<T>(this ReadOnlySpan<int> entities) where T : IDataComponent
    {
        var count = 0;
        foreach (var entity in entities)
        {
            if (!ComponentManager.HasComponent<T>(entity))
            {
                QueryBuffer[count++] = entity;
            }
        }

        return QueryBuffer.AsSpan(0, count);
    }
}