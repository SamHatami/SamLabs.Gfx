using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.Rendering.Passes;

public ref struct RenderContext
{
    public bool ResizeRequested { get; set; }
    public int ObjectHoverId { get; set; }
    public int[] SelectedObjectIds { get; set; }
    public int ViewWidth { get; set; }
    public int ViewHeight { get; set; }
}