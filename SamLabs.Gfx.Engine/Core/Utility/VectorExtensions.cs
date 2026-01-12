using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Core.Utility;

public static class VectorExtensions
{
    public static float[] ToFloatArray(this Vector3[] vector)
    {
        var array = new float[vector.Length * 3];
        for (var i = 0; i < vector.Length; i++)
        {
            array[i * 3] = vector[i].X;
            array[i * 3 + 1] = vector[i].Y;
            array[i * 3 + 2] = vector[i].Z;
        }

        return array;
    }
}
