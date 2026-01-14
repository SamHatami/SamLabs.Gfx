using System;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Components;

public interface IComponentRegistry
{
    void RemoveComponentFromEntities<T>(ReadOnlySpan<int> entityIds) where T : IComponent;
    void RemoveComponentFromEntities<T>(int[] entityIds) where T : IComponent;
    void RemoveComponentFromEntity<T>(int entityId) where T : IComponent;
    void RemoveEntity(int entityId);
    void SetComponentToEntity<T>(T component, int entityId) where T : IComponent;
    ref T GetComponent<T>(int entityId) where T : struct, IComponent;
    int GetId<T>() where T : IComponent;
    ReadOnlySpan<int> GetEntityIdsForComponentType<T>() where T : IComponent;
    ReadOnlySpan<int> GetChildEntitiesForParent(int parentId, Span<int> results);
    bool HasComponent<T>(int entityId) where T : IComponent;
}

