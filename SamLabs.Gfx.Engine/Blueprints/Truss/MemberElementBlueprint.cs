using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Structural;
using SamLabs.Gfx.Engine.Components.Structural.Flags;
using SamLabs.Gfx.Engine.Components.Transform;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Truss;

public class MemberElementBlueprint : EntityBlueprint
{
    private readonly ShaderService _shaderService;
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private const float ScreenPixelSize = 250f;

    private MeshDataComponent _memberMesh;
    private MeshDataComponent _nodeMesh;
    private bool _meshesLoaded;

    public MemberElementBlueprint(ShaderService shaderService, EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        _shaderService = shaderService;
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
    }

    public override string Name { get; } = EntityNames.MemberElement;

    public async Task EnsureMeshesLoaded()
    {
        if (_meshesLoaded) return;
        _memberMesh = await ModelLoader.LoadObjFromResource("CylinderLow8.obj");
        _nodeMesh = await ModelLoader.LoadObjFromResource("GeoSphereLow.Obj");
        _meshesLoaded = true;
    }

    public override async void Build(Entity entity, MeshDataComponent meshData = default)
    {
        entity.Type = EntityType.SceneObject;

        await EnsureMeshesLoaded();

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        foreach (var vertex in _memberMesh.Vertices)
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

        BuildMember(entity, _memberMesh, _nodeMesh, endA, endB);
    }

    public async void BuildAtPositions(Entity entity, Vector3 startPosition, Vector3 endPosition)
    {
        entity.Type = EntityType.SceneObject;
        await EnsureMeshesLoaded();
        BuildMember(entity, _memberMesh, _nodeMesh, startPosition, endPosition);
    }

    /// <summary>Call only after EnsureMeshesLoaded() has been awaited.</summary>
    public void BuildMemberSync(Entity entity, Vector3 startPosition, Vector3 endPosition)
    {
        entity.Type = EntityType.SceneObject;
        BuildMember(entity, _memberMesh, _nodeMesh, startPosition, endPosition);
    }

    private void BuildMember(Entity entity, MeshDataComponent memberMesh, MeshDataComponent nodeMesh, Vector3 endA, Vector3 endB)
    {
        var shader = _shaderService.GetShader("unlit");
        var pickingShader = _shaderService.GetShader("picking");

        var memberMaterial = new MaterialComponent { Shader = shader, PickingShader = pickingShader };
        var memberGlMesh = new GlMeshDataComponent
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = memberMesh.Vertices.Length,
            IndexCount = memberMesh.TriangleIndices.Length
        };

        var screenScale = new ScaleToScreenComponent { Size = new Vector3(ScreenPixelSize, ScreenPixelSize, 1), IsPixelSize = true, LockZ = true };
        _componentRegistry.SetComponentToEntity(new TransformComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(memberMesh, entity.Id);
        _componentRegistry.SetComponentToEntity(memberMaterial, entity.Id);
        _componentRegistry.SetComponentToEntity(memberGlMesh, entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), entity.Id);
        _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(screenScale, entity.Id);

        var startNodeId =CreateEndNode(nodeMesh, entity.Id, endA, shader, pickingShader);
        var endNodeId= CreateEndNode(nodeMesh, entity.Id, endB, shader, pickingShader);
        
        _componentRegistry.SetComponentToEntity(new FrameMemberComponent() {StartNodeEntityId = startNodeId, EndNodeEntityId = endNodeId}, entity.Id);
        
        _componentRegistry.SetComponentToEntity(new NodeMovedFlag { OriginatingMemberId = -1 }, startNodeId);
        _componentRegistry.SetComponentToEntity(new NodeMovedFlag { OriginatingMemberId = -1 }, endNodeId);
    }

    private int CreateEndNode(MeshDataComponent nodeMesh, int connectedMemberId, Vector3 position, GLShader? shader, GLShader? pickingShader)
    {
        var nodeEntity = _entityRegistry.CreateEntity();
        nodeEntity.Type = EntityType.SceneObject;

        var transform = new TransformComponent { Position = position, Scale = Vector3.One, Rotation = Quaternion.Identity };
        var material = new MaterialComponent { Shader = shader, PickingShader = pickingShader };
        var glMesh = new GlMeshDataComponent
        {
            PrimitiveType = PrimitiveType.Triangles,
            VertexCount = nodeMesh.Vertices.Length,
            IndexCount = nodeMesh.TriangleIndices.Length
        };

        var screenScale = new ScaleToScreenComponent { Size = new Vector3(ScreenPixelSize), IsPixelSize = true };
        _componentRegistry.SetComponentToEntity(transform, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(nodeMesh, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(material, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(glMesh, nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(new FrameNodeTag(), nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(new CreateGlMeshDataFlag(), nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(new SelectableDataComponent(), nodeEntity.Id);
        _componentRegistry.SetComponentToEntity(screenScale, nodeEntity.Id);

        return nodeEntity.Id;
    }
}
