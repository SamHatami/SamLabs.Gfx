using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Rendering.Engine;

// Need this for the flag

namespace SamLabs.Gfx.Engine.Entities.Blueprints.Manipulators;

public class TranslateManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;

    public TranslateManipulatorBlueprint(ShaderService shaderService, EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public override string Name { get; } = EntityNames.TranslateManipulator;
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
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Translate };
       _componentRegistry.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var importedArrowMesh = await ModelLoader.LoadObjFromResource("Arrow.obj"); 
       var importedPlaneMesh = await ModelLoader.LoadObjFromResource("TranslatePlane.obj"); 
       
       var parentIdComponent = new ParentIdComponent(parentManipulator.Id);
       var manipulatorShader = _shaderService.GetShader("manipulator");
       // var highlightShader = _shader_service.GetShader("Highlight");
       
       var xAxisEntity = _entityRegistry.CreateEntity();
       xAxisEntity.Type = EntityType.Manipulator;
       var transformX = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(10,0,0),
       };
       var materialX = new MaterialComponent { Shader = manipulatorShader };
       var glArrowMesh = new GlMeshDataComponent()
       {
           IsManipulator = true,
           IndexCount = importedArrowMesh.TriangleIndices.Length,
           VertexCount = importedArrowMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       _componentRegistry.SetComponentToEntity(parentIdComponent, xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(transformX, xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(importedArrowMesh, xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(materialX, xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(glArrowMesh, xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), xAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), xAxisEntity.Id);


       var yAxisEntity = _entityRegistry.CreateEntity();
       yAxisEntity.Type = EntityType.Manipulator;
       var transformY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,10,0),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90f)) 
       };
       var materialY = new MaterialComponent { Shader = manipulatorShader };
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(transformY, yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(importedArrowMesh, yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(materialY, yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(glArrowMesh, yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), yAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), yAxisEntity.Id);
       
       
       var zAxisEntity = _entityRegistry.CreateEntity();
       zAxisEntity.Type = EntityType.Manipulator;
       var transformZ = new TransformComponent
       {
           Scale = scale,
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,10),
           Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f))
       };
       var materialZ = new MaterialComponent { Shader = manipulatorShader};
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(transformZ, zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(importedArrowMesh, zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(materialZ, zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(glArrowMesh, zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), zAxisEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), zAxisEntity.Id);
       
       //  
       // --- 6. Plane Entities (Optional but recommended for Translate Manipulator) ---

       var xyPlaneEntity = _entityRegistry.CreateEntity();
       xyPlaneEntity.Type = EntityType.Manipulator;
       var transformXY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(2,2,0)
       };
       var materialXY = new MaterialComponent { Shader = manipulatorShader };
       var glPlaneMesh = new GlMeshDataComponent()
       {
           IsManipulator = true,
           IndexCount = importedPlaneMesh.TriangleIndices.Length,
           VertexCount = importedPlaneMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       _componentRegistry.SetComponentToEntity(parentIdComponent, xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(transformXY, xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(importedPlaneMesh, xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(materialXY, xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(glPlaneMesh, xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), xyPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XY), xyPlaneEntity.Id);   
       
       
       var xzPlaneEntity = _entityRegistry.CreateEntity();
       xzPlaneEntity.Type = EntityType.Manipulator;
       var transformXZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(2,0,2),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(90f)) 
       };
       var materialXZ = new MaterialComponent { Shader = manipulatorShader };
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(transformXZ, xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(importedPlaneMesh, xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(materialXZ, xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(glPlaneMesh, xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), xzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XZ), xzPlaneEntity.Id);          
       
       var yzPlaneEntity = _entityRegistry.CreateEntity();
       yzPlaneEntity.Type = EntityType.Manipulator;
       var transformYZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(0,2,2),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f)) 
       };
       var materialYZ = new MaterialComponent { Shader = manipulatorShader };
       
       _componentRegistry.SetComponentToEntity(parentIdComponent, yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(transformYZ, yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(importedPlaneMesh, yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(materialYZ, yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(glPlaneMesh, yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), yzPlaneEntity.Id);
       _componentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.YZ), yzPlaneEntity.Id);   
       
    }
}