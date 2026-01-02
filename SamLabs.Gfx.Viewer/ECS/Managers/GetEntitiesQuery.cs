using SamLabs.Gfx.Viewer.Core;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public static class GetEntityIds
{
    private static readonly int[] QueryBuffer = new int[GlobalSettings.MaxEntities];

    public static ReadOnlySpan<int> With<T>() where T : IDataComponent => ComponentManager.GetEntityIdsForComponentType<T>();

    extension(ReadOnlySpan<int> entities)
    {
        public ReadOnlySpan<int> AndWith<T>() where T : IDataComponent
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

        public ReadOnlySpan<int> OrWith<T>() where T : IDataComponent
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

        public ReadOnlySpan<int> Without<T>() where T : IDataComponent
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

        public int First() => entities.IsEmpty ? -1 : entities[0];
    }
}