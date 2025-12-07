using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Core.Utility;

namespace SamLabs.Gfx.Viewer.Rendering.Engine;

public class UniformBufferService : IDisposable
{
    private const int BufferCount = 3; // Triple buffering
    private int[] _viewProjectionBuffers = new int[BufferCount];
    private int _currentBufferIndex = 0;
    
    private const int ViewProjectionBindingPoint = 0;
    private const int ObjectIdBindingPoint = 1;
    public const string ViewProjectionName = "ViewProjection";
    private readonly Dictionary<string, uint> UniformBindingPoints = new();

    public uint GetUniformBindingPoint(string name)
    {
        return UniformBindingPoints.TryGetValue(name, out var bindingPoint) ? bindingPoint : 0;
    }

    public int RegisterModelMatrixToShader(int program) => GL.GetUniformLocation(program, "uModel");

    public void RegisterViewProjectionBuffer()
    {
        for (int i = 0; i < BufferCount; i++)
        {
            _viewProjectionBuffers[i] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _viewProjectionBuffers[i]);
            GL.BufferData(BufferTarget.UniformBuffer, SizeOf.FMatrix4 * 2, IntPtr.Zero, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        GL.BindBufferBase(BufferTarget.UniformBuffer, ViewProjectionBindingPoint, _viewProjectionBuffers[0]);
        
        UniformBindingPoints.Add(ViewProjectionName, ViewProjectionBindingPoint);
    }

    public void UpdateViewProjectionBuffer(Matrix4 view, Matrix4 projection)
    {
        if (_viewProjectionBuffers[0] == 0) 
            RegisterViewProjectionBuffer();

        // Rotate to next buffer to avoid GPU stalls
        _currentBufferIndex = (_currentBufferIndex + 1) % BufferCount;
        var currentBuffer = _viewProjectionBuffers[_currentBufferIndex];

        // Update the current buffer
        GL.BindBuffer(BufferTarget.UniformBuffer, currentBuffer);
        GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, SizeOf.FMatrix4, ref view);
        GL.BufferSubData(BufferTarget.UniformBuffer, SizeOf.FMatrix4, SizeOf.FMatrix4, ref projection);
        
        // Bind this buffer to the binding point
        GL.BindBufferBase(BufferTarget.UniformBuffer, ViewProjectionBindingPoint, currentBuffer);
        
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void RegisterAndBindUniform(int sizeInBytes, string uniqueName)
    {
        if (UniformBindingPoints.ContainsKey(uniqueName))
            return; 

        var buffer = GL.GenBuffer();
        var bindingPoint = UniformBindingPoints.Count > 0
            ? UniformBindingPoints.Values.Max() + 1
            : 1; 

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
        GL.BufferData(BufferTarget.UniformBuffer, SizeOf.Int, IntPtr.Zero, BufferUsage.DynamicDraw);
        GL.BindBufferBase(BufferTarget.UniformBuffer, ObjectIdBindingPoint, bufferId);

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
        for (int i = 0; i < BufferCount; i++)
        {
            if (_viewProjectionBuffers[i] != 0)
            {
                GL.DeleteBuffer(_viewProjectionBuffers[i]);
                _viewProjectionBuffers[i] = 0;
            }
        }
    }
}