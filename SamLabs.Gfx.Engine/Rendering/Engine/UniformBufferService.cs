using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Core.Utility;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class UniformBufferService : IDisposable
{
    private const int BufferCount = 3; // Triple buffering
    private int[] _viewProjectionBuffers = new int[BufferCount];
    private int _currentBufferIndex = 0;
    
    private const int ViewProjectionBindingPoint = 0;
    private const int ObjectIdBindingPoint = 1;
    public const string ViewProjectionName = "ViewProjection";
    private readonly Dictionary<string, uint> UniformBindingPoints = new();
    private readonly Dictionary<string, int> _uniformLocations = new();

    public uint GetUniformBindingPoint(string name)
    {
        return UniformBindingPoints.TryGetValue(name, out var bindingPoint) ? bindingPoint : 0;
    }

    public int RegisterModelMatrixToShader(int program) => GL.GetUniformLocation(program, "uModel");

    public void RegisterViewProjectionBuffer()
    {
        for (var i = 0; i < BufferCount; i++)
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
        for (var i = 0; i < BufferCount; i++)
        {
            if (_viewProjectionBuffers[i] != 0)
            {
                GL.DeleteBuffer(_viewProjectionBuffers[i]);
                _viewProjectionBuffers[i] = 0;
            }
        }
    }
    
    
}

public static class UniformNameTypeDictionary
{
    public static Dictionary<string, Type> UniformInfo = new();

    static UniformNameTypeDictionary()
    {

        UniformInfo.Add(UniformNames.uModel, typeof(Matrix4));
        UniformInfo.Add(UniformNames.uView, typeof(Matrix4));
        UniformInfo.Add(UniformNames.uProj, typeof(Matrix4));
        UniformInfo.Add(UniformNames.uColor, typeof(Vector4));
        UniformInfo.Add(UniformNames.uPickingColor, typeof(Vector4));
        UniformInfo.Add(UniformNames.uPickingId, typeof(int));
        UniformInfo.Add(UniformNames.uTime, typeof(float));
        UniformInfo.Add(UniformNames.uCameraPos, typeof(Vector3));
        UniformInfo.Add(UniformNames.uLightPos, typeof(Vector3));
        UniformInfo.Add(UniformNames.uLightColor, typeof(Vector3));
        UniformInfo.Add(UniformNames.uIsHovered, typeof(int));
        UniformInfo.Add(UniformNames.uEntityId, typeof(int));
        UniformInfo.Add(UniformNames.uIsSelected, typeof(int));
        UniformInfo.Add(UniformNames.uManipulatorCenter, typeof(Vector3));
        UniformInfo.Add(UniformNames.uManipulatorAxis, typeof(Vector3));
        UniformInfo.Add(UniformNames.uPickingType, typeof(int));
        UniformInfo.Add(UniformNames.uVertexRenderSize, typeof(int));
        UniformInfo.Add(UniformNames.uGridSize, typeof(int));
        UniformInfo.Add(UniformNames.uGridColor, typeof(Vector3));
        UniformInfo.Add(UniformNames.uGridLineSize, typeof(int));
        UniformInfo.Add(UniformNames.uTextureCoordinate, typeof(Vector2));
        
    }
}

public static class UniformNames
{
    public const string uModel = "uModel";
    public const string uView = "uView";
    public const string uProj = "uProj";
    public const string uTextureCoordinate = "uTextureCoordinate";
    public const string uColor = "uColor";
    public const string uPickingColor = "uPickingColor";
    public const string uPickingId = "uPickingId";
    public const string uEntityId = "uEntityId";
    public const string uTime = "uTime";
    public const string uCameraPos = "uCameraPos";
    public const string uLightPos = "uLightPos";
    public const string uLightColor = "uLightColor";
    public const string uIsHovered = "uIsHovered";
    public const string uIsSelected = "uIsSelected";
    public const string uManipulatorCenter = "uManipulatorCenter";
    public const string uManipulatorAxis = "uManipulatorAxis";
    public const string uPickingType = "uPickingType";
    public const string uVertexRenderSize = "uVertexRenderSize";
    public const string uGridSize = "uGridSize";
    public const string uGridColor = "uGridColor";
    public const string uGridLineSize = "uGridLineSize";
}