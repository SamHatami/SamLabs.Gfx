namespace SamLabs.Gfx.Engine.Components.Structural;

public struct TrussNodeComponent : IComponent
{
    public TrussNodeComponent()
    {
    }

    public List<int> ConnectedMemberIds { get; set; } = [];
    public int ConnectedMemberCount => ConnectedMemberIds.Count;
}
