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

    public void OnEntityAdded(Entity e) => EntityAdded?.Invoke(this, e);

    protected virtual void OnEntityRemoved(Entity e) => EntityRemoved?.Invoke(this, e);

    protected virtual void OnEntityUpdated(Entity e) => EntityUpdated?.Invoke(this, e);

    protected virtual void OnEntityDeleted(Entity e) => EntityDeleted?.Invoke(this, e);

    protected virtual void OnSelectedEntityChanged(Entity e) => SelectedEntityChanged?.Invoke(this, e);

    protected virtual void OnSelectedEntityAdded(Entity e) => SelectedEntityAdded?.Invoke(this, e);

    protected virtual void OnTransformUpdating(TransformUpdatingArgs e) => TransformUpdating?.Invoke(this, e);
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