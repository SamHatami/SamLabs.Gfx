using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Entities.Blueprints.Manipulators;

public class RotateManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;

    public RotateManipulatorBlueprint(ShaderService shaderService, EntityRegistry entityRegistry) : base()
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
    }

    public override string Name { get; } = EntityNames.RotateManipulator;
    public override async void Build(Entity parentManipulator, MeshDataComponent meshData = default)
    {
        parentManipulator.Type = EntityType.Manipulator;
        var scale = new Vector3(1f, 1f, 1f);
        
       // --- 1. Setup Parent Manipulator Entity ---
       var parentManipulatorTransform = new TransformComponent
       {
           Scale = new Vector3(1f,1f, 1f),
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.Identity 
       };

       ComponentRegistry.SetComponentToEntity(parentManipulatorTransform, parentManipulator.Id);
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Rotate };
       ComponentRegistry.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var rotatePath = Path.Combine(AppContext.BaseDirectory, "Models", "Rotate.obj");
       var importedRotateMesh = await ModelLoader.LoadObj(rotatePath); 
       
       var parentIdComponent = new ParentIdComponent(parentManipulator.Id);
       var manipulatorShader = _shaderService.GetShader("manipulator");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var rotateX = _entityRegistry.CreateEntity();
       rotateX.Type = EntityType.Manipulator;
       var transformX = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(0,0,0),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90f)) 
           
       };
       var materialX = new MaterialComponent { Shader = manipulatorShader };
       var glRotateMesh = new GlMeshDataComponent()
       {
           IsManipulator = true,
           IndexCount = importedRotateMesh.TriangleIndices.Length,
           VertexCount = importedRotateMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       ComponentRegistry.SetComponentToEntity(parentIdComponent, rotateX.Id);
       ComponentRegistry.SetComponentToEntity(transformX, rotateX.Id);
       ComponentRegistry.SetComponentToEntity(importedRotateMesh, rotateX.Id);
       ComponentRegistry.SetComponentToEntity(materialX, rotateX.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateX.Id);
       ComponentRegistry.SetComponentToEntity(glRotateMesh, rotateX.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateX.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), rotateX.Id);


       var rotateY = _entityRegistry.CreateEntity();
       rotateY.Type = EntityType.Manipulator;
       var meshRotation = new Vector3(MathHelper.DegreesToRadians(-90f),0,MathHelper.DegreesToRadians(180f));
       var transformY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.FromEulerAngles(meshRotation)
       };
       var materialY = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, rotateY.Id);
       ComponentRegistry.SetComponentToEntity(transformY, rotateY.Id);
       ComponentRegistry.SetComponentToEntity(importedRotateMesh, rotateY.Id);
       ComponentRegistry.SetComponentToEntity(materialY, rotateY.Id);
       ComponentRegistry.SetComponentToEntity(glRotateMesh, rotateY.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateY.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateY.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), rotateY.Id);
       
       var rotateZ = _entityRegistry.CreateEntity();
       rotateZ.Type = EntityType.Manipulator;
       meshRotation = new Vector3(0,0,MathHelper.DegreesToRadians(-90f));
       var transformZ = new TransformComponent
       {
           Scale = scale,
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.FromEulerAngles(meshRotation)
       };
       var materialZ = new MaterialComponent { Shader = manipulatorShader};
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(transformZ, rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(importedRotateMesh, rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(materialZ, rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(glRotateMesh, rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateZ.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), rotateZ.Id);
       
    }
}