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

        scaleMatrix.M11 = scale.X;
        scaleMatrix.M22 = scale.Y;
        scaleMatrix.M33 = scale.Z;

        return Matrix4x4.CreateScale(scale);

        return scaleMatrix;
    }

    public static Matrix4x4 Translate(Vector3 translation)
    {
        var translationMatrix = Matrix4x4.Identity;

        translationMatrix.M41 = translation.X;
        translationMatrix.M42 = translation.Y;
        translationMatrix.M43 = translation.Z;

        return Matrix4x4.CreateTranslation(translation);

    }
    
    public static Vector4 Project(Matrix4x4 projection, Vector4 v)
    {
        //Transform the position vector by the projectionmatrix
        v = Vector4.Transform(v, projection);

        //perform the perspective divide by the original z-value, stored in w.
        if (v.W == 0.0f) return v;
        v.X /= v.W;
        v.Y /= v.W;
        v.Z /= v.W;
        return v;
    }
}