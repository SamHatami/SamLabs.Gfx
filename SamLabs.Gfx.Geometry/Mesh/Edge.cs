namespace SamLabs.Gfx.Geometry.Mesh;

public struct Edge
{
    public int Id { get; }
    public int V1 { get; }
    public int V2 { get; }

    public Edge(int v1, int v2, int id)
    {
        V1 = v1;
        V2 = v2;
        Id = id;
    }
    
}