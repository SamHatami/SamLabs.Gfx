using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components.Flags;

public struct GridComponent:IDataComponent
{
    public int LinesPerSide { get;}
    public float Spacing { get;}

    public GridComponent(int linesPerSide, float spacing)
    {
        LinesPerSide = linesPerSide;
        Spacing = spacing;
    }
}