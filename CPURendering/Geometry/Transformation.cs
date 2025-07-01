using System.Numerics;

namespace CPURendering.Geometry;

public static class Transformation
{
    public static Matrix4x4 Rotate(Vector4 rotation)
    {
        var rotationMatrix = Matrix4x4.Identity;
    }

    public static Matrix4x4 Scale(Vector3 scale)
    {
        var scaleMatrix = Matrix4x4.Identity;

        scaleMatrix[0, 0] = scale.X;
        scaleMatrix[1, 1] = scale.Y;
        scaleMatrix[2, 2] = scale.Z;

        return scaleMatrix;
    }
}