using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.GL;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Manipulators;

public class RotateManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;

    public RotateManipulatorBlueprint(ShaderService shaderService, EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
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

       _componentRegistry.SetComponentToEntity(parentManipulatorTransform, parentManipulator.Id);
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Rotate };
       _componentRegistry.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var importedRotateMesh = await ModelLoader.LoadObjFromResource("Rotate.obj"); 
       
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
       _componentRegistry.SetComponentToEntity(parentIdComponent, rotateX.Id);
       _componentRegistry.SetComponentToEntity(transformX, rotateX.Id);
       _componentRegistry.SetComponentToEntity(importedRotateMesh, rotateX.Id);
       _componentRegistry.SetComponentToEntity(materialX, rotateX.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateX.Id);
       _componentRegistry.SetComponentToEntity(glRotateMesh, rotateX.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateX.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), rotateX.Id);


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
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, rotateY.Id);
       _componentRegistry.SetComponentToEntity(transformY, rotateY.Id);
       _componentRegistry.SetComponentToEntity(importedRotateMesh, rotateY.Id);
       _componentRegistry.SetComponentToEntity(materialY, rotateY.Id);
       _componentRegistry.SetComponentToEntity(glRotateMesh, rotateY.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateY.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateY.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), rotateY.Id);
       
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
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, rotateZ.Id);
       _componentRegistry.SetComponentToEntity(transformZ, rotateZ.Id);
       _componentRegistry.SetComponentToEntity(importedRotateMesh, rotateZ.Id);
       _componentRegistry.SetComponentToEntity(materialZ, rotateZ.Id);
       _componentRegistry.SetComponentToEntity(glRotateMesh, rotateZ.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateZ.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), rotateZ.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), rotateZ.Id);
       
    }
}