using System.Numerics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

/// <summary>
/// Transitional backend seam for GPU access. This is the only service the engine should consume for GPU operations.
/// </summary>
public class OpenGLGraphicsBackend : IGraphicsBackend
{
    private readonly ShaderService _shaderService;
    private readonly UniformBufferService _uniformBufferService;
    private readonly FrameBufferService _frameBufferService;
    private readonly Dictionary<int, GLShader> _shaderById = new();
    private readonly Dictionary<int, Components.Common.FrameBufferInfo> _frameBufferById = new();
    private int _nextShaderId = 1;
    private int _nextFrameBufferId = 1;

    public OpenGLGraphicsBackend(
        ShaderService shaderService,
        UniformBufferService uniformBufferService,
        FrameBufferService frameBufferService)
    {
        _shaderService = shaderService;
        _uniformBufferService = uniformBufferService;
        _frameBufferService = frameBufferService;
    }

    public void Initialize()
    {
        _uniformBufferService.RegisterViewProjectionBuffer();
        _uniformBufferService.CreateSingleIntUniform("objectId");
        _shaderService.RegisterShaders();

        foreach (var shader in _shaderService.GetShaderPrograms())
            _uniformBufferService.BindUniformToProgram(shader.ProgramId, UniformBufferService.ViewProjectionName);
    }

    public void Shutdown()
    {
    }

    public GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor) => new(0);
    public void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor) { }
    public void DeleteMesh(GpuMeshHandle handle) { }
    public void DrawMesh(GpuMeshHandle handle, DrawFlags flags) { }

    public ShaderHandle GetShader(string name)
    {
        var shader = _shaderService.GetShader(name);
        if (shader == null)
            return new ShaderHandle(0);

        var handle = new ShaderHandle(_nextShaderId++);
        _shaderById[handle.Id] = shader;
        return handle;
    }

    public void UseShader(ShaderHandle handle)
    {
        if (_shaderById.TryGetValue(handle.Id, out var shader))
            GL.UseProgram(shader.ProgramId);
    }

    public void SetUniformInt(ShaderHandle shader, string name, int value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = GL.GetUniformLocation(glShader.ProgramId, name);
        if (location >= 0)
            GL.Uniform1(location, value);
    }

    public void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = GL.GetUniformLocation(glShader.ProgramId, name);
        if (location < 0) return;

        var matrix = new Matrix4(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public FrameBufferHandle CreateFrameBuffer(int width, int height, bool isPicking)
    {
        var info = _frameBufferService.CreateFrameBuffer(width, height, isPicking);
        var handle = new FrameBufferHandle(_nextFrameBufferId++);
        _frameBufferById[handle.Id] = info;
        return handle;
    }

    public void ResizeFrameBuffer(FrameBufferHandle handle, int width, int height)
    {
        if (_frameBufferById.TryGetValue(handle.Id, out var info))
            _frameBufferService.ResizeFrameBuffer(info, width, height, true);
    }

    public void BindFrameBuffer(FrameBufferHandle handle)
    {
        if (_frameBufferById.TryGetValue(handle.Id, out var info))
            _frameBufferService.RenderToFrameBuffer(info);
    }

    public void UnbindFrameBuffer() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

    public void ClearFrameBuffer(FrameBufferHandle handle)
    {
        if (_frameBufferById.TryGetValue(handle.Id, out var info))
            _frameBufferService.ClearViewportBuffer(info);
    }

    public void BeginPickingPass(FrameBufferHandle handle) => BindFrameBuffer(handle);

    public void EndPickingPass() => UnbindFrameBuffer();

    public PickReadResult ReadPickPixel(int x, int y)
    {
        Span<int> data = stackalloc int[2];
        unsafe
        {
            fixed (int* ptr = data)
            {
                GL.ReadPixels(x, y, 1, 1, PixelFormat.RgInteger, PixelType.Int, (IntPtr)ptr);
            }
        }

        var type = data[1] >= 0 ? (SelectionType)data[1] : SelectionType.None;
        return new PickReadResult(data[0], data[1], type);
    }

    public void SetWireframe(bool enabled) =>
        GL.PolygonMode(TriangleFace.FrontAndBack, enabled ? PolygonMode.Line : PolygonMode.Fill);

    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in Vector3 cameraPos)
    {
        var viewMatrix = ToOpenTk(view);
        var projectionMatrix = ToOpenTk(projection);
        var position = new OpenTK.Mathematics.Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z);
        _uniformBufferService.UpdateViewProjectionBuffer(viewMatrix, projectionMatrix, position);
    }

    public void SetViewport(int x, int y, int width, int height) => GL.Viewport(x, y, width, height);

    private static Matrix4 ToOpenTk(in Matrix4x4 value) => new(
        value.M11, value.M12, value.M13, value.M14,
        value.M21, value.M22, value.M23, value.M24,
        value.M31, value.M32, value.M33, value.M34,
        value.M41, value.M42, value.M43, value.M44);
}

public sealed class MockGraphicsBackend : IGraphicsBackend
{
    public void Initialize() { }
    public void Shutdown() { }
    public GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor) => new(1);
    public void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor) { }
    public void DeleteMesh(GpuMeshHandle handle) { }
    public void DrawMesh(GpuMeshHandle handle, DrawFlags flags) { }
    public ShaderHandle GetShader(string name) => new(1);
    public void UseShader(ShaderHandle handle) { }
    public void SetUniformInt(ShaderHandle shader, string name, int value) { }
    public void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value) { }
    public FrameBufferHandle CreateFrameBuffer(int width, int height, bool isPicking) => new(1);
    public void ResizeFrameBuffer(FrameBufferHandle handle, int width, int height) { }
    public void BindFrameBuffer(FrameBufferHandle handle) { }
    public void UnbindFrameBuffer() { }
    public void ClearFrameBuffer(FrameBufferHandle handle) { }
    public void BeginPickingPass(FrameBufferHandle handle) { }
    public void EndPickingPass() { }
    public PickReadResult ReadPickPixel(int x, int y) => new(-1, -1, SelectionType.None);
    public void SetWireframe(bool enabled) { }
    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in Vector3 cameraPos) { }
    public void SetViewport(int x, int y, int width, int height) { }
}
