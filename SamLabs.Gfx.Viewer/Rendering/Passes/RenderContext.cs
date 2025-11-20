using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.Rendering.Passes;

public ref struct RenderContext
{
    public int ObjectHoverId { get; set; }
    public int[] SelectedObjectIds { get; set; }
    public int ViewWidth { get; set; }
    public int ViewHeight { get; set; }
    public Matrix4 ViewMatrix { get; set; }
    public Matrix4 ProjectionMatrix { get; set; }
}