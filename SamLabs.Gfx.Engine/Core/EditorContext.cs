using SamLabs.Gfx.Engine.Components.Selection;

namespace SamLabs.Gfx.Engine.Core;

public class EditorContext
{
    //Keeper of the edit state
    
    public SelectionType SelectionType { get; }
    public EditorMode EditorMode { get; }
    
    public EditorContext()
    {
        
    }
}