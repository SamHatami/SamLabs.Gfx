// See https://aka.ms/new-console-template for more information

using System.Numerics;
using CPURendering;
using CPURendering.Enums;
using CPURendering.Geometry;
using Run;

Console.WriteLine("Hello, World!");

var display = new Display();
var running = true;
var height = 800;
var width = 800;
display.InitializeWindow(width, height);
var screen = new Screen(width, height, (float)Math.PI/3);
var project = new Projection(screen,0.1f,100);
var cubePoint = TestGeometries.GetUnitCubePointCloud();
var cube  = new Vector4[cubePoint.Length];

for (int i = 0; i < cubePoint.Length; i++)
    cube[i] = new Vector4(cubePoint[i], 1);


var worldMatrix = Matrix4x4.Identity;

var scaleVector = new Vector3(10,10,10);
var scaleMatrix = Transformation.Scale(scaleVector);

// var rotationsMatrixX = Transformation.Rotate(0, Axis.X);
// var rotationsMatrixY = Transformation.Rotate(0, Axis.Y);
// var rotationsMatrixZ = Transformation.Rotate(0, Axis.Z);

var translations = new Vector3(width/2, height/2, 0);
var translateMatrix = Transformation.Translate(translations);

worldMatrix = Matrix4x4.Multiply(worldMatrix, scaleMatrix);
// worldMatrix = Matrix4x4.Multiply(rotationsMatrixX, worldMatrix);
// worldMatrix = Matrix4x4.Multiply(rotationsMatrixY, worldMatrix);
// worldMatrix = Matrix4x4.Multiply(rotationsMatrixZ, worldMatrix);
worldMatrix = Matrix4x4.Multiply(worldMatrix, translateMatrix);

for (int i = 0; i < cube.Length; i++)
    cube[i] = Vector4.Transform(cube[i], worldMatrix); 

for (int i = 0; i < cube.Length; i++)
{
    cube[i] = Transformation.Project(project.ProjectionMatrix, cube[i]);
    
}

while (InputHandler.HandleInput())
{
    for (int i = 0; i < cube.Length; i++)
        display.DrawPoint((int)cube[i].X, (int)cube[i].Y, 2, 0xFF0000FF);

    display.Render();
}