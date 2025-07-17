using System.Numerics;
using CPURendering.Geometry;
using SDL3;

namespace CPURendering;

public class Rasterizer
{
    private readonly Display _display;

    public Rasterizer(Display display)
    {
        _display = display;
    }
    
    public void DrawTriangleEdges(Triangle triangle, uint color = 0xFFFFFFFF )
    {
        _display.DrawLine(triangle.Vertices[0].AsVector2(), triangle.Vertices[1].AsVector2(), color);
        _display.DrawLine(triangle.Vertices[1].AsVector2(), triangle.Vertices[2].AsVector2(), color);
        _display.DrawLine(triangle.Vertices[2].AsVector2(), triangle.Vertices[0].AsVector2(), color);
    }
}