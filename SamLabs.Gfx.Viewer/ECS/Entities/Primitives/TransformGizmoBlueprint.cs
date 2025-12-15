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

public class TransformGizmoBlueprint:EntityBlueprint
{
    private readonly ComponentManager _componentManager;
    private readonly ShaderService _shaderService;
    private readonly EntityManager _entityManager;

    public TransformGizmoBlueprint(ComponentManager componentManager, ShaderService shaderService, EntityManager entityManager) : base(componentManager)
    {
        _componentManager = componentManager;
        _shaderService = shaderService;
        _entityManager = entityManager;
    }

    public override string Name { get; } = EntityNames.TransformGizmo;
    public override async void Build(Entity entity, MeshDataComponent meshData = default)
    {
        
        var scale = new Vector3(1f, 1f, 1f);
        
       // --- 1. Setup Parent Gizmo Entity ---
       var transformComponent = new TransformComponent
       {
           Scale = new Vector3(1f,1f, 1f),
           ParentId = entity.Id,
           Position = new Vector3(0,0,0),
           Rotation = Quaternion.Identity 
       };

       _componentManager.SetComponentToEntity(transformComponent, entity.Id);
       
       var gizmoComponent = new GizmoComponent() { Type = GizmoType.Translate };
       _componentManager.SetComponentToEntity(gizmoComponent, entity.Id);
       
       var modelPath = Path.Combine(AppContext.BaseDirectory, "Models", "Arrow.obj");
       var importedArrowMesh = await ModelLoader.LoadObj(modelPath); 
       
       var parentIdComponent = new ParentIdComponent(entity.Id);
       var gizmoShader = _shaderService.GetShader("gizmo");
       // var highlightShader = _shaderService.GetShader("Highlight");
       
       var xAxisEntity = _entityManager.CreateEntity();
       var transformX = new TransformComponent
       {
           Scale = scale,
           ParentId = entity.Id,
           Position = new Vector3(10,0,0),
           Rotation = Quaternion.Identity 
       };
       var materialX = new MaterialComponent { Shader = gizmoShader };
       var glArrowMesh = new GlMeshDataComponent()
       {
           IsGizmo = true,
           IndexCount = importedArrowMesh.Indices.Length,
           VertexCount = importedArrowMesh.Vertices.Length,
           PrimitiveType = PrimitiveType.Triangles
       };
       _componentManager.SetComponentToEntity(parentIdComponent, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(transformX, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(importedArrowMesh, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(materialX, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), xAxisEntity.Id);
       _componentManager.SetComponentToEntity(glArrowMesh, xAxisEntity.Id);
       _componentManager.SetComponentToEntity(new SelectableDataComponent(), xAxisEntity.Id);


       var yAxisEntity = _entityManager.CreateEntity();
       var transformY = new TransformComponent
       {
           Scale = scale,
           ParentId = entity.Id,
           Position = new Vector3(10,0,0),
           Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90f)) 
       };
       var materialY = new MaterialComponent { Shader = gizmoShader };
       
       _componentManager.SetComponentToEntity(parentIdComponent, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(transformY, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(importedArrowMesh, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(materialY, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(glArrowMesh, yAxisEntity.Id);
       _componentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), yAxisEntity.Id);
       _componentManager.SetComponentToEntity(new SelectableDataComponent(), yAxisEntity.Id);
       
       
       var zAxisEntity = _entityManager.CreateEntity();
       var transformZ = new TransformComponent
       {
           Scale = scale,
           ParentId = entity.Id,
           Position = new Vector3(10,0,0),
           Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90f))
       };
       var materialZ = new MaterialComponent { Shader = gizmoShader};
       
       _componentManager.SetComponentToEntity(parentIdComponent, zAxisEntity.Id);
       _componentManager.SetComponentToEntity(transformZ, zAxisEntity.Id);
       _componentManager.SetComponentToEntity(importedArrowMesh, zAxisEntity.Id);
       _componentManager.SetComponentToEntity(materialZ, zAxisEntity.Id);
       _componentManager.SetComponentToEntity(glArrowMesh, zAxisEntity.Id);
       _componentManager.SetComponentToEntity(new CreateGlMeshDataFlag(), zAxisEntity.Id);
       _componentManager.SetComponentToEntity(new SelectableDataComponent(), zAxisEntity.Id);

        
       // --- 6. Plane Entities (Optional but recommended for Translate Gizmo) ---
       // The planes allow movement in two axes (e.g., XY plane for moving along the floor)
       // This geometry is usually a small, transparent square.
       
       // Example Plane (XY)
       // var xyPlaneEntity = _entityManager.CreateEntity();
       // ... setup components for the plane geometry here ...
    }
}