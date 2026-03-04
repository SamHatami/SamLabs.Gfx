namespace SamLabs.Gfx.Engine.Components.Common;

/// <summary>
/// Pure ECS mesh metadata. Contains no GPU/backend handle details.
/// </summary>
public struct GlMeshDataComponent : IComponent
{
    public bool IsManipulator { get; set; }
    public bool IsGrid { get; set; }
    public int VertexCount { get; set; }
    public int IndexCount { get; set; }
    public int EdgeIndexCount { get; set; }
    public DrawMode DrawMode { get; set; }
}

public enum DrawMode
{
    Triangles,
    Lines,
    Points
}
