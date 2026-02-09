using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Sketch;

namespace SamLabs.Gfx.Engine.Components.Sketch.Annotations;

//Works in conjugation with Constraints?
public struct DimensionComponent : ISketchComponent
{
    public DimensionType Type { get; set; }
    public DimensionStyle Style { get; set; }
    public Vector3 AttachmentPointA { get; set; }
    public Vector3 AttachmentPointB { get; set; }
    public Vector3 TextPosition { get; set; }
    public string Text { get; set; }
}

