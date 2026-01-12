using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using SamLabs.Gfx.Geometry.Mesh;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

[RenderPassAttributes.RenderOrder(SystemOrders.PreRenderUpdate)]
public class GlUpdateVertexPositionSystem: RenderSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate;
    public GlUpdateVertexPositionSystem(EntityRegistry entityRegistry) : base(entityRegistry)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        var entityIds = ComponentRegistry.GetEntityIdsForComponentType<GlMeshDataChangedComponent>();
        if (entityIds.IsEmpty) return;
        
        foreach (var entityId in entityIds)
        {
            var glMeshData = ComponentRegistry.GetComponent<GlMeshDataComponent>(entityId);
            var meshData = ComponentRegistry.GetComponent<MeshDataComponent>(entityId);
            var selectedVertices = ComponentRegistry.GetComponent<VertexSelectionComponent>(entityId);
            var startIndex = selectedVertices.SelectedIndices.Min();
            var endIndex = selectedVertices.SelectedIndices.Max();
            var sliceLength = endIndex - startIndex + 1;
            //get a slice of the vertices
            ReadOnlySpan<Vertex> vertices = meshData.Vertices.AsSpan(startIndex, sliceLength);
            
            UpdatePositions(vertices, glMeshData, startIndex);
            
            ComponentRegistry.RemoveComponentFromEntity<GlMeshDataChangedComponent>(entityId);
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