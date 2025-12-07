namespace SamLabs.Gfx.Viewer.Rendering.Shaders;

public class GLShader
{
    public string ShaderName;
    public int ProgramId;
    public int MatrixModelUniformLocation;

    public GLShader(string shaderName, int programId, int matrixModelUniformLocation)
    {
        ShaderName = shaderName;
        ProgramId = programId;
        MatrixModelUniformLocation = matrixModelUniformLocation;
    }
}