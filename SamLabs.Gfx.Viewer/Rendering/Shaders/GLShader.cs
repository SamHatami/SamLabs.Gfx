namespace SamLabs.Gfx.Viewer.Rendering.Shaders;

public class GLShader
{
    public string ShaderName;
    public int ProgramId;

    public GLShader(string shaderName, int programId)
    {
        ShaderName = shaderName;
        ProgramId = programId;
    }
}