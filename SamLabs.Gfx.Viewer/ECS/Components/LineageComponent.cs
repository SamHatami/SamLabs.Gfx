using SamLabs.Gfx.Viewer.ECS.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct LineageComponent : IDataComponent
{
    public int ParentId { get; set; }
}