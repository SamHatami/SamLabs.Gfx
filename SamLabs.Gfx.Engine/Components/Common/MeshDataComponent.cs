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
    //materialId
    
}
