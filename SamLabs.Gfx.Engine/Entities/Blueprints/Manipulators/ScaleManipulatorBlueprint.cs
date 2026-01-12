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

public class ScaleManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;

    public ScaleManipulatorBlueprint(ShaderService shaderService, EntityRegistry entityRegistry) : base()
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
    }

    public override string Name { get; } = EntityNames.ScaleManipulator;
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
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Scale };
       ComponentRegistry.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var arrowPath = Path.Combine(AppContext.BaseDirectory, "Models", "ScaleArrow.obj");
       var planePath = Path.Combine(AppContext.BaseDirectory, "Models", "ScalePlane.obj");
       var importedArrowMesh = await ModelLoader.LoadObj(arrowPath); 
       var importedPlaneMesh = await ModelLoader.LoadObj(planePath); 
       
       var parentIdComponent = new ParentIdComponent(parentManipulator.Id);
       var manipulatorShader = _shaderService.GetShader("manipulator");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
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
       ComponentRegistry.SetComponentToEntity(parentIdComponent, xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformX, xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedArrowMesh, xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialX, xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(glArrowMesh, xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), xAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), xAxisEntity.Id);

       var yAxisEntity = _entityRegistry.CreateEntity();
       yAxisEntity.Type = EntityType.Manipulator;
       var transformY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,10,0),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90f)) 
       };
       var materialY = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformY, yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedArrowMesh, yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialY, yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(glArrowMesh, yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), yAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), yAxisEntity.Id);
       
       
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
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformZ, zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedArrowMesh, zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialZ, zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(glArrowMesh, zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), zAxisEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), zAxisEntity.Id);
       
       var xyPlaneEntity = _entityRegistry.CreateEntity();
       xyPlaneEntity.Type = EntityType.Manipulator;
       var transformXY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(2,2,0),
           Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(180f)) 
       };
       var materialXY = new MaterialComponent { Shader = manipulatorShader };
       var glPlaneMesh = new GlMeshDataComponent()
       {
           IsManipulator = true,
           IndexCount = importedPlaneMesh.TriangleIndices.Length,
           VertexCount = importedPlaneMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       ComponentRegistry.SetComponentToEntity(parentIdComponent, xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformXY, xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedPlaneMesh, xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialXY, xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(glPlaneMesh, xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), xyPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XY), xyPlaneEntity.Id);   
       
       var xzPlaneEntity = _entityRegistry.CreateEntity();
       xzPlaneEntity.Type = EntityType.Manipulator;
       var meshRotation = new Vector3(MathHelper.DegreesToRadians(90f),MathHelper.DegreesToRadians(180f),0f);
       var transformXZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(2,0,2),
           Rotation =  Quaternion.FromEulerAngles(meshRotation) 
       };
       var materialXZ = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformXZ, xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedPlaneMesh, xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialXZ, xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(glPlaneMesh, xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), xzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XZ), xzPlaneEntity.Id);          
       
       var yzPlaneEntity = _entityRegistry.CreateEntity();
       yzPlaneEntity.Type = EntityType.Manipulator;
       var transformYZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(0,2,2),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90f)) 
       };
       var materialYZ = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentRegistry.SetComponentToEntity(parentIdComponent, yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(transformYZ, yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(importedPlaneMesh, yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(materialYZ, yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(glPlaneMesh, yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new SelectableDataComponent(), yzPlaneEntity.Id);
       ComponentRegistry.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.YZ), yzPlaneEntity.Id);   
       
    }
}