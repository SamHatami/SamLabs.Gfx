using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry;

public struct Ray
{
    public Vector3 Origin { get; }
    public Vector3 Direction { get; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        this.Origin = origin;
        this.Direction = Vector3.Normalize(direction);
    }
}

public static class RayExtensions
{
    public static Vector3 GetPoint(this Ray ray, float distance)
    {
        return ray.Origin + ray.Direction * distance;
    }
}