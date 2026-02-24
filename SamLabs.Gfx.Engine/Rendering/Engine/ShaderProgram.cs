using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class ShaderProgram : IDisposable
{
    private readonly GLShader _shader;
    private static GL Gl => SilkGlContextProvider.GetGl();

    public ShaderProgram(GLShader shader) { _shader = shader; }

    public ShaderProgram Use()
    {
        try { Gl.UseProgram((uint)_shader.ProgramId); }
        catch (Exception e) { Console.WriteLine(e); }
        return this;
    }

    public ShaderProgram SetMatrix4(string name, ref Matrix4 matrix)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.UniformMatrix4(uniform.Location, 1, false, MemoryMarshal.Cast<Matrix4, float>(MemoryMarshal.CreateReadOnlySpan(ref matrix, 1)));
        return this;
    }

    public ShaderProgram SetInt(string name, ref int value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.Uniform1(uniform.Location, value);
        return this;
    }

    public ShaderProgram SetUInt(string name, uint value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.Uniform1((int)uniform.Location, value);
        return this;
    }

    public ShaderProgram SetFloat(string name, float value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.Uniform1(uniform.Location, value);
        return this;
    }

    public ShaderProgram SetVector3(string name, Vector3 value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.Uniform3(uniform.Location, value.X, value.Y, value.Z);
        return this;
    }

    public ShaderProgram SetVector2(string name, Vector2 value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            Gl.Uniform2(uniform.Location, value.X, value.Y);
        return this;
    }

    public void Dispose() => Gl.UseProgram(0);
}

