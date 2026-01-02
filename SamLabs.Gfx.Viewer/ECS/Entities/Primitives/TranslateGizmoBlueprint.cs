using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.ECS.Components.Flags; // Need this for the flag

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class TranslateGizmoBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public TranslateGizmoBlueprint(ShaderService shaderService, EntityManager entityManager) : base()
    {
        _shaderService = shaderService;
        _entityManager = entityManager;
    }

    public override string Name { get; } = EntityNames.TranslateGizmo;
    public override async void Build(Entity parentGizmo, MeshDataComponent meshData = default)
    {
        parentGizmo.Type = EntityType.Gizmo;
        var scale = new Vector3(1f, 1f, 1f);
        
       // --- 1. Setup Parent Gizmo Entity ---
       var parentGizmoTransform = new TransformComponent
       {
           Scale = new Vector3(1f,1f, 1f),
           ParentId = parentGizmo.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.Identity 
       };

       ComponentManager.SetComponentToEntity(parentGizmoTransform, parentGizmo.Id);
       
       var gizmoComponent = new GizmoComponent() { Type = GizmoType.Translate };
       ComponentManager.SetComponentToEntity(gizmoComponent, parentGizmo.Id);
       
       var arrowPath = Path.Combine(AppContext.BaseDirectory, "Models", "Arrow.obj");
       var planePath = Path.Combine(AppContext.BaseDirectory, "Models", "TranslatePlane.obj");
       var importedArrowMesh = await ModelLoader.LoadObj(arrowPath); 
       var importedPlaneMesh = await ModelLoader.LoadObj(planePath); 
       
       var parentIdComponent = new ParentIdComponent(parentGizmo.Id);
       var gizmoShader = _shaderService.GetShader("gizmo");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var xAxisEntity = _entityManager.CreateEntity();
       xAxisEntity.Type = EntityType.Gizmo;
       var transformX = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition =  new Vector3(10,0,0),
       };
       var materialX = new MaterialComponent { Shader = gizmoShader };
       var glArrowMesh = new GlMeshDataComponent()
       {
           IsGizmo = true,
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
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.X), xAxisEntity.Id);


       var yAxisEntity = _entityManager.CreateEntity();
       yAxisEntity.Type = EntityType.Gizmo;
       var transformY = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition = new Vector3(0,10,0),
           LocalRotation =  Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90f)) 
       };
       var materialY = new MaterialComponent { Shader = gizmoShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(transformY, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(importedArrowMesh, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(materialY, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(glArrowMesh, yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), yAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.Y), yAxisEntity.Id);
       
       
       var zAxisEntity = _entityManager.CreateEntity();
       zAxisEntity.Type = EntityType.Gizmo;
       var transformZ = new TransformComponent
       {
           LocalScale = scale,
           ParentId = parentGizmo.Id,
           LocalPosition = new Vector3(0,0,10),
           LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f))
       };
       var materialZ = new MaterialComponent { Shader = gizmoShader};
       
       ComponentManager.SetComponentToEntity(parentIdComponent, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(transformZ, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(importedArrowMesh, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(materialZ, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(glArrowMesh, zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), zAxisEntity.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.Z), zAxisEntity.Id);
       
       //  
       // --- 6. Plane Entities (Optional but recommended for Translate Gizmo) ---

       var xyPlaneEntity = _entityManager.CreateEntity();
       xyPlaneEntity.Type = EntityType.Gizmo;
       var transformXY = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition =  new Vector3(2,2,0)
       };
       var materialXY = new MaterialComponent { Shader = gizmoShader };
       var glPlaneMesh = new GlMeshDataComponent()
       {
           IsGizmo = true,
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
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.XY), xyPlaneEntity.Id);   
       
       
       var xzPlaneEntity = _entityManager.CreateEntity();
       xzPlaneEntity.Type = EntityType.Gizmo;
       var transformXZ = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition =  new Vector3(2,0,2),
           LocalRotation =  Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(90f)) 
       };
       var materialXZ = new MaterialComponent { Shader = gizmoShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(transformXZ, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(importedPlaneMesh, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(materialXZ, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(glPlaneMesh, xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), xzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.XZ), xzPlaneEntity.Id);          
       
       var yzPlaneEntity = _entityManager.CreateEntity();
       yzPlaneEntity.Type = EntityType.Gizmo;
       var transformYZ = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition =  new Vector3(0,2,2),
           LocalRotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f)) 
       };
       var materialYZ = new MaterialComponent { Shader = gizmoShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(transformYZ, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(importedPlaneMesh, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(materialYZ, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(glPlaneMesh, yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), yzPlaneEntity.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.YZ), yzPlaneEntity.Id);   
       
    }
}