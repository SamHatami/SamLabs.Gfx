using Matrix4x4 = System.Numerics.Matrix4x4;
using NumericsVector3 = System.Numerics.Vector3;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class OpenGLGraphicsBackend : IGraphicsBackend
{
    private readonly ShaderService _shaderService;
    private readonly UniformBufferService _uniformBufferService;
    private readonly FrameBufferService _frameBufferService;
    private readonly Dictionary<int, GLShader> _shaderById = new();
    private readonly Dictionary<int, FrameBufferInfo> _frameBufferById = new();
    private readonly Dictionary<int, BackendMeshResource> _meshById = new();
    private int _nextShaderId = 1;
    private int _nextFrameBufferId = 1;
    private int _nextMeshId = 1;

    public OpenGLGraphicsBackend(ShaderService shaderService, UniformBufferService uniformBufferService, FrameBufferService frameBufferService)
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
        foreach (var mesh in _meshById.Values)
        {
            GL.DeleteVertexArray(mesh.Vao);
            GL.DeleteBuffer(mesh.Vbo);
            if (mesh.Ebo != 0) GL.DeleteBuffer(mesh.Ebo);
            if (mesh.EdgeEbo != 0) GL.DeleteBuffer(mesh.EdgeEbo);
        }

        _meshById.Clear();
    }

    public GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor)
    {
        var vao = GL.GenVertexArray();
        var vbo = GL.GenBuffer();
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, descriptor.Vertices.Length * sizeof(float), descriptor.Vertices, BufferUsage.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, descriptor.VertexStride * sizeof(float), 0);
        if (descriptor.VertexStride >= 6)
        {
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, descriptor.VertexStride * sizeof(float), 3 * sizeof(float));
        }
        if (descriptor.VertexStride >= 8)
        {
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, descriptor.VertexStride * sizeof(float), 6 * sizeof(float));
        }

        var ebo = 0;
        if (descriptor.Indices is { Length: > 0 })
        {
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, descriptor.Indices.Length * sizeof(uint), descriptor.Indices, BufferUsage.StaticDraw);
        }

        var edgeEbo = 0;
        if (descriptor.EdgeIndices is { Length: > 0 })
        {
            edgeEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, edgeEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, descriptor.EdgeIndices.Length * sizeof(uint), descriptor.EdgeIndices, BufferUsage.StaticDraw);
        }

        GL.BindVertexArray(0);

        var handle = new GpuMeshHandle(_nextMeshId++);
        _meshById[handle.Id] = new BackendMeshResource(vao, vbo, ebo, edgeEbo, descriptor.VertexStride, descriptor.Vertices.Length / descriptor.VertexStride, descriptor.Indices?.Length ?? 0, descriptor.EdgeIndices?.Length ?? 0);
        return handle;
    }

    public void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor)
    {
        DeleteMesh(handle);
        var uploaded = UploadMesh(descriptor);
        _meshById[handle.Id] = _meshById[uploaded.Id];
        _meshById.Remove(uploaded.Id);
    }

    public void DeleteMesh(GpuMeshHandle handle)
    {
        if (!_meshById.TryGetValue(handle.Id, out var mesh)) return;

        GL.DeleteVertexArray(mesh.Vao);
        GL.DeleteBuffer(mesh.Vbo);
        if (mesh.Ebo != 0) GL.DeleteBuffer(mesh.Ebo);
        if (mesh.EdgeEbo != 0) GL.DeleteBuffer(mesh.EdgeEbo);
        _meshById.Remove(handle.Id);
    }

    public void DrawMesh(GpuMeshHandle handle, DrawFlags flags)
    {
        if (!_meshById.TryGetValue(handle.Id, out var mesh)) return;

        GL.BindVertexArray(mesh.Vao);
        if ((flags & DrawFlags.Faces) != 0)
        {
            if (mesh.Ebo > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.Ebo);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.VertexCount);
            }
        }

        if ((flags & DrawFlags.Edges) != 0 && mesh.EdgeEbo > 0)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.EdgeEbo);
            GL.DrawElements(PrimitiveType.Lines, mesh.EdgeIndexCount, DrawElementsType.UnsignedInt, 0);
        }

        if ((flags & DrawFlags.Vertices) != 0)
        {
            GL.PointSize(5f);
            GL.DrawArrays(PrimitiveType.Points, 0, mesh.VertexCount);
            GL.PointSize(1f);
        }

        GL.BindVertexArray(0);
    }

    public ShaderHandle GetShader(string name)
    {
        var shader = _shaderService.GetShader(name);
        if (shader == null) return new ShaderHandle(0);
        var handle = new ShaderHandle(_nextShaderId++);
        _shaderById[handle.Id] = shader;
        return handle;
    }

    public void UseShader(ShaderHandle handle)
    {
        if (_shaderById.TryGetValue(handle.Id, out var shader)) GL.UseProgram(shader.ProgramId);
    }

    public void SetUniformInt(ShaderHandle shader, string name, int value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = GL.GetUniformLocation(glShader.ProgramId, name);
        if (location >= 0) GL.Uniform1i(location, 1, ref value);
    }

    public void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = GL.GetUniformLocation(glShader.ProgramId, name);
        if (location < 0) return;
        var matrix = ToOpenTk(value);
        GL.UniformMatrix4f(location, 1, false, ref matrix);
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
        if (_frameBufferById.TryGetValue(handle.Id, out var info)) _frameBufferService.ResizeFrameBuffer(info, width, height, true);
    }

    public void BindFrameBuffer(FrameBufferHandle handle)
    {
        if (_frameBufferById.TryGetValue(handle.Id, out var info)) _frameBufferService.RenderToFrameBuffer(info);
    }

    public void UnbindFrameBuffer() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

    public void ClearFrameBuffer(FrameBufferHandle handle)
    {
        if (_frameBufferById.TryGetValue(handle.Id, out var info)) _frameBufferService.ClearViewportBuffer(info);
    }

    public void BeginPickingPass(FrameBufferHandle handle) => BindFrameBuffer(handle);
    public void EndPickingPass() => UnbindFrameBuffer();

    public PickResult ReadPickPixel(int x, int y)
    {
        Span<int> data = stackalloc int[2];
        unsafe
        {
            fixed (int* ptr = data)
            {
                GL.ReadPixels(x, y, 1, 1, PixelFormat.RgInteger, PixelType.Int, (IntPtr)ptr);
            }
        }

        var entityId = data[0];
        if (entityId < 0)
            return PickResult.Empty;

        var packedId = data[1];
        var type = (SelectionType)((packedId >> 28) & 0xF);
        var subElementId = packedId & 0x0FFFFFFF;
        return new PickResult(entityId, subElementId, type);
    }

    public void SetWireframe(bool enabled) => GL.PolygonMode(TriangleFace.FrontAndBack, enabled ? PolygonMode.Line : PolygonMode.Fill);

    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in NumericsVector3 cameraPos)
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

    private sealed record BackendMeshResource(int Vao, int Vbo, int Ebo, int EdgeEbo, int VertexStride, int VertexCount, int IndexCount, int EdgeIndexCount);
}

public sealed class MockGraphicsBackend : IGraphicsBackend
{
    private int _nextHandle;
    public void Initialize() { }
    public void Shutdown() { }
    public GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor) => new(++_nextHandle);
    public void UpdateMesh(GpuMeshHandle handle, MeshUploadDescriptor descriptor) { }
    public void DeleteMesh(GpuMeshHandle handle) { }
    public void DrawMesh(GpuMeshHandle handle, DrawFlags flags) { }
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
    public PickResult ReadPickPixel(int x, int y) => new(-1, -1, SelectionType.None);
    public void SetWireframe(bool enabled) { }
    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in NumericsVector3 cameraPos) { }
    public void SetViewport(int x, int y, int width, int height) { }
}
