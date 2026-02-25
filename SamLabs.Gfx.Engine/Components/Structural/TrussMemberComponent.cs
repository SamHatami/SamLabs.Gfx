namespace SamLabs.Gfx.Engine.Components.Structural;

/// <summary>
/// DEPRECATED: Use FrameMemberComponent instead.
/// This component has been replaced by the Frame architecture system.
/// </summary>
[Obsolete("Use FrameMemberComponent instead", true)]
public struct TrussBarComponent : IComponent
{
    public int StartNodeEntityId;
    public int EndNodeEntityId;
    public float Thickness { get; set; }
    public float Length { get; set; }
    
    public int MaterialId;
    public int ProfileId;
}