using System.Numerics;
using CPURendering.Geometry;
using CPURendering.View;
using SDL3;

namespace CPURendering;

public class Rasterizer
{
    private readonly Display _display;

    public Rasterizer(Display display)
    {
        _display = display;
    }

    public void DrawTriangleEdges(Triangle triangle, uint color = 0xFFFFFFFF)
    {
        _display.DrawLine(triangle.Vertices[0].AsVector2(), triangle.Vertices[1].AsVector2(), color);
        _display.DrawLine(triangle.Vertices[1].AsVector2(), triangle.Vertices[2].AsVector2(), color);
        _display.DrawLine(triangle.Vertices[2].AsVector2(), triangle.Vertices[0].AsVector2(), color);
    }

    public void DrawVertices(Triangle triangle, uint color = 0xFF00FFFF)
    {
        for (var i = 0; i < 3; i++)
        {
            _display.DrawPoint((int)triangle.Vertices[i].X, (int)triangle.Vertices[i].Y, 4, color);
        }
    }

    //https://fgiesen.wordpress.com/2013/02/10/optimizing-the-basic-rasterizer/
    public void DrawFilledTriangle(Triangle triangle, uint color = 0xFF00FFFF)
    {
        var boundingBox = triangle.GetBoundingBox2D();

        _display.DrawBoundingBox(boundingBox);
        
        //Cant find why the ordering becomes wrong here for me,
        //but this rendering techinque uses CCW winding for vertex order 
        var v0 = triangle.Vertices[2].AsVector2(); 
        var v1 = triangle.Vertices[1].AsVector2();
        var v2 = triangle.Vertices[0].AsVector2();
        
        //Triangle setup
        var a01 = v0.Y - v1.Y;
        var a12 = v1.Y - v2.Y;
        var a20 = v2.Y - v0.Y;
        var b01 = v1.X - v0.X;
        var b12 = v2.X - v1.X;
        var b20 = v0.X - v2.X;

        //topleft bias for proper fill
        var bias0 = IsTopLeft(v1, v2) ? 0 : -1;
        var bias1 = IsTopLeft(v2, v0) ? 0 : -1;
        var bias2 = IsTopLeft(v0, v1) ? 0 : -1;
        
        //Barycentric coordinates at top left corner of bounding box, ie starting polong
       var p = new Vector2(boundingBox.MinX, boundingBox.MinY);
        
        var w0_row = TMath.Orientation2D(v1, v2,p) + bias0;
        var w1_row = TMath.Orientation2D(v2, v0,p) + bias1;;
        var w2_row = TMath.Orientation2D(v0, v1,p) + bias2;

        for (p.Y = boundingBox.MinY; p.Y <= boundingBox.MaxY; p.Y++)
        {
            var w0 = w0_row ;
            var w1 = w1_row ;
            var w2 = w2_row ;

            for (p.X = boundingBox.MinX; p.X <= boundingBox.MaxX; p.X++)
            {
                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    _display.DrawPixel((int)p.X, (int)p.Y, color);
                }

                w0 += a12;
                w1 += a20;
                w2 += a01;
            }

            w0_row += b12;
            w1_row += b20;
            w2_row += b01;
        }
        
    }

    bool IsTopLeft(Vector2 v1, Vector2 v2)
    {
        // Top edge: horizontal edge with v2 to the right of v1
        if (v1.Y == v2.Y && v2.X > v1.X) return true;
    
        // Left edge: v2 is above v1 (smaller Y)
        if (v2.Y < v1.Y) return true;
    
        return false;
    }

    public void DrawFlatShadedTriangle(Triangle triangle, ulong baseColor, Vector3 lightDirection)
    {
    }

    public List<Triangle> CullBackFaces(List<Triangle> triangles, Vector3 cameraDirection)
    {
        var trianglesToCull = new List<Triangle>();
        for (var i = 0; i < triangles.Count; i++)
        {
            var viewAngle = Vector3.Dot(triangles[i].Normal, cameraDirection);
            if (viewAngle < 0)
                trianglesToCull.Add(triangles[i]);
        }

        foreach (var t in trianglesToCull)
            triangles.Remove(t);

        return triangles;
    }
}