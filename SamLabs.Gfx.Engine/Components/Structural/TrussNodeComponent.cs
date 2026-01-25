namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussNodeComponent
{
    public int[] ConnectedBarIds;
    public int ConnectedBarCount => ConnectedBarIds.Length;
}