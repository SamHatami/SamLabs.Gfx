using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.Framework;

public class Renderer: IDisposable
{
    private ShaderManager _shaderManager;
    private readonly UniformBufferManager _bufferManager;
    private readonly ILogger<Renderer> _logger;
    private int _mvpLocation = -1;
    private int _vbo = 0;
    private int _vao = 0;
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _proj = Matrix4.Identity;
    private int _vertexCount = 0;

    public Renderer(ShaderManager shaderManager, UniformBufferManager bufferManager, ILogger<Renderer> logger)
    {
        _shaderManager = shaderManager;
        _bufferManager = bufferManager;
        _logger = logger;
    }
    public void Initialize()
    {
        _shaderManager.RegisterShaders();
        _bufferManager.RegisterViewProjectionBuffer();

        //bind shaders to view projection buffer
        foreach (var shader in _shaderManager.GetShaderPrograms())
            _bufferManager.BindUniformToProgram(shader, UniformBufferManager.ViewProjectionName);
    }

    public int GetShaderProgram(string shaderName) => ShaderManager.GetShaderProgram(shaderName);

    public void SetWireframes(bool wireframe)
    {
        if(wireframe)
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        else
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
    }
    public void SetCamera(Matrix4 view, Matrix4 proj)
    {
        _view = view;
        _proj = proj;
        _bufferManager.UpdateViewProjectionBuffer(view, proj);
    }


    public void Dispose()
    {
    }
}


