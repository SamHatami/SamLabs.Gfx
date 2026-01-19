using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Core;

public class EditorEvents
{
    public event EventHandler<Entity>? EntityAdded;
    public event EventHandler<Entity>? EntityRemoved;
    public event EventHandler<Entity>? EntityUpdated;
    public event EventHandler<Entity>? EntityDeleted;

    public event EventHandler<Entity>? SelectedEntityChanged;
    public event EventHandler<Entity>? SelectedEntityAdded;

    public event EventHandler<TransformUpdatingArgs>? TransformUpdating;

    // New selection cleared event
    public event EventHandler<SelectionClearedArgs>? SelectionCleared;

    public void OnEntityAdded(Entity e) => EntityAdded?.Invoke(this, e);

    protected virtual void OnEntityRemoved(Entity e) => EntityRemoved?.Invoke(this, e);

    protected virtual void OnEntityUpdated(Entity e) => EntityUpdated?.Invoke(this, e);

    protected virtual void OnEntityDeleted(Entity e) => EntityDeleted?.Invoke(this, e);

    protected virtual void OnSelectedEntityChanged(Entity e) => SelectedEntityChanged?.Invoke(this, e);

    protected virtual void OnSelectedEntityAdded(Entity e) => SelectedEntityAdded?.Invoke(this, e);

    protected virtual void OnTransformUpdating(TransformUpdatingArgs e) => TransformUpdating?.Invoke(this, e);

    // Protected invoker - for overrides
    protected virtual void OnSelectionCleared(SelectionClearedArgs e) => SelectionCleared?.Invoke(this, e);

    // Public publisher so systems and other parts can raise the selection cleared event
    public void PublishSelectionCleared(SelectionClearedArgs e) => SelectionCleared?.Invoke(this, e);
}

public class TransformUpdatingArgs : EventArgs
{
    public TransformComponent Transform { get; }
    public Entity Entity { get; }


    public TransformUpdatingArgs(TransformComponent transform, Entity entity)
    {
        Transform = transform;
        Entity = entity;
    }
}

public class SelectionClearedArgs : EventArgs
{
    public int[] ClearedEntityIds { get; }

    public SelectionClearedArgs(int[] clearedEntityIds)
    {
        ClearedEntityIds = clearedEntityIds ?? System.Array.Empty<int>();
    }
}
