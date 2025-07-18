using System.Numerics;

namespace CPURendering.Geometry;

public struct Triangle
{
    public Vector4[] Vertices { get; private set; } = new Vector4[3];
    private float avarageDepth;
    public Vector3 Normal { get; private set; }

    public Triangle(Vector4[] vertices)
    {
        Vertices = vertices;
        GetNormal();
        avarageDepth = 0;
    }

    private void GetNormal()
    {
        var vec1 = (Vertices[1] - Vertices[0]).AsVector3();
        var vec2 = (Vertices[2] - Vertices[0]).AsVector3();
        Normal = Vector3.Normalize(Vector3.Cross(vec1, vec2));
    }
}