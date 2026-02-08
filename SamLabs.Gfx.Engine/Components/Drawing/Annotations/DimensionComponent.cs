using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Drawing.Annotations;

//Works in conjugation with Contraints?
public struct DimensionComponent:IDrawingComponent
{
    DimensionType Type { get; set; }
    DimensionStyle Style { get; set; }
    Vector3 AttachmentPointA { get; set; }
    Vector3 AttachmentPointB { get; set; }
    Vector3 TextPosition { get; set; }
    string Text { get; set; }
    
}