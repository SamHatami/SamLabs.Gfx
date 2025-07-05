using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
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
        var cube = TestGeometries.GetUnitCubePointCloud(4);
    } 
}