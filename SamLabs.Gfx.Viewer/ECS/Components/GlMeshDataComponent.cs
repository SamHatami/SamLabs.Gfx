using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

/// <summary>
/// Contains OpenGL data for a mesh
/// </summary>
public struct GlMeshDataComponent : IDataComponent
{
    public bool IsManipulator { get; set; }
    
    //Vertex buffer
    public int Vao;
    public int Vbo;
    
    //Face buffer
    public int Ebo;
    public int IndexCount { get; set; }
    
    //Edge buffer
    public int EdgeEbo { get; set; }
    public int EdgeIndexCount { get; set; }
    
    public int VertexCount;

    public PrimitiveType PrimitiveType { get; set; }
    public GlMeshDataComponent(int vertexCount)
    {
        VertexCount = vertexCount;
    }

}