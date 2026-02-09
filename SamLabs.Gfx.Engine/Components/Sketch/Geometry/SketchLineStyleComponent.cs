namespace SamLabs.Gfx.Engine.Components.Sketch.Geometry;

/// <summary>
/// Visual style properties for line segment rendering
/// </summary>
public struct SketchLineStyleComponent : ISketchComponent
{
    public float Thickness;
    public uint StyleFlags;    // Bit flags: DASHED = 1<<0, DOTTED = 1<<1, HIDDEN = 1<<2
    public float DashLength;
    public float GapLength;
    public uint Color;         // Packed RGBA (0xAABBGGRR)
    
    public SketchLineStyleComponent()
    {
        Thickness = 2.0f;
        StyleFlags = 0;
        DashLength = 5.0f;
        GapLength = 5.0f;
        Color = 0xFFFFFFFF; // White
    }
    
    public static SketchLineStyleComponent Default => new()
    {
        Thickness = 2.0f,
        StyleFlags = 0,
        DashLength = 5.0f,
        GapLength = 5.0f,
        Color = 0xFFFFFFFF
    };
}

