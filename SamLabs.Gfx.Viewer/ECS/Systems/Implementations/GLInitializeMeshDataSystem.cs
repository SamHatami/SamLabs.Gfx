using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(RenderOrders.Init)]
public class GLInitializeMeshDataSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.Init;
    private readonly ComponentManager _componentManager;

    public GLInitializeMeshDataSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var glMeshDataEntities = _componentManager.GetEntityIdsForComponentType<CreateGlMeshDataFlag>();
        if (glMeshDataEntities.IsEmpty) return;

        for (var i = 0; i < glMeshDataEntities.Length; i++)
        {
            ref var glMeshData = ref _componentManager.GetComponent<GlMeshDataComponent>(glMeshDataEntities[i]);
            ref var meshData = ref _componentManager.GetComponent<MeshDataComponent>(glMeshDataEntities[i]);

            CreateGlMeshData(ref glMeshData, ref meshData);
            
            _componentManager.RemoveComponentFromEntity<CreateGlMeshDataFlag>(glMeshDataEntities[i]);
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
        
        if( meshData.Indices.Length > 0)
            IndexVertices(ref glMeshData, ref meshData);
        else
            glMeshData.Ebo = 0;
        
        GL.BindVertexArray(0);
    }

    private void IndexVertices(ref GlMeshDataComponent glMeshData, ref MeshDataComponent meshData)
    {
        glMeshData.Ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, meshData.Indices.Length * sizeof(uint),
            meshData.Indices, BufferUsage.StaticDraw);
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