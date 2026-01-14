namespace SamLabs.Gfx.Engine.Components.Flags;

public struct GridComponent:IComponent
{
    public int LinesPerSide { get;}
    public float Spacing { get;}

    public GridComponent(int linesPerSide, float spacing)
    {
        LinesPerSide = linesPerSide;
        Spacing = spacing;
    }
}
