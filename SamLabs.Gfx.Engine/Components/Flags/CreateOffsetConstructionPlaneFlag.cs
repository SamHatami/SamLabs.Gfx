using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Components.Construction;

public struct CreateOffsetConstructionPlaneFlag : IComponent
{
    public Plane ReferencePlane;
    public float Offset;
}
