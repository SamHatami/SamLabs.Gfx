using Microsoft.Extensions.Logging;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.Framework;

public class Renderer: IDisposable
{
    private ShaderManager _shaderManager;
    private readonly ILogger<Renderer> _logger;
    private int _mvpLocation = -1;
    private int _vbo = 0;
    private int _vao = 0;
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _proj = Matrix4.Identity;
    private int _vertexCount = 0;

    public Renderer(ShaderManager shaderManager, ILogger<Renderer> logger)
    {
        _shaderManager = shaderManager;
        _logger = logger;
    }
    public void Initialize()
    {
        _shaderManager.RegisterShaders();
    }

    public int GetShaderProgram(string shaderName) => _shaderManager.GetShaderProgramPosition(shaderName);

    public void SetCamera(Matrix4 view, Matrix4 proj)
    {
        _view = view;
        _proj = proj;
    }


    public void Dispose()
    {
    }
}


