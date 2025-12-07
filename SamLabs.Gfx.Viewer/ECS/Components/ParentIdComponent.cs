using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public readonly struct ParentIdComponent(int parentId) : IDataComponent
{
    public int ParentId { get;} = parentId;
}