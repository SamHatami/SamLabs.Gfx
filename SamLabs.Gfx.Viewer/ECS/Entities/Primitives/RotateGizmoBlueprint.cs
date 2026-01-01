using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.ECS.Components.Flags; // Need this for the flag

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class RotateGizmoBlueprint:EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public RotateGizmoBlueprint(ShaderService shaderService, EntityManager entityManager) : base()
    {
        _shaderService = shaderService;
        _entityManager = entityManager;
    }

    public override string Name { get; } = EntityNames.RotateGizmo;
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
       
       var gizmoComponent = new GizmoComponent() { Type = GizmoType.Rotate };
       ComponentManager.SetComponentToEntity(gizmoComponent, parentGizmo.Id);
       
       var rotatePath = Path.Combine(AppContext.BaseDirectory, "Models", "Rotate.obj");
       var importedRotateMesh = await ModelLoader.LoadObj(rotatePath); 
       
       var parentIdComponent = new ParentIdComponent(parentGizmo.Id);
       var gizmoShader = _shaderService.GetShader("gizmo");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var rotateX = _entityManager.CreateEntity();
       rotateX.Type = EntityType.Gizmo;
       var transformX = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition =  new Vector3(0,0,0),
           LocalRotation =  Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90f)) 
           
       };
       var materialX = new MaterialComponent { Shader = gizmoShader };
       var glRotateMesh = new GlMeshDataComponent()
       {
           IsGizmo = true,
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
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.X), rotateX.Id);


       var rotateY = _entityManager.CreateEntity();
       rotateY.Type = EntityType.Gizmo;
       var meshRotation = new Vector3(MathHelper.DegreesToRadians(90f),0,MathHelper.DegreesToRadians(270f));
       var transformY = new TransformComponent
       {
           ParentId = parentGizmo.Id,
           LocalPosition = new Vector3(0,0,0),
           LocalRotation = Quaternion.FromEulerAngles(meshRotation)
       };
       var materialY = new MaterialComponent { Shader = gizmoShader };
       
       ComponentManager.SetComponentToEntity(parentIdComponent, rotateY.Id);
       ComponentManager.SetComponentToEntity(transformY, rotateY.Id);
       ComponentManager.SetComponentToEntity(importedRotateMesh, rotateY.Id);
       ComponentManager.SetComponentToEntity(materialY, rotateY.Id);
       ComponentManager.SetComponentToEntity(glRotateMesh, rotateY.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateY.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), rotateY.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.Y), rotateY.Id);
       
       
       var rotateZ = _entityManager.CreateEntity();
       rotateZ.Type = EntityType.Gizmo;
       var transformZ = new TransformComponent
       {
           LocalScale = scale,
           ParentId = parentGizmo.Id,
           LocalPosition = new Vector3(0,0,0),
           LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(180f))
       };
       var materialZ = new MaterialComponent { Shader = gizmoShader};
       
       ComponentManager.SetComponentToEntity(parentIdComponent, rotateZ.Id);
       ComponentManager.SetComponentToEntity(transformZ, rotateZ.Id);
       ComponentManager.SetComponentToEntity(importedRotateMesh, rotateZ.Id);
       ComponentManager.SetComponentToEntity(materialZ, rotateZ.Id);
       ComponentManager.SetComponentToEntity(glRotateMesh, rotateZ.Id);
       ComponentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), rotateZ.Id);
       ComponentManager.SetComponentToEntity(new SelectableDataComponent(), rotateZ.Id);
       ComponentManager.SetComponentToEntity(new GizmoChildComponent(parentGizmo.Id, GizmoAxis.Z), rotateZ.Id);
       
    }
}