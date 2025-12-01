using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

/// <summary>
/// Contains OpenGL data for a mesh
/// </summary>
public struct GlMeshDataComponent : IDataComponent
{
    public int Vao;
    public int Vbo;
    public int Ebo;
    public int VertexCount;
    public PrimitiveType PrimitiveType;

    public GlMeshDataComponent(int vertexCount)
    {
        VertexCount = vertexCount;
    }
}