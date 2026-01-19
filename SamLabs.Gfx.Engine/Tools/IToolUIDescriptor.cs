namespace SamLabs.Gfx.Engine.Tools;

public interface IToolUIDescriptor
{
    ToolUIType UIType { get; }
    string ViewModelTypeName { get; }
    
    Dictionary<string, object> GetDisplayData();
    void UpdateFromUI(Dictionary<string, object> data);
}

