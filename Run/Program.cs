// See https://aka.ms/new-console-template for more information

using CPURendering;
using CPURendering.Geometry;
using Run;

Console.WriteLine("Hello, World!");

var display = new Display();
var running = true;

display.InitializeWindow(800, 600);
var screen = new Screen(800, 600, 60);
var project = new Projection(screen,0,10);
var cubePoint = TestGeometries.GetUnitCubePointCloud(5);

var projectedCube  = Transformation.Project(project,) 
while (InputHandler.HandleInput())
{
    
    display.DrawGrid(0x3333333);
    display.DrawRect(100, 100, 100, 100, 0xFFFFFFFF);
    display.Render();
}