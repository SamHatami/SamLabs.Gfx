namespace SamLabs.Gfx.Engine.Components.Structural;

/// <summary>
/// Represents a structural member connecting two frame nodes.
/// </summary>
public struct FrameMemberComponent : IComponent
{
    public int StartNodeEntityId;
    public int EndNodeEntityId;
    public float Length;
    public MemberType Type;
}

/// <summary>
/// Defines the type of structural member.
/// </summary>
public enum MemberType
{
    Truss = 0,
    Beam = 1
}
