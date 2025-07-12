using System.Numerics;

namespace CPURendering.Geometry;

public struct Face
{
    public int A;
    public int B;
    public int C;
    public int UvA;
    public int UvB;
    public int UvC;
    public Vector3 Normal;

    public Face(int a, int b, int c, Vector3 normal)
    {
        A = a;
        B = b;
        C = c;
        Normal = normal;
    }
}