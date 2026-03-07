using SamLabs.Gfx.Geometry.Mesh;

namespace SamLabs.Gfx.Engine.Components.Common;
/// <summary>
/// Contains data for a mesh
/// </summary>
public struct MeshDataComponent: IComponent
{
    public Vertex[] Vertices { get; set; }
    public Edge[] Edges { get; set; }
    public Face[] Faces { get; set; }

    public int[] TriangleIndices { get; set; }
    public int[] EdgeIndices { get; set; }
    public string Name { get; set; }

    // Phase 2 logical render metadata (GPU-handle free)
    public int VertexCount { get; set; }
    public int IndexCount { get; set; }
    public int EdgeIndexCount { get; set; }
    public DrawMode DrawMode { get; set; }
    public bool IsManipulator { get; set; }
    public bool IsGrid { get; set; }

    public void RefreshDerivedData()
    {
        VertexCount = Vertices?.Length ?? 0;
        IndexCount = TriangleIndices?.Length ?? 0;
        EdgeIndexCount = EdgeIndices?.Length ?? 0;
        if (DrawMode == default && IndexCount > 0)
            DrawMode = DrawMode.Triangles;
    }

}

