// See https://aka.ms/new-console-template for more information

using System.Numerics;
using System.Reflection;
using CPURendering;
using CPURendering.Enums;
using CPURendering.Geometry;
using CPURendering.View;
using Run;
using SDL3;

//break this out to managers and handlers



var display = new Display();
var running = true;
var height = 800;
var width = 1200;
display.InitializeWindow(width, height);
var rasteriser = new Rasterizer(display);
var screen = new Screen(width, height, (float)Math.PI / 3);
var camera = new Camera();
var project = new Projection(screen, 0.1f, 100);

float deltaTime = 0;
var prevFrameTime = 0;

Mesh cube = ImportTestCube();

SDL.CaptureMouse(true);

float mouseX = 0;
float mouseY = 0;
var transformedVertices = new Vector4[cube.Vertices.Length];

while (InputHandler.HandleInput()) //Check alternative way instead of a main loop, events?
{
    camera.Target = new Vector3(0, 0, -10f);
    deltaTime = ((int)SDL.GetTicks() - prevFrameTime) / 1000.0f;
    prevFrameTime = (int)SDL.GetTicks();
    RenderMesh(cube, [RenderMode.Triangles]);
    display.Render();
}

Mesh ImportTestCube()
{
    var exePath = Assembly.GetExecutingAssembly().Location;

    var fullPath = Path.Combine(Path.GetDirectoryName(exePath) ?? string.Empty, "Geometry\\Cube.obj");

    return MeshReader.ReadFromFile(fullPath) ?? new Mesh();
}

void RenderMesh(Mesh mesh, RenderMode[] renderModes)
{
    // --- The loop below goes for each object in the scene ---

    cube.Rotation = cube.Rotation with { Y = cube.Rotation.Y + 0.4f * deltaTime };
    cube.Rotation = cube.Rotation with { X = cube.Rotation.X + 0.2f * deltaTime };

    //Gather world matrix and world transformations
    var worldMatrix = Matrix4x4.Identity;

    var scaleVector = new Vector3(1, 1, 1);
    var scaleMatrix = Matrix4x4.CreateScale(scaleVector);

    var rotationsMatrixX = Matrix4x4.CreateRotationX(cube.Rotation.X);
    var rotationsMatrixY = Matrix4x4.CreateRotationY(cube.Rotation.Y);
    var rotationsMatrixZ = Matrix4x4.CreateRotationZ(cube.Rotation.Z);

    var translations = new Vector3(-1f, 0f, -10f);

    var translateMatrix = Matrix4x4.CreateTranslation(translations);

    worldMatrix = Matrix4x4.Multiply(worldMatrix, scaleMatrix);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixX);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixY);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixZ);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, translateMatrix);

    var viewMatrix = camera.LookAt(camera.Target);
    //Create copy 

    for (int i = 0; i < cube.Vertices.Length; i++)
    {
        transformedVertices[i] = Vector4.Transform(cube.Vertices[i].ToVector4(), worldMatrix); //World Space
        transformedVertices[i] = Vector4.Transform(transformedVertices[i], viewMatrix); //View Space
        transformedVertices[i] = Projection.Project(project.ProjectionMatrix, transformedVertices[i]); //Screen Space
    }
    
    for (int i = 0; i < transformedVertices.Length; i++)
    {
        transformedVertices[i].X = (width / 2) * transformedVertices[i].X + (width / 2);
        transformedVertices[i].Y = -(height / 2) * transformedVertices[i].Y + (height / 2); // Note the Y flip
    }


    //Render triangles
    List<Triangle> triangles = []; //make this global for the entire "scene"
    for (var i = 0; i < cube.Faces.Length; i++)
    {
        var vertices = new Vector4[3];
        vertices[0] = transformedVertices[cube.Faces[i].VertIndices[0]];
        vertices[1] = transformedVertices[cube.Faces[i].VertIndices[1]];
        vertices[2] = transformedVertices[cube.Faces[i].VertIndices[2]];

        var tri = new Triangle(vertices);

        triangles.Add(tri);       
    }
    
    triangles = rasteriser.CullBackFaces(triangles, camera.Direction);

    foreach (var t in triangles)
    {
        rasteriser.DrawFilledTriangle(t);
        //rasteriser.DrawTriangleEdges(t);
        rasteriser.DrawVertices(t,0xFF0000FF);
    }
    
}