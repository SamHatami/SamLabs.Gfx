namespace SamLabs.Gfx.Viewer.UI.Gizmos;

//Render to small quad in one of the corners of the viewport?

public class ViewGizmo
{
    public int RenderQuadWidth { get; } = 10;
    public int RenderQuadHeight { get; } = 10;

    public ViewGizmo(int renderQuadWidth, int renderQuadHeight)
    {
        RenderQuadWidth = renderQuadWidth;
        RenderQuadHeight = renderQuadHeight;
    }
}