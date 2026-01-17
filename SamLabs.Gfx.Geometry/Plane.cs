using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry;

//This is a helper struct for raycasting and creating construction planes
public struct Plane
{
    public Vector3 Origin { get; }
    public Vector3 Normal { get; }
    
    public Vector3 Tangent { get; }
    
    public Vector3 Bitangent { get; }
    
    public Plane(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Normal = Vector3.Normalize(direction);

        if (Math.Abs(Normal.X) > 0.99f)
            Tangent = Vector3.Cross(Normal, Vector3.UnitY);
        else
            Tangent = Vector3.Cross(Normal, Vector3.UnitX);

        Tangent = Vector3.Normalize(Tangent);
        Bitangent = Vector3.Normalize(Vector3.Cross(Normal, Tangent));
    }

    public static Plane From3Points(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var v1 = p2 - p1;
        var v2 = p3 - p1;
        var normal = Vector3.Cross(v1, v2);
        return new Plane(p1, normal);
    }

    public static Plane From2Lines(Vector3 p1, Vector3 d1, Vector3 p2, Vector3 d2)
    {
        var normal = Vector3.Cross(d1, d2);
        // We use p1 as origin. If lines don't cross, this is still a plane containing first line and parallel to second.
        return new Plane(p1, normal);
    }
}

public static class PlaneExtensions
{
    public static bool RayCast(this Plane plane, Ray ray, out float t)
    {
        t = 0f;

        var denom = Vector3.Dot(plane.Normal, ray.Direction);

        // parallel check
        if (Math.Abs(denom) < 1e-6f)
            return false;

        t = Vector3.Dot(plane.Normal, plane.Origin - ray.Origin) / denom;

        if (t < 0f)
            return false;

        return true;
    }
    
}