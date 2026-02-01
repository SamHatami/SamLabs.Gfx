namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussNodeComponent:IComponent
{
    public TrussNodeComponent()
    {
    }

    public List<int> ConnectedBarIds { get; set; }= [];
    public int ConnectedBarCount => ConnectedBarIds.Count;
}