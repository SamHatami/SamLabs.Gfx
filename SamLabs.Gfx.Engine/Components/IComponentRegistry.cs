using System;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Components;

public interface IComponentRegistry
{
    void RemoveComponentFromEntities<T>(ReadOnlySpan<int> entityIds) where T : IDataComponent;
    void RemoveComponentFromEntities<T>(int[] entityIds) where T : IDataComponent;
    void RemoveComponentFromEntity<T>(int entityId) where T : IDataComponent;
    void RemoveEntity(int entityId);
    void SetComponentToEntity<T>(T component, int entityId) where T : IDataComponent;
    ref T GetComponent<T>(int entityId) where T : struct, IDataComponent;
    int GetId<T>() where T : IDataComponent;
    ReadOnlySpan<int> GetEntityIdsForComponentType<T>() where T : IDataComponent;
    ReadOnlySpan<int> GetChildEntitiesForParent(int parentId, Span<int> results);
    bool HasComponent<T>(int entityId) where T : IDataComponent;
}

