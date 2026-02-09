using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Sketch;

namespace SamLabs.Gfx.Engine.Components.Sketch.Geometry;

public struct LineSegmentComponent : ISketchComponent
{
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
}

