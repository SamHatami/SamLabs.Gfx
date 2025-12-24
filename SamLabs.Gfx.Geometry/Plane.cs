using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry;

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


}

public static class PlaneExtensions
{
    public static bool RayCast(this Plane plane, Ray ray, out float ratio)
    {
        var hit = false;
        ratio = 0.0f;       
        
        //p0 point on the plane creating a orthogonal axis to the normal
        //t = ((P0 - RayOrigin) * PlaneNormal)/(RayDirection*PlanNormal)

        return hit;

    }
    
}