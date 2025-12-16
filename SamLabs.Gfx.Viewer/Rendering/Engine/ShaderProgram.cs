using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Rendering.Shaders;

namespace SamLabs.Gfx.Viewer.Rendering.Engine;

public class ShaderProgram : IDisposable
{
    private readonly GLShader _shader;

    public ShaderProgram(GLShader shader)
    {
        _shader = shader;
    }
    
    public ShaderProgram Use()
    {
        GL.UseProgram(_shader.ProgramId);
        return this;
    }
    
    public ShaderProgram SetMatrix4(string name, ref Matrix4 matrix)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            GL.UniformMatrix4f(uniform.Location, 1, false, ref matrix);
        return this;
    }
    
    public ShaderProgram SetInt(string name, ref int value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            GL.Uniform1i(uniform.Location, 1, ref value);
        return this;
    }
    
    public ShaderProgram SetUInt(string name, uint value)
    {
        if (_shader.UniformLocations.TryGetValue(name, out var uniform))
            GL.Uniform1ui(uniform.Location, value);
        return this;
    }
    
    public void Dispose() => GL.UseProgram(0);
}