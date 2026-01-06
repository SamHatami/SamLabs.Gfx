using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Components.Manipulators; // Need this for the flag

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class ScaleManipulatorBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public ScaleManipulatorBlueprint(ShaderService shaderService, EntityManager entityManager) : base()
    {
        _shaderService = shaderService;
        _entityManager = entityManager;
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

       ComponentManager.SetComponentToEntity(parentManipulatorTransform, parentManipulator.Id);
       
       var manipulatorComponent = new ManipulatorComponent() { Type = ManipulatorType.Scale };
       ComponentManager.SetComponentToEntity(manipulatorComponent, parentManipulator.Id);
       
       var arrowPath = Path.Combine(AppContext.BaseDirectory, "Models", "ScaleArrow.obj");
       var planePath = Path.Combine(AppContext.BaseDirectory, "Models", "ScalePlane.obj");
       var importedArrowMesh = await ModelLoader.LoadObj(arrowPath); 
       var importedPlaneMesh = await ModelLoader.LoadObj(planePath); 
       
       var parentIdComponent = new ParentIdComponent(parentManipulator.Id);
       var manipulatorShader = _shaderService.GetShader("manipulator");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var xAxisEntity = _entityManager.CreateEntity();
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
           IndexCount = importedArrowMesh.Indices.Length,
           VertexCount = importedArrowMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       ComponentManager.SetComponentToEntity(parentIdComponent, xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(transformX, xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(importedArrowMesh, xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(materialX, xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(glArrowMesh, xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), xAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.X), xAxisEntity.Id);

       var yAxisEntity = _entityManager.CreateEntity();
       yAxisEntity.Type = EntityType.Manipulator;
       var transformY = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,10,0),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90f)) 
       };
       var materialY = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(transformY, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(importedArrowMesh, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(materialY, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(glArrowMesh, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Y), yAxisEntity.Id);
       
       
       var zAxisEntity = _entityManager.CreateEntity();
       zAxisEntity.Type = EntityType.Manipulator;
       var transformZ = new TransformComponent
       {
           Scale = scale,
           ParentId = parentManipulator.Id,
           Position = new Vector3(0,0,10),
           Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f))
       };
       var materialZ = new MaterialComponent { Shader = manipulatorShader};
       
       ComponentManager.SetComponentToEntity(parentIdComponent, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(transformZ, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(importedArrowMesh, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(materialZ, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(glArrowMesh, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.Z), zAxisEntity.Id);
       
       var xyPlaneEntity = _entityManager.CreateEntity();
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
           IndexCount = importedPlaneMesh.Indices.Length,
           VertexCount = importedPlaneMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       ComponentManager.SetComponentToEntity(parentIdComponent, xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(transformXY, xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(importedPlaneMesh, xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(materialXY, xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(glPlaneMesh, xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), xyPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XY), xyPlaneEntity.Id);   
       
       var xzPlaneEntity = _entityManager.CreateEntity();
       xzPlaneEntity.Type = EntityType.Manipulator;
       var meshRotation = new Vector3(MathHelper.DegreesToRadians(90f),MathHelper.DegreesToRadians(180f),0f);
       var transformXZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(2,0,2),
           Rotation =  Quaternion.FromEulerAngles(meshRotation) 
       };
       var materialXZ = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(transformXZ, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(importedPlaneMesh, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(materialXZ, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(glPlaneMesh, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.XZ), xzPlaneEntity.Id);          
       
       var yzPlaneEntity = _entityManager.CreateEntity();
       yzPlaneEntity.Type = EntityType.Manipulator;
       var transformYZ = new TransformComponent
       {
           ParentId = parentManipulator.Id,
           Position =  new Vector3(0,2,2),
           Rotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90f)) 
       };
       var materialYZ = new MaterialComponent { Shader = manipulatorShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(transformYZ, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(importedPlaneMesh, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(materialYZ, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(glPlaneMesh, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new ManipulatorChildComponent(parentManipulator.Id, ManipulatorAxis.YZ), yzPlaneEntity.Id);   
       
    }
}