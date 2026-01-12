
namespace SamLabs.Gfx.Core.Math;

public static class MathExtensions
{
    public static float Tolerance { get; set; } = 1e-5f;
    public static float ToRadians(this float degrees) => degrees * (float)System.Math.PI / 180f;
    
    public static float ToDegrees(this float radians) => radians * 180 / (float)System.Math.PI;
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