using SamLabs.Gfx.Viewer.ECS.Components.Selection;

namespace SamLabs.Gfx.Viewer.Core;

public class EditorContext
{
    //Keeper of the edit state
    
    public SelectionType SelectionType { get; }
    
    public EditorContext()
    {
        
    }
}