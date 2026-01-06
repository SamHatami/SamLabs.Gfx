using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators;
using SamLabs.Gfx.Viewer.ECS.Components.Selection;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Blueprints.Manipulators;

public class RotateManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public RotateManipulatorBlueprint(ShaderService shaderService, EntityManager entityManager) : base()
    {
        _shaderService = shaderService;
        _entityManager = entityManager;
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

       ComponentManager.SetComponentToEntity(parentManipulatorTransform, parentManipulator.Id);
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Rotate };
       ComponentManager.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var rotatePath = Path.Combine(AppContext.BaseDirectory, "Models", "Rotate.obj");
       var importedRotateMesh = await ModelLoader.LoadObj(rotatePath); 
       
       var parentIdComponent = new ParentIdComponent(parentManipulator.Id);
       var manipulatorShader = _shaderService.GetShader("manipulator");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var rotateX = _entityManager.CreateEntity();
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
           IndexCount = importedRotateMesh.Indices.Length,
           VertexCount = importedRotateMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       ComponentManager.SetComponentToEntity(parentIdComponent, rotateX.Id);
       ComponentManager.SetComponentToEntity(transformX, rotateX.Id);
       ComponentManager.SetComponentToEntity(importedRotateMesh, rotateX.Id);
       ComponentManager.SetComponentToEntity(materialX, rotateX.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateX.Id);
       ComponentManager.SetComponentToEntity(glRotateMesh, rotateX.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), rotateX.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), rotateX.Id);


       var rotateY = _entityManager.CreateEntity();
       rotateY.Type = EntityType.Manipulator;
       var meshRotation = new Vector3(MathHelper.DegreesToRadians(-90f),0,MathHelper.DegreesToRadians(180f));
       var transformY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.FromEulerAngles(meshRotation)
       };
       var materialY = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, rotateY.Id);
       ComponentManager.SetComponentToEntity(transformY, rotateY.Id);
       ComponentManager.SetComponentToEntity(importedRotateMesh, rotateY.Id);
       ComponentManager.SetComponentToEntity(materialY, rotateY.Id);
       ComponentManager.SetComponentToEntity(glRotateMesh, rotateY.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateY.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), rotateY.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), rotateY.Id);
       
       var rotateZ = _entityManager.CreateEntity();
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
       
       ComponentManager.SetComponentToEntity(parentIdComponent, rotateZ.Id);
       ComponentManager.SetComponentToEntity(transformZ, rotateZ.Id);
       ComponentManager.SetComponentToEntity(importedRotateMesh, rotateZ.Id);
       ComponentManager.SetComponentToEntity(materialZ, rotateZ.Id);
       ComponentManager.SetComponentToEntity(glRotateMesh, rotateZ.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateZ.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), rotateZ.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), rotateZ.Id);
       
    }
}