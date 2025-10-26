
using System.Numerics;

namespace SamLabs.Gfx.Viewer.Geometry;

public class Mesh
{
    private Vertex[] _vertices;
    private Face[] _faces;
    private Edge[] _edges;
    
    public Mesh()
    {
        
    }
    
    public Mesh(Vertex[] vertices, Face[] faces, Edge[] edges)
    {
        _vertices = vertices;
        _faces = faces;
        _edges = edges;
    }
    
    
}

public class Edge
{
    private Vertex[] _vertices;
    
    
}

public class Face
{
    private Vertex[] _vertices;
    
}

public class Vertex
{
    private Vector3 _position;
    private Vector3 _normal;
    private Vector2 _texCoord;
}