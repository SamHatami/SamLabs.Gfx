using System.Numerics;
using CPURendering.Geometry;

namespace CPURendering;

/// <summary>
/// Triangle Math
/// </summary>
public static class TMath
{
    //Alot referenced from https://fgiesen.wordpress.com/2013/02/06/the-barycentric-conspirac/
    public static Vector3 BarycentricWeights(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 point)
    {
        var totalArea = Math.Abs(TriArea(v0, v1, v2)); // windingd-order independent
    
        var area0 = TriArea(v1, v2, point);
        var area1 = TriArea(v2, v0, point);
        var area2 = TriArea(v0, v1, point);
    
        return new Vector3
        {
            X = area0 / totalArea,
            Y = area1 / totalArea,
            Z = area2 / totalArea
        };


    }    
    
    public static Vector3 BarycentricCoordinates(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 point)
    {
        var a0 = Orientation2D(v1, v2, point);  
        var a1 = Orientation2D(v2, v0, point);    
        var a2 = Orientation2D(v0, v1, point);      
        
        return new Vector3
        {
            X = a0,
            Y = a1,
            Z = a2
        };

    }

    public static float Orientation2D(Vector2 v0, Vector2 v1, Vector2 v2)
    {
        var e1 = Vector2.Subtract(v1, v0);
        var e2 = Vector2.Subtract(v2, v0);
        return e1.Cross(e2);

    }
    public static float TriArea(Vector2 v0, Vector2 v1, Vector2 v2)
    {
        return Orientation2D(v0, v1, v2) / 2;
    }
    
    //Barycentric interpolation
    //z(p) = weight.x / w0 + weight.y/w1 + weights.z/w2; 

    public static BoundingBox2D GetBoundingBox2D(this Triangle triangle)
    {
        Vector2 v0 = triangle.Vertices[0].AsVector2();
        Vector2 v1 = triangle.Vertices[1].AsVector2();
        Vector2 v2 = triangle.Vertices[2].AsVector2();
        
        var min = Vector2.Min(Vector2.Min(v0, v1), v2);
        var max = Vector2.Max(Vector2.Max(v0, v1), v2);
        return new BoundingBox2D()
        {
            // MaxX = (int)Math.Round(max.X / 2, MidpointRounding.AwayFromZero)*2,
            // MaxY = (int)Math.Round(max.Y / 2, MidpointRounding.AwayFromZero)*2,
            // MinX = (int)Math.Round(min.X / 2, MidpointRounding.AwayFromZero)*2,
            // MinY = (int)Math.Round(min.Y / 2, MidpointRounding.AwayFromZero)*2,
            MaxX = (int)Math.Ceiling(max.X),
            MaxY = (int)Math.Ceiling(max.Y),
            MinX = (int)Math.Floor(min.X),
            MinY = (int)Math.Floor(min.Y),
        };
    }
}