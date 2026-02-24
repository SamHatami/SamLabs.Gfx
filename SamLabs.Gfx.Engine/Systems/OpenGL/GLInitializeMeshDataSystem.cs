using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.MeshUpload)]
public class MeshUploadSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.MeshUpload;
    private readonly IGraphicsBackend _graphicsBackend;

    public MeshUploadSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IGraphicsBackend graphicsBackend)
        : base(entityRegistry, componentRegistry)
    {
        _graphicsBackend = graphicsBackend;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        var newMeshes = EntityRegistry.Query.With<MeshDataComponent>().With<GlMeshDataComponent>().Without<GpuMeshHandleComponent>().Get();
        foreach (var id in newMeshes)
            UploadNew(id);

        var dirtyMeshes = EntityRegistry.Query.With<MeshDataComponent>().With<GpuMeshHandleComponent>().Get();
        foreach (var id in dirtyMeshes)
        {
            ref var h = ref ComponentRegistry.GetComponent<GpuMeshHandleComponent>(id);
            if (!h.IsDirty) continue;

            var descriptor = BuildUploadDescriptor(id);
            _graphicsBackend.UpdateMesh(h.Handle, descriptor);
            h.IsDirty = false;
        }

        var removed = ComponentRegistry.GetEntityIdsForComponentType<GlMeshRemoved>();
        foreach (var id in removed)
        {
            if (ComponentRegistry.HasComponent<GpuMeshHandleComponent>(id))
            {
                var handle = ComponentRegistry.GetComponent<GpuMeshHandleComponent>(id).Handle;
                _graphicsBackend.DeleteMesh(handle);
                ComponentRegistry.RemoveComponentFromEntity<GpuMeshHandleComponent>(id);
            }
            ComponentRegistry.RemoveComponentFromEntity<GlMeshRemoved>(id);
        }
    }

    private void UploadNew(int entityId)
    {
        var descriptor = BuildUploadDescriptor(entityId);
        var handle = _graphicsBackend.UploadMesh(descriptor);
        ComponentRegistry.SetComponentToEntity(new GpuMeshHandleComponent { Handle = handle, IsDirty = false }, entityId);
        ComponentRegistry.RemoveComponentFromEntity<CreateGlMeshDataFlag>(entityId);
    }

    private MeshUploadDescriptor BuildUploadDescriptor(int entityId)
    {
        ref var meshData = ref ComponentRegistry.GetComponent<MeshDataComponent>(entityId);
        ref var glMeshData = ref ComponentRegistry.GetComponent<GlMeshDataComponent>(entityId);

        var flat = FlattenVertices(meshData.Vertices);
        var tri = meshData.TriangleIndices?.Select(i => (uint)i).ToArray();
        var edge = meshData.EdgeIndices?.Select(i => (uint)i).ToArray();

        glMeshData.VertexCount = meshData.Vertices?.Length ?? 0;
        glMeshData.IndexCount = meshData.TriangleIndices?.Length ?? 0;
        glMeshData.EdgeIndexCount = meshData.EdgeIndices?.Length ?? 0;

        return new MeshUploadDescriptor(flat, tri, edge, 8);
    }

    private static float[] FlattenVertices(SamLabs.Gfx.Geometry.Mesh.Vertex[] vertices)
    {
        if (vertices == null || vertices.Length == 0)
            return Array.Empty<float>();

        var data = new float[vertices.Length * 8];
        var d = 0;
        foreach (var vertex in vertices)
        {
            data[d++] = vertex.Position.X;
            data[d++] = vertex.Position.Y;
            data[d++] = vertex.Position.Z;
            data[d++] = vertex.Normal.X;
            data[d++] = vertex.Normal.Y;
            data[d++] = vertex.Normal.Z;
            data[d++] = vertex.TextureCoordinate.X;
            data[d++] = vertex.TextureCoordinate.Y;
        }

        return data;
    }
}
