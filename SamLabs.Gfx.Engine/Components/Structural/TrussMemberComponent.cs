namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussMemberComponent : IComponent
{
    public int StartNodeEntityId;
    public int EndNodeEntityId;
    public float Thickness { get; set; }
    public float Length { get; set; }

    public int MaterialId;
    public int ProfileId;
}
