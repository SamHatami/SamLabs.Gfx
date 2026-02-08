using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Drawing.Geometry;

public struct ArcSegmentComponent:IDrawingComponent
{
    public Vector3 Center;
    public float Radius;
    public float StartAngle;
    public float EndAngle;
    public Vector3 Normal; 
}