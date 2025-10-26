
using OpenTK.Mathematics;

public static class MathExtensions
{
    public static void Clamp(ref float value, float min, float max)
    {
        if (value < min) value = min;
        else if (value > max) value = max;
    }

    public static void Clamp(ref double value, double min, double max)
    {
        if (value < min) value = min;
        else if (value > max) value = max;
    }

    public static void Clamp<T>(ref T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
            value = min;
        else if (value.CompareTo(max) > 0)
            value = max;
    }
}