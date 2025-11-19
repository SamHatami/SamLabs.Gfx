using SamLabs.Gfx.Viewer.ECS.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct DimensionComponent : IDataComponent
{
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
}