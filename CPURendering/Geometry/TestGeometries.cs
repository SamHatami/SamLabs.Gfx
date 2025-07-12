using System.Numerics;

namespace CPURendering.Geometry;

public static class TestGeometries
{
    public static Vector3[] GetUnitCubePointCloud()
    {
        var cubePoints = new Vector3[9*9*9];
        int pointNr = 0;  
        for (float x = -1; x < 1; x +=0.25f)
        { 
            for (float y = -1; y < 1; y +=0.25f)
            {
                for (float z = -1; z < 1; z+=0.25f)
                {
                    cubePoints[pointNr] = new Vector3(x, y, z);
                    pointNr++;
                }
            }
        }
        return cubePoints;
    }

    // public static Mesh CreateCubeMesh()
    // {
    //     var cubeMesh = new Mesh();
    //     cubeMesh.Vertices = new Vector3[8]
    //     {
    //         new (-1f, -1f, -1f), 
    //         new (-1f, 1f, -1f),
    //         new (1f, 1f, -1f),
    //         new (1f, -1f, -1f),
    //         new (1f, 1f, 1f),
    //         new (1f, -1f, 1f),
    //         new (-1f, 1f, 1f),
    //         new (-1f, -1f, 1f)
    //     };
    //     
    //     cubeMesh.Faces = new []
    //     {
    //         new Face()
    //     }
    //     
    //     
    // }
    
}