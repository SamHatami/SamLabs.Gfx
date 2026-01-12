namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class GLShader
{
    public Dictionary<string, Uniform> UniformLocations { get; }
    public string ShaderName;
    public int ProgramId;
    

    public GLShader(string shaderName, int programId, Dictionary<string, Uniform> uniformLocations)
    {
        UniformLocations = uniformLocations;
        ShaderName = shaderName;
        ProgramId = programId;
    }
}