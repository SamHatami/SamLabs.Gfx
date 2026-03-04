using System.Numerics;
using SamLabs.Gfx.Engine.Components.Selection;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IGraphicsBackend
{
    void Initialize();
    void Shutdown();

    GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor);
    void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor);
    void DeleteMesh(GpuMeshHandle handle);
    void DrawMesh(GpuMeshHandle handle, DrawFlags flags);

    ShaderHandle GetShader(string name);
    void UseShader(ShaderHandle handle);
    void SetUniformInt(ShaderHandle shader, string name, int value);
    void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value);

    FrameBufferHandle CreateFrameBuffer(int width, int height, bool isPicking);
    void ResizeFrameBuffer(FrameBufferHandle handle, int width, int height);
    void BindFrameBuffer(FrameBufferHandle handle);
    void UnbindFrameBuffer();
    void ClearFrameBuffer(FrameBufferHandle handle);

    void BeginPickingPass(FrameBufferHandle handle);
    void EndPickingPass();
    PickResult ReadPickPixel(int x, int y);

    void SetWireframe(bool enabled);
    void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in Vector3 cameraPos);
    void SetViewport(int x, int y, int width, int height);
    void BeginMainRenderPass(int frameBufferId, int viewWidth, int viewHeight);
    void EndMainRenderPass();
    void BeginDepthPass();
    void EndDepthPass();

}

public readonly record struct GpuMeshHandle(int Id);
public readonly record struct ShaderHandle(int Id);
public readonly record struct FrameBufferHandle(int Id);

[Flags]
public enum DrawFlags
{
    None = 0,
    Faces = 1,
    Edges = 2,
    Vertices = 4,
}

public readonly record struct MeshUploadDescriptor(
    float[] Vertices,
    uint[]? Indices = null,
    uint[]? EdgeIndices = null,
    int VertexStride = 3);
