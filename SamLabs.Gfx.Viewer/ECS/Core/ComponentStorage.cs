using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Core;

public class ComponentStorage<T>: IComponentStorage where T : IDataComponent
{
    public T[] Items { get; } = new T[GlobalSettings.MaxEntities];

    public ref T Get(int entityId) => ref Items[entityId];
    public void Clear(int entityId) => Items[entityId] = default;
}

public interface IComponentStorage
{
    void Clear(int entityId);
}