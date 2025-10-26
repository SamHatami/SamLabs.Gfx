using System.Numerics;
using CPURendering;

namespace Run;

public class ConvexHull2D
{
    public Vector2[] HullPoints { get; private set; } 
    public Vector2[] InnerPoints { get; private set; }

    private float _espilon = 0.0001f;
    public void GenerateGrahamScan()
    {
        var quantity = (int)Random.Shared.NextInt64(30,70);
        var points = CreatePoints(quantity, 100, 100);
        
        //Vertical and horizontal sorting
        points = points.OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

        Dictionary<Vector2, float> angles = new Dictionary<Vector2, float>();
        //Polar sorting from first points
        angles[points[0]] = 0;
        for (int i = 1; i < points.Length; i++)
        {
            var v = Vector2.Dot(Vector2.Normalize(points[i]), Vector2.Normalize(points[0]));
            var angleRad = MathF.Acos(v); 
            
            //if there are two points with the same angle, save the one furthers away
            var similarPoint = angles.FirstOrDefault(a => Math.Abs(a.Value - angleRad) < _espilon);
            if (similarPoint.Key != default)
            {
                var d1 = Vector2.Distance(points[i], points[0]);
                var d2 = Vector2.Distance(similarPoint.Key, points[0]);
                if (!(d1 > d2)) continue;
                
                angles[points[i]] = angleRad;
                angles.Remove(similarPoint.Key);
            }
            else
            {
                angles[points[i]] = angleRad;
            }
            
        }
        
        var sortedPoints = angles.OrderBy(p => angles.Values).Select(p => p.Key).ToArray();
        
        
        for (int i = 0; i < sortedPoints.Length-2; i++)
        {
            var v1 =points[i+1] - points[i];
            var v2 = points[i+2] - points[i+1];
            if (v1.Cross(v2) > 0)
            {
                //Dont add
                Console.WriteLine("Dont add");
            }
        }
    }  
    
    //Described in 
    // public void GenerateGrahamScan2();
    
    private Vector2[] CreatePoints(int quantity, int sizeX, int sizeY)
    {
        var points = new Vector2[quantity];
        for (int i = 0; i < quantity; i++)
        {
            points[i] = new Vector2(Random.Shared.Next(sizeX), Random.Shared.Next(sizeY));
        }
        
        return points;
    }
    
}