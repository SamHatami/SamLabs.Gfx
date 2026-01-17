using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Flags.GL;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

[RenderPassAttributes.RenderOrder(SystemOrders.Init)]
public class GLInitializeMeshDataSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.Init;
    private readonly IComponentRegistry _componentRegistry;
    public GLInitializeMeshDataSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry,componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var glMeshDataEntities = _componentRegistry.GetEntityIdsForComponentType<CreateGlMeshDataFlag>();
        if (glMeshDataEntities.IsEmpty) return;

        for (var i = 0; i < glMeshDataEntities.Length; i++)
        {
            ref var glMeshData = ref _componentRegistry.GetComponent<GlMeshDataComponent>(glMeshDataEntities[i]);
            ref var meshData = ref _componentRegistry.GetComponent<MeshDataComponent>(glMeshDataEntities[i]);

            CreateGlMeshData(ref glMeshData, ref meshData);
            
            _componentRegistry.RemoveComponentFromEntity<CreateGlMeshDataFlag>(glMeshDataEntities[i]);
        }
    }

    //TBD transient mesh data for dynamic draw (?)
    private void CreateGlMeshData(ref GlMeshDataComponent glMeshData, ref MeshDataComponent meshData)
    {
        glMeshData.Vao = GL.GenVertexArray();
        glMeshData.Vbo = GL.GenBuffer();

        GL.BindVertexArray(glMeshData.Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, glMeshData.Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, glMeshData.VertexCount * SizeOf.Vertex, meshData.Vertices,
            BufferUsage.StaticDraw);

        SetupVertexAttributes();
        
        if( meshData.TriangleIndices.Length > 0)
            IndexVertices(ref glMeshData, ref meshData);
        if (meshData.EdgeIndices != null && meshData.EdgeIndices.Length > 0)
            IndexEdges(ref glMeshData, meshData.EdgeIndices);
        else
            glMeshData.Ebo = 0;
        
        GL.BindVertexArray(0);
    }

    private void IndexVertices(ref GlMeshDataComponent glMeshData, ref MeshDataComponent meshData)
    {
        glMeshData.Ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, meshData.TriangleIndices.Length * sizeof(uint),
            meshData.TriangleIndices, BufferUsage.StaticDraw);
    }
    
    private void IndexEdges(ref GlMeshDataComponent glMeshData, int[] edgeIndices)
    {
        glMeshData.EdgeEbo = GL.GenBuffer();
        glMeshData.EdgeIndexCount = edgeIndices.Length;

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.EdgeEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, edgeIndices.Length * sizeof(uint),
            edgeIndices, BufferUsage.StaticDraw);
    }

    private void SetupVertexAttributes()
    {
        // Position
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 0);

        // Normal
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 3 * sizeof(float));

        // TexCoord
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 6 * sizeof(float));
    }
}