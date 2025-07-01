using System.Numerics;

namespace CPURendering;

public struct Mesh
{
    public Vector3[] Vertices;
    public Face[] Faces;
    public Vector3 Rotation;
}