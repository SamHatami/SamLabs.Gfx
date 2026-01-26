namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussNodeComponent:IComponent
{
    public int[] ConnectedBarIds;
    public int ConnectedBarCount => ConnectedBarIds.Length;
}