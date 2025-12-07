namespace SamLabs.Gfx.Viewer.Rendering;

public static class RenderOrders
{
    public const int Init = -2;
    public const int PreRenderUpdate = -1;
    
    public const int GizmoPickingStart = 0;
    public const int GizmoPickingRender = 1;
    public const int GizmoPickingEnd = 2;
    
    public const int MainStart = 100;
    public const int MainRender = 101;
    public const int MainEnd = 102;
    
    public const int GizmoPickingRead = 103;
    public const int GizmoRender = 104;
    public const int HighlightSelectionStart = 200;
    public const int HighlightSelectionRender = 201;
    public const int HighlightSelectionEnd = 202;
    
    public const int CleanUp = 9999;
}