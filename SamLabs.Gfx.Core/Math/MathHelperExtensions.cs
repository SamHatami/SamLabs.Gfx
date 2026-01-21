
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Core.Math;

public static class MathExtensions
{
    public static float Tolerance { get; set; } = 1e-5f;
    public static float ToRadians(this float degrees) => degrees * (float)System.Math.PI / 180f;
    
    public static float ToDegrees(this float radians) => radians * 180 / (float)System.Math.PI;
    
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) value = min;
        else if (value > max) value = max;
        
        return value;
    }
    
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
    
    public static float EaseInOutCubic(float t) => t * t * (3f - 2f * t);
    
    public static Vector3 ExtractEulerAngles(Quaternion q)
    {
        var x = MathF.Atan2(2 * (q.W * q.X + q.Y * q.Z), 1 - 2 * (q.X * q.X + q.Y * q.Y));
        var y = MathF.Asin(Clamp(2 * (q.W * q.Y - q.Z * q.X), -1f, 1f));
        var z = MathF.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (q.Y * q.Y + q.Z * q.Z));
        
        return new Vector3(
            MathHelper.RadiansToDegrees(x),
            MathHelper.RadiansToDegrees(y),
            MathHelper.RadiansToDegrees(z)
        );
    }
    
    public static Quaternion QuaternionFromEulerAngles(Vector3 eulerDegrees)
    {
        var x = MathHelper.DegreesToRadians(eulerDegrees.X);
        var y = MathHelper.DegreesToRadians(eulerDegrees.Y);
        var z = MathHelper.DegreesToRadians(eulerDegrees.Z);
        
        return Quaternion.FromEulerAngles(x, y, z);
    }
}