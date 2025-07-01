using System.Numerics;

namespace CPURendering.Geometry;

public struct Mesh
{
    public Vector3[] Vertices;
    public Face[] Faces;
    public Vector3 Rotation;
}