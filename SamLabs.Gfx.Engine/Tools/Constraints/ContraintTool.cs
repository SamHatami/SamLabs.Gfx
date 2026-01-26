using System.ComponentModel;
using SamLabs.Gfx.Engine.IO;

namespace SamLabs.Gfx.Engine.Tools.Constraints;

public class ContraintTool:ITool
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string ToolId { get; }
    public string DisplayName { get; }
    public ToolCategory Category { get; }
    public ToolState State { get; }
    public void Activate()
    {
        throw new NotImplementedException();
    }

    public void Deactivate()
    {
        throw new NotImplementedException();
    }

    public void ProcessInput(FrameInput input)
    {
        throw new NotImplementedException();
    }

    public IToolUIDescriptor GetUIDescriptor()
    {
        throw new NotImplementedException();
    }

    public event EventHandler<ToolStateChangedArgs>? StateChanged;
}