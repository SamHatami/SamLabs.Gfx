using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags.OpenGl;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.Init)]
public class GLInitializeMeshDataSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.Init;
    private readonly IComponentRegistry _componentRegistry;

    public GLInitializeMeshDataSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(
        entityRegistry, componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
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
        glMeshData.Vao = OpenTK.Graphics.OpenGL.GL.GenVertexArray();
        glMeshData.Vbo = OpenTK.Graphics.OpenGL.GL.GenBuffer();

        OpenTK.Graphics.OpenGL.GL.BindVertexArray(glMeshData.Vao);
        OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, glMeshData.Vbo);
        OpenTK.Graphics.OpenGL.GL.BufferData(BufferTarget.ArrayBuffer, glMeshData.VertexCount * SizeOf.Vertex, meshData.Vertices,
            BufferUsage.StaticDraw);

        SetupVertexAttributes();

        if (meshData.TriangleIndices.Length > 0)
            IndexVertices(ref glMeshData, ref meshData);
        if (meshData.EdgeIndices != null && meshData.EdgeIndices.Length > 0)
            IndexEdges(ref glMeshData, meshData.EdgeIndices);
        // else
        //     glMeshData.Ebo = 0;

        OpenTK.Graphics.OpenGL.GL.BindVertexArray(0);

    }

    private void IndexVertices(ref GlMeshDataComponent glMeshData, ref MeshDataComponent meshData)
    {
        glMeshData.Ebo = OpenTK.Graphics.OpenGL.GL.GenBuffer();
        OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.Ebo);
        OpenTK.Graphics.OpenGL.GL.BufferData(BufferTarget.ElementArrayBuffer, meshData.TriangleIndices.Length * sizeof(uint),
            meshData.TriangleIndices, BufferUsage.StaticDraw);
    }

    private void IndexEdges(ref GlMeshDataComponent glMeshData, int[] edgeIndices)
    {
        glMeshData.EdgeEbo = OpenTK.Graphics.OpenGL.GL.GenBuffer();
        glMeshData.EdgeIndexCount = edgeIndices.Length;

        OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.EdgeEbo);
        OpenTK.Graphics.OpenGL.GL.BufferData(BufferTarget.ElementArrayBuffer, edgeIndices.Length * sizeof(uint),
            edgeIndices, BufferUsage.StaticDraw);
    }

    private void SetupVertexAttributes()
    {
        // Position
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(0);
        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 0);

        // Normal
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(1);
        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 3 * sizeof(float));

        // TexCoord
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(2);
        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
            SizeOf.Vertex, 6 * sizeof(float));
    }
}