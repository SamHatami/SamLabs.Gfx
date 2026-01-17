using SamLabs.Gfx.Engine.Components.Selection;

namespace SamLabs.Gfx.Engine.Core;

public class EditorContext
{
    //Keeper of the edit state
    //In what mode is Editor currently operating?
    //This dictates what kind of UI is shown to the user
    //What type of camera or view should be used
    //Which tools are available, ie manipulators, and if there are some special settings that goes along with them
    
    //This can be an abstract class, so we have 
    //3DEditMode
    //SkethMode
    //ResultMode
    //whatever else things might be needed for a specific editor mode.
    //systems will behave differently based on the mode
    //for instance, if we have a 2D mode, we won't have a camera, we'll have a 2D viewport
    
    //The modes are switched basically by commands, 
    public SelectionType SelectionType { get; }
    public EditorMode EditorMode { get; }
    
    public EditorContext()
    {
        
    }
}