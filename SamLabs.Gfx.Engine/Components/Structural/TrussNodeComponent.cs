namespace SamLabs.Gfx.Engine.Components.Structural;

/// <summary>
/// DEPRECATED: Use FrameNodeTag instead.
/// This component has been replaced by the Frame architecture system.
/// </summary>
[Obsolete("Use FrameNodeTag instead", true)]
public struct TrussNodeComponent:IComponent
{
    public TrussNodeComponent()
    {
    }

    public List<int> ConnectedMemberIds { get; set; } = [];
    public int ConnectedMemberCount => ConnectedMemberIds.Count;
}
