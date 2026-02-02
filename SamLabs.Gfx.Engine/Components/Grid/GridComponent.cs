namespace SamLabs.Gfx.Engine.Components.Grid;

public struct GridComponent:IComponent
{
    public bool Visible { get; set; } = true;
    public int LinesPerSide { get; set; }
    public float GridLineSpacing { get; set; }
    public float MajorLineFrequency { get; set; } = 5;
    
    public float GridSize { get;set; } = 1.0f;
    
    public SnapMode SnapMode { get; set; }

    public bool UpdateRequested { get; set; } = true; //To signal the system to update the grid mesh when created

    public GridComponent(float gridSize, float gridLineSpacing, float majorLineFrequency)
    {
        GridSize = gridSize;
        GridLineSpacing = gridLineSpacing;
        MajorLineFrequency = majorLineFrequency;
    }
}
