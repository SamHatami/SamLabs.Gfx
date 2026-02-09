using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Sketch;

namespace SamLabs.Gfx.Engine.Components.Sketch.Geometry;

public class CurveRenderStyleComponent : ISketchComponent
{
    public float Thickness { get; set; }
    public bool IsDashed { get; set; }
    public float DashLength { get; set; }
    public float GapLength { get; set; }
    public Vector3 Color { get; set; }
}

