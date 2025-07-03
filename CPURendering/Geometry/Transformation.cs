using System.Numerics;
using CPURendering.Enums;

namespace CPURendering.Geometry;

public static class Transformation
{
    public static Matrix4x4 Rotate(float rotRad, Axis rotRadAxis)
    {
        var rotRadMatrix = Matrix4x4.Identity;

        switch (rotRadAxis)
        {
            case Axis.X:
                rotRadMatrix[2, 2] = (float)Math.Cos(rotRad);
                rotRadMatrix[2, 3] = (float)Math.Sin(rotRad);
                rotRadMatrix[3, 2] = (float)-Math.Sin(rotRad);
                rotRadMatrix[3, 3] = (float)Math.Cos(rotRad);
                break;
            case Axis.Y:
                rotRadMatrix[0, 0] *= (float)Math.Cos(rotRad);
                rotRadMatrix[0, 2] *= (float)-Math.Sin(rotRad);
                rotRadMatrix[2, 0] *= (float)Math.Sin(rotRad);
                rotRadMatrix[2, 2] *= (float)Math.Cos(rotRad);
                break;
            case Axis.Z:
                rotRadMatrix[0, 0] *= (float)Math.Cos(rotRad);
                rotRadMatrix[1, 0] *= (float)Math.Sin(rotRad);
                rotRadMatrix[0, 1] *= (float)-Math.Sin(rotRad);
                rotRadMatrix[1, 1] *= (float)Math.Cos(rotRad);
                break;
               
        }

        return rotRadMatrix;
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