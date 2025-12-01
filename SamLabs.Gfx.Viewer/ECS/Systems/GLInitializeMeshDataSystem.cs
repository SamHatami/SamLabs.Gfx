using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class GLInitializeMeshDataSystem : PreRenderSystem
{
    private readonly ComponentManager _componentManager;

    public GLInitializeMeshDataSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update()
    {
        var glMeshDataEntities = _componentManager.GetEntityIdsFor<CreateGlMeshDataFlag>();
        if (glMeshDataEntities.IsEmpty) return;

        for (var i = 0; i < glMeshDataEntities.Length; i++)
        {
            ref var glMeshData = ref _componentManager.GetComponent<GlMeshDataComponent>(glMeshDataEntities[i]);
            ref var meshData = ref _componentManager.GetComponent<MeshDataComponent>(glMeshDataEntities[i]);

            CreateGlMeshData(ref glMeshData, ref meshData);
            
            _componentManager.RemoveComponentFromEntity<CreateGlMeshDataFlag>(glMeshDataEntities[i]);
        }


        //TBD Implement GL upload, same as in base mesh class
    }

    //System to create GL mesh data

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
        IndexVertices(ref glMeshData, ref meshData);

        GL.BindVertexArray(0);
    }

    private void IndexVertices(ref GlMeshDataComponent glMeshData, ref MeshDataComponent meshData)
    {
        glMeshData.Ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, glMeshData.Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, glMeshData.VertexCount * sizeof(uint),
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