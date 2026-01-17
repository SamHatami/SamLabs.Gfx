namespace SamLabs.Gfx.Engine.Components.Sketch;

public struct SketchComponent : IComponent
{
    //Activated during sketch mode, if this exists then the entity is a sketchable object
    public int ConstructionPlaneEntityId { get; set; }
}