namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussBarComponent : IComponent
{
    public int StartNodeEntityId;
    public int EndNodeEntityId;
    public float Thickness;
}