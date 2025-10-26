using System.Numerics;

namespace CPURendering;

public static class VectorExtension
{
    /// <summary>
    /// Returns cross product as float.
    /// if > 0 v2 is in counterclockwise polar direction of v1 (left)
    /// if < 0 v2 is in clockwise polare direction of v1 (right)
    /// if 0 they are collinear
    /// </summary>
    public static float Cross(this Vector2 v1, Vector2 v2)
    {
        //arean av parallelogram i rummet
        //Positiv area ger v2 är mellan0 och pi i vinkel mot v1
        //Negativ area ger v2 är mellan pi och 2pi i vinkel mot v1
        return (v1.X * v2.Y - v1.Y * v2.X);
    }
    public static Vector4 ToVector4(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 1f);
}