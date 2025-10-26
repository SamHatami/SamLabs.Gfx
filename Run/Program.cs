// See https://aka.ms/new-console-template for more information

using System.Numerics;
using System.Reflection;
using CPURendering;
using CPURendering.Display;
using CPURendering.Enums;
using CPURendering.Geometry;
using CPURendering.Import;
using CPURendering.View;
using Run;
using SDL3;

//break this out to managers and handlers

// var convexHull = new ConvexHull2D();
//
// convexHull.GenerateGrahamScan();


var display = new Display();
var running = true;
var height = 800;
var width = 1200;
int SSAA = 1;
display.InitializeWindow(width, height, samplingSize:SSAA,sampling:false);
var rasteriser = new Rasterizer(display);
var screen = new Screen(width, height, (float)Math.PI / 3);
var camera = new Camera();
var project = new Projection(screen, 0.1f, 100);

float deltaTime = 0;
var prevFrameTime = 0;

Mesh cube = ImportTeaPot();
Mesh sphere = CreateTestSphere();

Mesh CreateTestSphere()
{
    Vector3[] nodes = FibonacciSprial.GetNodes(50, 1);
    
    var mesh = new Mesh();
    
    mesh.Vertices = nodes;
    
    return mesh;
}

SDL.CaptureMouse(true);

float mouseX = 0;
float mouseY = 0;
var transformedVertices = new Vector4[sphere.Vertices.Length];
var transformedSphereVertices = new Vector4[sphere.Vertices.Length];

while (InputHandler.HandleInput()) //Check alternative way instead of a main loop, events?
{
    camera.Target = new Vector3(0, 0, -10f);
    deltaTime = ((int)SDL.GetTicks() - prevFrameTime) / 1000.0f;
    prevFrameTime = (int)SDL.GetTicks();
    //RenderMesh(cube, [RenderMode.Triangles], true);
    //RenderMesh(cube, [RenderMode.Triangles], true);
    RenderSphere();
    display.Render();
}

Mesh ImportTestCube()
{
    var exePath = Assembly.GetExecutingAssembly().Location;

    var fullPath = Path.Combine(Path.GetDirectoryName(exePath) ?? string.Empty, "Geometry\\Cube.obj");

    return MeshReader.ReadFromFile(fullPath) ?? new Mesh();
}

Mesh ImportTeaPot()
{
    var exePath = Assembly.GetExecutingAssembly().Location;

    var fullPath = Path.Combine(Path.GetDirectoryName(exePath) ?? string.Empty, "Geometry\\TeaPot.obj");

    return MeshReader.ReadFromFile(fullPath) ?? new Mesh();
}

void RenderMesh(Mesh mesh, RenderMode[] renderModes, bool renderFaces = false)
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

    var translations = new Vector3(-0f, 0f, -5f);

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
    }
    
    //transform the face normals aswell

    if(renderFaces)
    {
        for (int i = 0; i < cube.Faces.Length; i++)
        {
            var faceNormal = cube.VertexNormals[cube.Faces[i].VertNormalIndices[0]];
            var transformedFaceNormal = Vector4.Transform(faceNormal.ToVector4(), worldMatrix);
            transformedFaceNormal = Vector4.Transform(transformedFaceNormal, viewMatrix);
            cube.Faces[i].Normal = Vector3.Normalize(transformedFaceNormal.AsVector3());
        }
    }
    
    //all is in screenspace here now

    for (int i = 0; i < cube.Vertices.Length; i++)
    {
        transformedVertices[i] = Projection.Project(project.ProjectionMatrix, transformedVertices[i]); //Screen Space
        
    }
    
    for (int i = 0; i < transformedVertices.Length; i++)
    {
        transformedVertices[i].X = (transformedVertices[i].X * width / 2) * SSAA + (width / 2) * SSAA;
        transformedVertices[i].Y = -(transformedVertices[i].Y * height / 2) * SSAA + (height / 2) * SSAA;
    }


    for (int i = 0; i < transformedVertices.Length; i++)
    {
        display.DrawPoint((int)transformedVertices[i].X, (int)transformedVertices[i].Y, 4, 0xFF0000FF);
    }
    
    if(!renderFaces) return;
    
    //Render triangles
    List<Triangle> triangles = []; //make this global for the entire "scene"
    for (var i = 0; i < cube.Faces.Length; i++)
    {
        var vertices = new Vector4[3];
        vertices[0] = transformedVertices[cube.Faces[i].VertIndices[0]];
        vertices[1] = transformedVertices[cube.Faces[i].VertIndices[1]];
        vertices[2] = transformedVertices[cube.Faces[i].VertIndices[2]];

        var tri = new Triangle(vertices);
        tri.FaceNormal = cube.Faces[i].Normal;

        triangles.Add(tri);       
    }
    
    triangles = rasteriser.CullBackFaces(triangles, camera.Direction);

    foreach (var t in triangles)
    {
        uint color = GetShade(t.FaceNormal, Vector3.Normalize(new Vector3(-1,-.5f,-1)), 0xFFFFFFFF);
        rasteriser.DrawFilledTriangle(t, color);
        rasteriser.DrawTriangleEdges(t);
    }
    
}

void RenderSphere(bool renderFaces = false)
{
    // --- The loop below goes for each object in the scene ---

    sphere.Rotation = sphere.Rotation with { Y = sphere.Rotation.Y + 0.15f * deltaTime };
    sphere.Rotation = sphere.Rotation with { X = sphere.Rotation.X + 0.15f * deltaTime };

    //Gather world matrix and world transformations
    var worldMatrix = Matrix4x4.Identity;

    var scaleVector = new Vector3(10, 10, 10);
    var scaleMatrix = Matrix4x4.CreateScale(scaleVector);

    var rotationsMatrixX = Matrix4x4.CreateRotationX(sphere.Rotation.X);
    var rotationsMatrixY = Matrix4x4.CreateRotationY(sphere.Rotation.Y);
    var rotationsMatrixZ = Matrix4x4.CreateRotationZ(sphere.Rotation.Z);

    var translations = new Vector3(-0f, 0f, -25f);

    var translateMatrix = Matrix4x4.CreateTranslation(translations);

    worldMatrix = Matrix4x4.Multiply(worldMatrix, scaleMatrix);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixX);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixY);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, rotationsMatrixZ);
    worldMatrix = Matrix4x4.Multiply(worldMatrix, translateMatrix);

    var viewMatrix = camera.LookAt(camera.Target);
    //Create copy 

    for (int i = 0; i < sphere.Vertices.Length; i++)
    {
        transformedSphereVertices[i] = Vector4.Transform(sphere.Vertices[i].ToVector4(), worldMatrix); //World Space
        transformedSphereVertices[i] = Vector4.Transform(transformedSphereVertices[i], viewMatrix); //View Space
    }
    
    //all is in screenspace here now

    for (int i = 0; i < sphere.Vertices.Length; i++)
    {
        transformedSphereVertices[i] = Projection.Project(project.ProjectionMatrix, transformedSphereVertices[i]); //Screen Space
        
    }
    
    for (int i = 0; i < transformedSphereVertices.Length; i++)
    {
        transformedSphereVertices[i].X = (transformedSphereVertices[i].X * width / 2) * SSAA + (width / 2) * SSAA;
        transformedSphereVertices[i].Y = -(transformedSphereVertices[i].Y * height / 2) * SSAA + (height / 2) * SSAA;
    }


    for (int i = 0; i < 3; i++)
    {
        display.DrawPoint((int)transformedSphereVertices[i].X, (int)transformedSphereVertices[i].Y, 4, 0xFF0000FF);
    }
    
    for (int i = 0; i < transformedSphereVertices.Length-2; i++)
    {
        display.DrawLine(transformedSphereVertices[i].AsVector2(), transformedSphereVertices[i+2].AsVector2(),  0x0000FFFF);
    }
    
}


uint GetShade(Vector3 normal, Vector3 lightDirection, uint color)
{
    var intensity = Math.Clamp(Vector3.Dot(normal, lightDirection),0,1); 

    byte originalRed = (byte)(color >> 24);
    byte originalGreen = (byte)(color >> 16);
    byte originalBlue = (byte)(color >> 8);

    // Modify color components based on intensity
    byte red = ClampToByte(originalRed * intensity);
    byte green = ClampToByte(originalGreen * intensity);
    byte blue = ClampToByte(originalBlue * intensity);
    

    // Calculate the color of the pixel
    return (uint)((red << 24) | (green << 16) | (blue << 8) | 0xFF);
}

byte ClampToByte(float value)
{
    return (byte)Math.Clamp(value, 0, 255);
}
