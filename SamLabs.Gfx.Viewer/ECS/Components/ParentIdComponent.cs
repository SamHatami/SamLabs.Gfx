using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct ParentIdComponent : IDataComponent
{
    public int ParentId { get; set; }
}