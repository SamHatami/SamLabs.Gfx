using System.Numerics;

namespace CPURendering;

public static class Vector4Extension
{
    public static Vector4 ToVector4(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 1f);
}