using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Engine.Components.Flags;

public struct CreateOffsetConstructionPlaneFlag : IComponent
{
    public Plane ReferencePlane;
    public float Offset;
}
