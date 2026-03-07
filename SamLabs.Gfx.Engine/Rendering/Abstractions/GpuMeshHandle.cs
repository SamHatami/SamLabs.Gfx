namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public struct GpuMeshHandle
{
    public int VertexArrayId { get; set; }
    public int VertexBufferId { get; set; }
    public int ElementBufferId { get; set; }
    public int EdgeElementBufferId { get; set; }
}
