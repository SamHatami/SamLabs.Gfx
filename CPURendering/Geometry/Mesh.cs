using System.Collections.Specialized;
using System.Numerics;

namespace CPURendering.Geometry;

public struct Mesh
{
    public string Name { get; set;}
    public Vector3[] Vertices { get; set;}
    public TextureCoordinate[] TextureCoordinates { get; set;}
    public Face[] Faces { get; set;}
    public Vector3 Rotation { get; set;}
    public Vector3 Scale { get; set; }
    public Vector3 Position { get; set;}
    public Vector3 PivotPoint { get; set;}

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
    
    public Mesh Clone()
    {
        return new Mesh(Vertices, TextureCoordinates, Faces, Rotation, Scale, Position, PivotPoint, Name);
    }
}