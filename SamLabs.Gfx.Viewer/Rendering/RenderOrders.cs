namespace SamLabs.Gfx.Viewer.Rendering;

public static class RenderOrders
{
    public const int Init = -2;
    public const int PreRenderUpdate = -1;
    
    public const int GizmoPickingStart = 0;
    public const int GizmoPickingRender = 1;
    public const int GizmoPickingEnd = 2;
    public const int GizmoPickingRead = 3;
    
    public const int MainStart = 100;
    public const int MainRender = 101;

    public const int GizmoTransform = 101;
    public const int GizmoRender = 102;
    public const int HighlightSelectionRender = 103;
    public const int MainEnd = 199;

    
    public const int CleanUp = 9999;
}