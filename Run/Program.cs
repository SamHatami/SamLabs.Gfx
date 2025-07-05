// See https://aka.ms/new-console-template for more information

using System.Numerics;
using CPURendering;
using CPURendering.Geometry;
using Run;

Console.WriteLine("Hello, World!");

var display = new Display();
var running = true;

display.InitializeWindow(800, 600);
var screen = new Screen(800, 600, 60);
var project = new Projection(screen,0.1f,100f);
var cubePoint = TestGeometries.GetUnitCubePointCloud(5);
var cube  = new Vector4[cubePoint.Length];

for (int i = 0; i < cubePoint.Length; i++)
    cube[i] = new Vector4(cubePoint[i], 1);

var scaleVector = new Vector3(50f, 50f, 500f);
var scaleMatrix = Transformation.Scale(scaleVector);

for (int i = 0; i < cube.Length; i++)
    cube[i] = Vector4.Transform(cube[i], scaleMatrix); 

for (int i = 0; i < cube.Length; i++)
    cube[i] = Transformation.Project(project.ProjectionMatrix,cube[i]); 

while (InputHandler.HandleInput())
{
    for (int i = 0; i < cube.Length; i++)
        display.DrawPoint((int)cube[i].X, (int)cube[i].Y, 2, 0xFF0000FF);

    display.Render();
}