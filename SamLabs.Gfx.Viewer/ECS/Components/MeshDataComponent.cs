using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;
/// <summary>
/// Contains data for a mesh
/// </summary>
public struct MeshDataComponent: IDataComponent
{
    public Vertex[] Vertices { get; set; }
    public int[] Indices { get; set; }
    public string Name { get; set; }
    //materialId
    
}