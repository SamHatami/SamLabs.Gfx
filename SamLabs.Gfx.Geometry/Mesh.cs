namespace SamLabs.Gfx.Geometry;

public class Mesh
{
    public Vertex[]? Vertices { get; }
    public int[]? Indices { get; }

    public Mesh(Vertex[] vertices, int[] indices)
    {
        Vertices = vertices;
        Indices = indices;

    }
       
}