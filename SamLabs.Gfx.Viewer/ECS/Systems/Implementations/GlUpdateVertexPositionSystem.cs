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

[RenderPassAttributes.RenderOrder(SystemOrders.PreRenderUpdate)]
public class GlUpdateVertexPositionSystem: RenderSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate;
    public GlUpdateVertexPositionSystem(EntityManager entityManager) : base(entityManager)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var entityIds = ComponentManager.GetEntityIdsForComponentType<GlMeshDataChangedComponent>();
        if (entityIds.IsEmpty) return;
        
        foreach (var entityId in entityIds)
        {
            var glMeshData = ComponentManager.GetComponent<GlMeshDataComponent>(entityId);
            var meshData = ComponentManager.GetComponent<MeshDataComponent>(entityId);
            var selectedVertices = ComponentManager.GetComponent<VertexSelectionComponent>(entityId);
            var startIndex = selectedVertices.SelectedIndices.Min();
            var endIndex = selectedVertices.SelectedIndices.Max();
            var sliceLength = endIndex - startIndex + 1;
            //get a slice of the vertices
            ReadOnlySpan<Vertex> vertices = meshData.Vertices.AsSpan(startIndex, sliceLength);
            
            UpdatePositions(vertices, glMeshData, startIndex);
            
            ComponentManager.RemoveComponentFromEntity<GlMeshDataChangedComponent>(entityId);
        }
    }

    private void UpdatePositions(ReadOnlySpan<Vertex> vertices, GlMeshDataComponent glMeshData, int startIndex)
    {
        IntPtr byteOffset = (startIndex * SizeOf.Vertex);
        
        GL.BindVertexArray(glMeshData.Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, glMeshData.Vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, byteOffset, vertices.Length * SizeOf.Vertex, vertices);
        GL.BindVertexArray(0);
    }
}