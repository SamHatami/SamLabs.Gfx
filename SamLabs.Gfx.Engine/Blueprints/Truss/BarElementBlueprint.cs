using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.GL;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Truss;

public class BarElementBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private const float ScreenSize = 300f;

    public BarElementBlueprint(ShaderService shaderService, EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public override string Name { get; } = EntityNames.BarElement;

    public override async void Build(Entity entity, MeshDataComponent meshData = default)
    {
        entity.Type = EntityType.SceneObject;

        var bodyMesh = await ModelLoader.LoadObjFromResource("CylinderLow8.obj");
        var nodeMesh = await ModelLoader.LoadObjFromResource("GeoSphereLow.Obj");

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        foreach (var vertex in bodyMesh.Vertices)
        {
            var pos = vertex.Position;
            min.X = MathF.Min(min.X, pos.X);
            min.Y = MathF.Min(min.Y, pos.Y);
            min.Z = MathF.Min(min.Z, pos.Z);
            max.X = MathF.Max(max.X, pos.X);
            max.Y = MathF.Max(max.Y, pos.Y);
            max.Z = MathF.Max(max.Z, pos.Z);
        }

        // Place end nodes at the body's min/max along its longest axis.
        var size = max - min;
        var center = (min + max) * 0.5f;
        var endA = center;
        var endB = center;

        if (size.X >= size.Y && size.X >= size.Z)
        {
            endA.X = min.X;
            endB.X = max.X;
        }
        else if (size.Y >= size.Z)
        {
            endA.Y = min.Y;
            endB.Y = max.Y;
        }
        else
        {
            endA.Z = min.Z;
            endB.Z = max.Z;
        }

        var shader = _shaderService.GetShader("flat");
        var pickingShader = _shaderService.GetShader("picking");


        var bodyMaterial = new MaterialComponent
        {
            Shader = shader,
            PickingShader = pickingShader
        };

        var bodyGlMesh = new GlMeshDataComponent
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = bodyMesh.Vertices.Length,
            IndexCount = bodyMesh.TriangleIndices.Length
        };

        var screenScale = new ScaleToScreenComponent {Size = new Vector3(ScreenSize, ScreenSize, 1), IsPixelSize = true, LockZ = true};
        _componentRegistry.SetComponentToEntity(new TransformComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(bodyMesh, entity.Id);
        _componentRegistry.SetComponentToEntity(bodyMaterial, entity.Id);
        _componentRegistry.SetComponentToEntity(bodyGlMesh, entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(screenScale, entity.Id);

        var parentIdComponent = new ParentIdComponent(entity.Id);
        var endNodeId =CreateEndNode(nodeMesh, parentIdComponent, endA, shader, pickingShader);
        var startNodeId= CreateEndNode(nodeMesh, parentIdComponent, endB, shader, pickingShader);
        
        _componentRegistry.SetComponentToEntity(new TrussBarComponent() {StartNodeEntityId = startNodeId, EndNodeEntityId = endNodeId}, entity.Id);
    }

    private int CreateEndNode(MeshDataComponent nodeMesh, ParentIdComponent parentIdComponent, Vector3 position,
        GLShader? shader, GLShader? pickingShader)
    {
        var nodeEntity = _entityRegistry.CreateEntity();
        nodeEntity.Type = EntityType.SceneObject;

        var transform = new TransformComponent
        {
            ParentId = parentIdComponent.ParentId,
            Position = position,
            Scale = Vector3.One,
            Rotation = Quaternion.Identity
        };

        var material = new MaterialComponent
        {
            Shader = shader,
            PickingShader = pickingShader
        };

        var glMesh = new GlMeshDataComponent
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = nodeMesh.Vertices.Length,
            IndexCount = nodeMesh.TriangleIndices.Length
        };

        var screenScale = new ScaleToScreenComponent {Size = new Vector3(ScreenSize), IsPixelSize = true};
        _componentRegistry.SetComponentToEntity(parentIdComponent, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(transform, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(nodeMesh, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(material, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(glMesh, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(screenScale, nodeEntity.Id);
        
        return nodeEntity.Id;
    }
}
