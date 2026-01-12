using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Components.Construction;

public struct ConstructionPlaneDataComponent:IDataComponent
{
    Plane Plane { get; set; }
}