﻿using System.ComponentModel;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Tools.Transforms;

public abstract class TransformTool : ITool, INotifyPropertyChanged
{
    protected readonly IComponentRegistry ComponentRegistry;
    protected readonly CommandManager CommandManager;
    protected readonly EntityQueryService Query;
    
    protected int? ActiveManipulatorId { get; private set; }
    protected ManipulatorType ManipulatorType { get; }
    protected ToolState _state = ToolState.Inactive;
    protected bool _isTransforming;
    protected int _selectedManipulatorSubEntity;
    protected TransformComponent _preChangeTransform;
    
    public abstract string ToolId { get; }
    public abstract string DisplayName { get; }
    public ToolCategory Category => ToolCategory.Transform;
    public ToolState State => _state;

    public event EventHandler<ToolStateChangedArgs>? StateChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected TransformTool(
        ManipulatorType manipulatorType,
        IComponentRegistry componentRegistry,
        CommandManager commandManager,
        EntityQueryService query,
        EditorEvents editorEvents)
    {
        ManipulatorType = manipulatorType;
        ComponentRegistry = componentRegistry;
        CommandManager = commandManager;
        Query = query;
    }

    public virtual void Activate()
    {
        HideAllManipulators();
        var targetManipulator = FindManipulatorByType(ManipulatorType);
        if (targetManipulator != -1)
        {
            ComponentRegistry.SetComponentToEntity(new ActiveManipulatorComponent(), targetManipulator);
            ActiveManipulatorId = targetManipulator;
        }
        
        SetState(ToolState.Active);
    }
    
    private void HideAllManipulators()
    {
        var activeManipulators = ComponentRegistry.GetEntityIdsForComponentType<ActiveManipulatorComponent>();
        foreach (var manipId in activeManipulators)
        {
            ComponentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(manipId);
        }
    }
    
    private int FindManipulatorByType(ManipulatorType type)
    {
        var manipulators = ComponentRegistry.GetEntityIdsForComponentType<ManipulatorComponent>();
        foreach (var manipId in manipulators)
        {
            ref var manip = ref ComponentRegistry.GetComponent<ManipulatorComponent>(manipId);
            if (manip.Type == type)
                return manipId;
        }
        return -1;
    }

    public virtual void Deactivate()
    {
        if (ActiveManipulatorId.HasValue && ComponentRegistry.HasComponent<ActiveManipulatorComponent>(ActiveManipulatorId.Value))
        {
            ComponentRegistry.RemoveComponentFromEntity<ActiveManipulatorComponent>(ActiveManipulatorId.Value);
        }
        
        SetState(ToolState.Inactive);
        ActiveManipulatorId = null;
        _isTransforming = false;
    }

    public abstract void ProcessInput(FrameInput input);
    public abstract IToolUIDescriptor GetUIDescriptor();
    public abstract void UpdateValues(double x, double y, double z);

    protected void SetState(ToolState newState)
    {
        if (_state != newState)
        {
            var oldState = _state;
            _state = newState;
            StateChanged?.Invoke(this, new ToolStateChangedArgs(oldState, newState));
        }
    }
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

