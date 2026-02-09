using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Sketch;

namespace SamLabs.Gfx.Engine.Components.Sketch.Geometry;

public struct ArcSegmentComponent : ISketchComponent
{
    public Vector3 Center;
    public float Radius;
    public float StartAngle;
    public float EndAngle;
    public Vector3 Normal;
}

