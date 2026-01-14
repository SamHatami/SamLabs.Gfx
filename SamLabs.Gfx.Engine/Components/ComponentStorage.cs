using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Components;

public class ComponentStorage<T>: IComponentStorage where T : IComponent
{
    public T[] Items { get; } = new T[EditorSettings.MaxEntities];

    public ref T Get(int entityId) => ref Items[entityId];
    public void Clear(int entityId) => Items[entityId] = default;
}

public interface IComponentStorage
{
    void Clear(int entityId);
}
