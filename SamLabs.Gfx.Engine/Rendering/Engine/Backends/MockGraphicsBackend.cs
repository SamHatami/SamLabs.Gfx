using Matrix4x4 = System.Numerics.Matrix4x4;
using NumericsVector3 = System.Numerics.Vector3;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public sealed class MockGraphicsBackend : IGraphicsBackend
{
    private int _nextHandle;
    public void Initialize() { }
    public void Shutdown() { }
    public unsafe GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor) => new(++_nextHandle);
    public void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor) { }
    public void DeleteMesh(GpuMeshHandle handle) { }
    public unsafe void DrawMesh(GpuMeshHandle handle, DrawFlags flags) { }
    public ShaderHandle GetShader(string name) => new(++_nextHandle);
    public void UseShader(ShaderHandle handle) { }
    public void SetUniformInt(ShaderHandle shader, string name, int value) { }
    public void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value) { }
    public FrameBufferHandle CreateFrameBuffer(int width, int height, bool isPicking) => new(++_nextHandle);
    public void ResizeFrameBuffer(FrameBufferHandle handle, int width, int height) { }
    public void BindFrameBuffer(FrameBufferHandle handle) { }
    public void UnbindFrameBuffer() { }
    public void ClearFrameBuffer(FrameBufferHandle handle) { }
    public void BeginPickingPass(FrameBufferHandle handle) { }
    public void EndPickingPass() { }
    public PickResult ReadPickPixel(int x, int y) => PickResult.Empty;
    public void SetWireframe(bool enabled) { }
    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in NumericsVector3 cameraPos) { }
    public void SetViewport(int x, int y, int width, int height) { }
    public void BeginMainRenderPass(int frameBufferId, int viewWidth, int viewHeight) { }
    public void EndMainRenderPass() { }
    public void BeginDepthPass() { }
    public void EndDepthPass() { }
}
