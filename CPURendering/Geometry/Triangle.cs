using System.Numerics;

namespace CPURendering.Geometry;

public struct Triangle
{
    public Vector4[] Vertices { get; set; } = new Vector4[3];
    private float avarageDepth;

    public Triangle()
    {
        avarageDepth = 0;
    }
}