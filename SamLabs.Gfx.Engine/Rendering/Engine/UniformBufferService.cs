using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Core.Utility;
using Silk.NET.OpenGL;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class UniformBufferService : IDisposable
{
    private const int BufferCount = 1;
    private int[] _viewProjectionBuffers = new int[BufferCount];
    private int _currentBufferIndex = 0;
    
    private const uint ViewProjectionBindingPoint = 0;
    private const uint ObjectIdBindingPoint = 1;
    public const string ViewProjectionName = "ViewProjection";
    private readonly Dictionary<string, uint> UniformBindingPoints = new();
    private readonly Dictionary<string, int> _uniformLocations = new();

    private static GL Gl => SilkGlContextProvider.GetGl();

    public uint GetUniformBindingPoint(string name) =>
        UniformBindingPoints.TryGetValue(name, out var bp) ? bp : 0;

    public int RegisterModelMatrixToShader(int program) =>
        (int)Gl.GetUniformLocation((uint)program, "uModel");

    public void RegisterViewProjectionBuffer()
    {
        var bufferSize = (nuint)(SizeOf.FMatrix4 * 2 + 16);
        for (var i = 0; i < BufferCount; i++)
        {
            var buffer = Gl.GenBuffer();
            _viewProjectionBuffers[i] = (int)buffer;
            Gl.BindBuffer(BufferTargetARB.UniformBuffer, buffer);
            Gl.BufferData(BufferTargetARB.UniformBuffer, bufferSize, ReadOnlySpan<byte>.Empty, BufferUsageARB.DynamicDraw);
            Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
        }
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, ViewProjectionBindingPoint, (uint)_viewProjectionBuffers[0]);
        UniformBindingPoints.Add(ViewProjectionName, ViewProjectionBindingPoint);
    }

    public void UpdateViewProjectionBuffer(Matrix4 view, Matrix4 projection, Vector3 cameraPosition)
    {
        if (_viewProjectionBuffers[0] == 0)
            RegisterViewProjectionBuffer();

        _currentBufferIndex = (_currentBufferIndex + 1) % BufferCount;
        var currentBuffer = (uint)_viewProjectionBuffers[_currentBufferIndex];

        Gl.BindBuffer(BufferTargetARB.UniformBuffer, currentBuffer);
        Gl.BufferSubData(BufferTargetARB.UniformBuffer, 0, (nuint)SizeOf.FMatrix4, MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref view, 1)));
        Gl.BufferSubData(BufferTargetARB.UniformBuffer, SizeOf.FMatrix4, (nuint)SizeOf.FMatrix4, MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref projection, 1)));
        Gl.BufferSubData(BufferTargetARB.UniformBuffer, SizeOf.FMatrix4 * 2, (nuint)SizeOf.FVector3, MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref cameraPosition, 1)));
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, ViewProjectionBindingPoint, currentBuffer);
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }

    public void RegisterAndBindUniform(int sizeInBytes, string uniqueName)
    {
        if (UniformBindingPoints.ContainsKey(uniqueName)) return;

        var buffer = Gl.GenBuffer();
        var bindingPoint = UniformBindingPoints.Count > 0 ? UniformBindingPoints.Values.Max() + 1 : 1;
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, buffer);
        Gl.BufferData(BufferTargetARB.UniformBuffer, (nuint)sizeInBytes, ReadOnlySpan<byte>.Empty, BufferUsageARB.DynamicDraw);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, bindingPoint, buffer);
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
        UniformBindingPoints.Add(uniqueName, bindingPoint);
    }

    public void CreateSingleIntUniform(string name)
    {
        var buffer = Gl.GenBuffer();
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, buffer);
        Gl.BufferData(BufferTargetARB.UniformBuffer, (nuint)SizeOf.Int, ReadOnlySpan<byte>.Empty, BufferUsageARB.DynamicDraw);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, ObjectIdBindingPoint, buffer);
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
        UniformBindingPoints.Add(name, ObjectIdBindingPoint);
    }

    public void BindUniformToProgram(int program, string name)
    {
        if (!UniformBindingPoints.TryGetValue(name, out var bindingPoint)) return;
        var blockIndex = Gl.GetUniformBlockIndex((uint)program, name);
        Gl.UniformBlockBinding((uint)program, blockIndex, bindingPoint);
    }

    public void Dispose()
    {
        for (var i = 0; i < BufferCount; i++)
        {
            if (_viewProjectionBuffers[i] != 0)
            {
                Gl.DeleteBuffer((uint)_viewProjectionBuffers[i]);
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
        UniformInfo.Add(UniformNames.uBaseColor, typeof(Vector4));
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
        UniformInfo.Add(UniformNames.uMajorLineFrequency, typeof(int));
        UniformInfo.Add(UniformNames.uTextureCoordinate, typeof(Vector2));
        UniformInfo.Add(UniformNames.uGridSpacing, typeof(float));
        
    }
}

public static class UniformNames
{
    public const string uModel = "uModel";
    public const string uView = "uView";
    public const string uProj = "uProj";
    public const string uTextureCoordinate = "uTextureCoordinate";
    public const string uBaseColor = "uBaseColor";
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
    public const string uGridSpacing = "uGridSpacing";
    public const string uMajorLineFrequency = "uMajorLineFrequency";
}