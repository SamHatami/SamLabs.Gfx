using System.Numerics;

namespace CPURendering.Geometry;

public static class FibonacciSprial
{
    public static Vector3[] GetNodes(int nrOfPoints, int radius)
    {
        var points = new Vector3[nrOfPoints];
        var golderRatio = (1 + MathF.Sqrt(5)) / 2;
        var twoPi = 2 * MathF.PI;
        for (int i = 0; i < nrOfPoints; i++)
        {
            var theta =  twoPi* i / golderRatio;
            var phi = MathF.Acos(1-2*(i+0.5f)/nrOfPoints);
            
            var x = MathF.Cos(theta)*MathF.Sin(phi)*radius;
            var y = MathF.Sin(theta)*MathF.Sin(phi)*radius;
            var z = MathF.Cos(phi)*radius;
            
            points[i] = new Vector3(x, y, z);
        }
        
        return points;
    }
}