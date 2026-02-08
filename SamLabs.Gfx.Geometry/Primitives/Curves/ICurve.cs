using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry.Primitives.Curves;

public interface ICurve
{
    Vector3 Evaluate(float t);
    Vector3 Tangent(float t);
    Bounds GetBounds { get; }
}

public static class ICurveExtensions
{
    public static Vector3 Normal(this ICurve curve, float t)
    {
        var tangent = curve.Tangent(t);
        var normal = Vector3.Cross(tangent, Vector3.UnitY).Normalized();
        return normal;
    }

    public static Bounds GetBounds(this ICurve curve)
    {
        throw  new NotImplementedException();
    }
}

