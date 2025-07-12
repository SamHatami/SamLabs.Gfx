using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using CPURendering;
using CPURendering.Enums;
using CPURendering.Geometry;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var rotation = Transformation.Rotate(3.14f, Axis.X);;
    }

    [Fact]
    public void CubePoints()
    {
        var cube = TestGeometries.GetUnitCubePointCloud();
    }

    [Fact]
    public void ObjReaderTest()
    {
        var exePath = Assembly.GetExecutingAssembly().Location;
        
        var fullPath = Path.Combine(Path.GetDirectoryName(exePath),"Geometry\\Cube.obj");
        
        var cube = MeshReader.ReadFromFile(fullPath);
    }
}