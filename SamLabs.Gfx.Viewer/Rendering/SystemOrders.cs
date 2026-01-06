namespace SamLabs.Gfx.Viewer.Rendering;

public static class SystemOrders
{
    public const int Init = -100;
    public const int PreRenderUpdate = -99;
    public const int ManipulatorSelectionUpdate = 0;
    public const int SelectionUpdate = 1;
    public const int TransformUpdate = 2;
    public const int PickingStart = 100;
    public const int PickingRender = 101;
    public const int PickingEnd = 102;
    
    public const int MainStart = 200;
    public const int MainRender = 201;

    public const int ManipulatorRender = 301;
    public const int HighlightSelectionRender = 302;
    public const int MainEnd = 599;

    
    public const int CleanUp = 9999;
}