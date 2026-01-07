using SamLabs.Gfx.Geometry.Mesh;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;
/// <summary>
/// Contains data for a mesh
/// </summary>
public struct MeshDataComponent: IDataComponent
{
    public Vertex[] Vertices { get; set; }
    public Edge[] Edges { get; set; }
    public Face[] Faces { get; set; }
    
    public int[] TriangleIndices { get; set; }
    public int[] EdgeIndices { get; set; }
    public string Name { get; set; }
    //materialId
    
}