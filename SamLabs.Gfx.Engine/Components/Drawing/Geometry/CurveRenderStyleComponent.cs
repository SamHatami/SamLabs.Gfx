using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Drawing.Geometry;

public class CurveRenderStyleComponent:IDrawingComponent
{
        public float Thickness { get; set; }
        public bool IsDashed { get; set; }
        public float DashLength { get; set; }
        public float GapLength { get; set; }
        public Vector3 Color { get; set; }

}