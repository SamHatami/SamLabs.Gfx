﻿using SamLabs.Gfx.Engine.Components.Common;
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

    public event EventHandler<SelectionClearedArgs>? SelectionCleared;
    
    public event EventHandler<ToolEventArgs>? ToolActivated;
    public event EventHandler<ToolEventArgs>? ToolDeactivated;
    public event EventHandler<ToolDataUpdatedArgs>? ToolDataUpdated;

    public void OnEntityAdded(Entity e) => EntityAdded?.Invoke(this, e);

    protected virtual void OnEntityRemoved(Entity e) => EntityRemoved?.Invoke(this, e);

    protected virtual void OnEntityUpdated(Entity e) => EntityUpdated?.Invoke(this, e);

    protected virtual void OnEntityDeleted(Entity e) => EntityDeleted?.Invoke(this, e);

    protected virtual void OnSelectedEntityChanged(Entity e) => SelectedEntityChanged?.Invoke(this, e);

    protected virtual void OnSelectedEntityAdded(Entity e) => SelectedEntityAdded?.Invoke(this, e);

    protected virtual void OnTransformUpdating(TransformUpdatingArgs e) => TransformUpdating?.Invoke(this, e);

    protected virtual void OnSelectionCleared(SelectionClearedArgs e) => SelectionCleared?.Invoke(this, e);

    public void PublishSelectionCleared(SelectionClearedArgs e) => SelectionCleared?.Invoke(this, e);
    
    public void PublishToolActivated(ToolEventArgs e) => ToolActivated?.Invoke(this, e);
    public void PublishToolDeactivated(ToolEventArgs e) => ToolDeactivated?.Invoke(this, e);
    public void PublishToolDataUpdated(ToolDataUpdatedArgs e) => ToolDataUpdated?.Invoke(this, e);
    
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

public class ToolEventArgs : EventArgs
{
    public string ToolId { get; }
    public string ToolName { get; }

    public ToolEventArgs(string toolId, string toolName)
    {
        ToolId = toolId;
        ToolName = toolName;
    }
}

public class ToolDataUpdatedArgs : EventArgs
{
    public string ToolId { get; }
    public Dictionary<string, object> Data { get; }

    public ToolDataUpdatedArgs(string toolId, Dictionary<string, object> data)
    {
        ToolId = toolId;
        Data = data;
    }
}

