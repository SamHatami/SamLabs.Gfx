using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Components.Construction;

public struct ConstructionPlaneDataComponent : IComponent
{
    public Plane Plane { get; set; }
    public int? SketchEntityId { get; set; }
}
