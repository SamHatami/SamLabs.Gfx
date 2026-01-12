using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public ref struct RenderContext
{
    public IViewPort ViewPort { get; set; }
    public bool ResizeRequested { get; set; }
    public bool CameraMoved { get; set; }
    public int[] SelectedObjectIds { get; set; }
    public int HoveredObjectId { get; set; }
    public int ViewWidth { get; set; }
    public int ViewHeight { get; set; }
    public int MainViewFrameBuffer { get; set; }
    public float RenderScaling { get; set; } //platform thing
}