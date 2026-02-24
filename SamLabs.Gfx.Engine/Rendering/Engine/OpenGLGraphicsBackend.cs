using Matrix4x4 = System.Numerics.Matrix4x4;
using NumericsVector3 = System.Numerics.Vector3;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using Silk.NET.OpenGL;

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

    private GL Gl => SilkGlContextProvider.GetGl();

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
            Gl.DeleteVertexArray((uint)mesh.Vao);
            Gl.DeleteBuffer((uint)mesh.Vbo);
            if (mesh.Ebo != 0) Gl.DeleteBuffer((uint)mesh.Ebo);
            if (mesh.EdgeEbo != 0) Gl.DeleteBuffer((uint)mesh.EdgeEbo);
        }

        _meshById.Clear();
    }

    public unsafe GpuMeshHandle UploadMesh(MeshUploadDescriptor descriptor)
    {
        try
        {
            var vao = (int)Gl.GenVertexArray();
            var vbo = (int)Gl.GenBuffer();
            Gl.BindVertexArray((uint)vao);
            Gl.BindBuffer(GLEnum.ArrayBuffer, (uint)vbo);
            unsafe
            {
                fixed (float* ptr = descriptor.Vertices)
                    Gl.BufferData(GLEnum.ArrayBuffer, (nuint)(descriptor.Vertices.Length * sizeof(float)), ptr, GLEnum.StaticDraw);
            }

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)(descriptor.VertexStride * sizeof(float)), 0);
            if (descriptor.VertexStride >= 6)
            {
                Gl.EnableVertexAttribArray(1);
                Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)(descriptor.VertexStride * sizeof(float)), (void*)(3 * sizeof(float)));
            }

            if (descriptor.VertexStride >= 8)
            {
                Gl.EnableVertexAttribArray(2);
                Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)(descriptor.VertexStride * sizeof(float)), (void*)(6 * sizeof(float)));
            }

            var ebo = 0;
            if (descriptor.Indices is { Length: > 0 })
            {
                ebo = (int)Gl.GenBuffer();
                Gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)ebo);
                unsafe
                {
                    fixed (uint* ptr = descriptor.Indices)
                        Gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(descriptor.Indices.Length * sizeof(uint)), ptr, GLEnum.StaticDraw);
                }
            }

            var edgeEbo = 0;
            if (descriptor.EdgeIndices is { Length: > 0 })
            {
                edgeEbo = (int)Gl.GenBuffer();
                Gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)edgeEbo);
                unsafe
                {
                    fixed (uint* ptr = descriptor.EdgeIndices)
                        Gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(descriptor.EdgeIndices.Length * sizeof(uint)), ptr, GLEnum.StaticDraw);
                }
            }

            Gl.BindVertexArray(0);

            var handle = new GpuMeshHandle(_nextMeshId++);
            _meshById[handle.Id] = new BackendMeshResource(vao, vbo, ebo, edgeEbo, descriptor.VertexStride, descriptor.Vertices.Length / descriptor.VertexStride, descriptor.Indices?.Length ?? 0, descriptor.EdgeIndices?.Length ?? 0);
            return handle;
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR in UploadMesh: {e.Message}");
            Console.WriteLine(e.StackTrace);
            throw;
        }
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

        Gl.DeleteVertexArray((uint)mesh.Vao);
        Gl.DeleteBuffer((uint)mesh.Vbo);
        if (mesh.Ebo != 0) Gl.DeleteBuffer((uint)mesh.Ebo);
        if (mesh.EdgeEbo != 0) Gl.DeleteBuffer((uint)mesh.EdgeEbo);
        _meshById.Remove(handle.Id);
    }

    public unsafe void DrawMesh(GpuMeshHandle handle, DrawFlags flags)
    {
        if (!_meshById.TryGetValue(handle.Id, out var mesh))
        {
            Console.WriteLine($"ERROR: Mesh handle {handle.Id} not found in graphics backend");
            return;
        }

        try
        {
            Gl.BindVertexArray((uint)mesh.Vao);
            if ((flags & DrawFlags.Faces) != 0)
            {
                if (mesh.Ebo > 0)
                {
                    Gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)mesh.Ebo);
                    Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.IndexCount, DrawElementsType.UnsignedInt, (void*)0);
                }
                else
                {
                    Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.VertexCount);
                }
            }

            if ((flags & DrawFlags.Edges) != 0 && mesh.EdgeEbo > 0)
            {
                Gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)mesh.EdgeEbo);
                Gl.DrawElements(PrimitiveType.Lines, (uint)mesh.EdgeIndexCount, DrawElementsType.UnsignedInt, (void*)0);
            }

            if ((flags & DrawFlags.Vertices) != 0)
            {
                Gl.PointSize(5f);
                Gl.DrawArrays(PrimitiveType.Points, 0, (uint)mesh.VertexCount);
                Gl.PointSize(1f);
            }

            Gl.BindVertexArray(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR in DrawMesh: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
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
        if (_shaderById.TryGetValue(handle.Id, out var shader)) Gl.UseProgram((uint)shader.ProgramId);
    }

    public void SetUniformInt(ShaderHandle shader, string name, int value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = Gl.GetUniformLocation((uint)glShader.ProgramId, name);
        if (location >= 0) Gl.Uniform1(location, value);
    }

    public void SetUniformMatrix4(ShaderHandle shader, string name, in Matrix4x4 value)
    {
        if (!_shaderById.TryGetValue(shader.Id, out var glShader)) return;
        var location = Gl.GetUniformLocation((uint)glShader.ProgramId, name);
        if (location < 0) return;
        var values = new[]
        {
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        };
        unsafe
        {
            fixed (float* ptr = values)
                Gl.UniformMatrix4(location, 1, false, ptr);
        }
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

    public void UnbindFrameBuffer() => Gl.BindFramebuffer(GLEnum.Framebuffer, 0);

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
                // RG_INTEGER = 0x8228, must be read as Int to match RG32i texture
                Gl.ReadPixels(x, y, 1, 1, PixelFormat.RGInteger, PixelType.Int, ptr);
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

    public void SetWireframe(bool enabled) => Gl.PolygonMode(GLEnum.FrontAndBack, enabled ? GLEnum.Line : GLEnum.Fill);

    public void SetViewProjection(in Matrix4x4 view, in Matrix4x4 projection, in NumericsVector3 cameraPos)
    {
        var viewMatrix = ToOpenTk(view);
        var projectionMatrix = ToOpenTk(projection);
        var position = new OpenTK.Mathematics.Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z);
        _uniformBufferService.UpdateViewProjectionBuffer(viewMatrix, projectionMatrix, position);
    }

    public void SetViewport(int x, int y, int width, int height) => Gl.Viewport(x, y, (uint)width, (uint)height);

    public void BeginMainRenderPass(int frameBufferId, int viewWidth, int viewHeight)
    {
        Gl.BindFramebuffer(GLEnum.Framebuffer, (uint)frameBufferId);
        Gl.Enable(GLEnum.DepthTest);
        Gl.Enable(GLEnum.Blend);
        Gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        Gl.Enable(GLEnum.LineSmooth);
        Gl.Hint(GLEnum.LineSmoothHint, GLEnum.Nicest);
        Gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
        Gl.Viewport(0, 0, (uint)viewWidth, (uint)viewHeight);
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
    }

    public void EndMainRenderPass()
    {
        Gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        Gl.Disable(GLEnum.Blend);
        Gl.Disable(GLEnum.LineSmooth);
        Gl.Disable(GLEnum.DepthTest);
        Gl.BlendFunc(GLEnum.One, GLEnum.Zero);
    }

    public void BeginDepthPass()
    {
        Gl.Clear((uint)ClearBufferMask.DepthBufferBit);
        Gl.Enable(GLEnum.DepthTest);
    }

    public void EndDepthPass() => Gl.Disable(GLEnum.DepthTest);

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
