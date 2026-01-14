namespace SamLabs.Gfx.Engine.Components.Common;

public readonly struct ParentIdComponent(int parentId) : IComponent
{
    public int ParentId { get;} = parentId;
}
