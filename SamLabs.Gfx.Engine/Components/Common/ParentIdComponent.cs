namespace SamLabs.Gfx.Engine.Components.Common;

public readonly struct ParentIdComponent(int parentId) : IDataComponent
{
    public int ParentId { get;} = parentId;
}