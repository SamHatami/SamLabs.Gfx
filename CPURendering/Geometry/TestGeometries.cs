using System.Numerics;

namespace CPURendering.Geometry;

public static class TestGeometries
{
    public static Vector3[] GetUnitCubePointCloud(int density)
    {
        var cubePoints = new Vector3[density*density*density];
        int pointNr = 0;  
        for (int x = 0; x < density; x++)
        { 
            for (int y = 0; y < density; y ++)
            {
                for (int z = 0; z < density; z++)
                {
                    cubePoints[pointNr] = new Vector3(x, y, z);
                    pointNr++;
                }
            }
        }

        return cubePoints;
    }
    
}