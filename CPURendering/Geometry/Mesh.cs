using System.Collections.Specialized;
using System.Numerics;

namespace CPURendering.Geometry;

public struct Mesh
{
    public string Name;
    public Vector3[] Vertices;
    public TextureCoordinate[] TextureCoordinates;
    public Face[] Faces;
    public Vector3 Rotation;
    public Vector3 Scale;
    public Vector3 Position;
    public Vector3 PivotPoint;

    public Mesh(Vector3[] vertices, TextureCoordinate[] textureCoordinates, Face[] faces, Vector3 rotation, Vector3 scale, Vector3 position, Vector3 pivotPoint, string name = "")
    {
        Vertices = vertices;
        TextureCoordinates = textureCoordinates;
        Faces = faces;
        Rotation = rotation;
        Scale = scale;
        Position = position;
        PivotPoint = pivotPoint;
        Name = name;
    }
}