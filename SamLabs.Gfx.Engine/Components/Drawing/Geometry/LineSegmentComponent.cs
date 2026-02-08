using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Drawing.Geometry;

public struct LineSegmentComponent:IDrawingComponent
{
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }

    
}