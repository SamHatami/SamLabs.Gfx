namespace SamLabs.Gfx.Engine.Components.Grid;

public struct GridComponent:IComponent
{
    public int LinesPerSide { get;}
    public float Spacing { get;}
    
    public GridSnapMode GridSnapMode { get; set; }

    public GridComponent(int linesPerSide, float spacing)
    {
        LinesPerSide = linesPerSide;
        Spacing = spacing;
    }
}
