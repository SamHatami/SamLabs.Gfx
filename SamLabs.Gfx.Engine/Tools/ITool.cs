﻿using System.ComponentModel;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Tools;

public interface ITool : INotifyPropertyChanged
{
    string ToolId { get; }
    string DisplayName { get; }
    ToolCategory Category { get; }
    ToolState State { get; }
    
    void Activate();
    void Deactivate();
    void ProcessInput(FrameInput input);
    IToolUIDescriptor GetUIDescriptor();
    
    event EventHandler<ToolStateChangedArgs>? StateChanged;
}

public class ToolStateChangedArgs : EventArgs
{
    public ToolState PreviousState { get; }
    public ToolState NewState { get; }

    public ToolStateChangedArgs(ToolState previousState, ToolState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}

