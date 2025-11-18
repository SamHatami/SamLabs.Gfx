using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Viewer.Display;

public class UniformBufferManager : IDisposable
{
    private static int _viewProjectionBuffer;
    private const int ViewProjectionBindingPoint = 0;
    private const int ObjectIdBindingPoint = 1;
    public const string ViewProjectionName = "ViewProjection";
    private readonly Dictionary<string, uint> UniformBindingPoints = new();

    public uint GetUniformBindingPoint(string name)
    {
        return UniformBindingPoints.TryGetValue(name, out var bindingPoint) ? bindingPoint : 0;
    }


    public void RegisterViewProjectionBuffer()
    {
        //View and Projection matrices buffers

        _viewProjectionBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _viewProjectionBuffer);
        GL.BufferData(BufferTarget.UniformBuffer, Sizes.FMatrix4 * 2, IntPtr.Zero, BufferUsage.DynamicDraw);
        GL.BindBufferBase(BufferTarget.UniformBuffer, ViewProjectionBindingPoint, _viewProjectionBuffer);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);

        UniformBindingPoints.Add(ViewProjectionName, ViewProjectionBindingPoint);
    }

    public void UpdateViewProjectionBuffer(Matrix4 view, Matrix4 projection)
    {
        if (_viewProjectionBuffer == 0) RegisterViewProjectionBuffer();

        GL.BindBuffer(BufferTarget.UniformBuffer, _viewProjectionBuffer);
        GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, Sizes.FMatrix4, ref view);
        GL.BufferSubData(BufferTarget.UniformBuffer, Sizes.FMatrix4, Sizes.FMatrix4, ref projection);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void RegisterAndBindUniform(int sizeInBytes, string uniqueName) //TBD
    {
        if (UniformBindingPoints.ContainsKey(uniqueName))
            return; // Already registered

        var buffer = GL.GenBuffer();
        var bindingPoint = UniformBindingPoints.Count > 0
            ? UniformBindingPoints.Values.Max() + 1
            : 1; // Start at 1 since ViewProjection uses 0

        GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
        GL.BufferData(BufferTarget.UniformBuffer, sizeInBytes, IntPtr.Zero, BufferUsage.DynamicDraw);
        GL.BindBufferBase(BufferTarget.UniformBuffer, bindingPoint, buffer);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);

        UniformBindingPoints.Add(uniqueName, bindingPoint);
    }

    public void CreateSingleIntUniform(string name)
    {
        var bufferId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, bufferId);
        GL.BufferData(BufferTarget.UniformBuffer, Sizes.Int, IntPtr.Zero, BufferUsage.DynamicDraw);
        GL.BindBufferBase(BufferTarget.UniformBuffer, ObjectIdBindingPoint, _viewProjectionBuffer);

        GL.BindBuffer(BufferTarget.UniformBuffer, 0);

        UniformBindingPoints.Add(name, ObjectIdBindingPoint);
    }


    public void BindUniformToProgram(int program, string name)
    {
        if (!UniformBindingPoints.TryGetValue(name, out var bindingPoint))
            return;

        var blockIndex = GL.GetUniformBlockIndex(program, name);
        GL.UniformBlockBinding(program, blockIndex, bindingPoint);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_viewProjectionBuffer);
    }
}