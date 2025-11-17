using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;

namespace SamLabs.Gfx.Viewer.Display;

public class Renderer : IDisposable, IRenderer
{
    private ShaderManager _shaderManager;
    private readonly UniformBufferManager _uniformBufferManager;
    private readonly FrameBufferHandler _frameBufferHandler;
    private readonly ILogger<Renderer> _logger;
    private int _mvpLocation = -1;
    private int _vbo = 0;
    private int _vao = 0;
    private Matrix4? _view = Matrix4.Identity;
    private Matrix4? _proj = Matrix4.Identity;
    private int _vertexCount = 0;

    public IRenderPass[] RenderPasses; //Sorted renderpasses 
    public Renderer(ShaderManager shaderManager, UniformBufferManager uniformBufferManager,
        FrameBufferHandler frameBufferHandler, ILogger<Renderer> logger)
    {
        _uniformBufferManager = uniformBufferManager;
        _frameBufferHandler = frameBufferHandler;
        _shaderManager = shaderManager;
        _logger = logger;
    }

    public void Initialize()
    {
        _uniformBufferManager.RegisterViewProjectionBuffer();
        _uniformBufferManager.CreateSingleIntUniform("objectId");
        _shaderManager.RegisterShaders();

        //bind shaders to view projection buffer
        foreach (var shader in _shaderManager.GetShaderPrograms())
            _uniformBufferManager.BindUniformToProgram(shader, UniformBufferManager.ViewProjectionName);
    }

    public int GetShaderProgram(string shaderName) => ShaderManager.GetShaderProgram(shaderName);

    public void SetWireframes(bool wireframe)
    {
        if (wireframe)
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        else
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
    }

    public void SetCamera(Matrix4 view, Matrix4 proj)
    {
        _view = view;
        _proj = proj;
        _uniformBufferManager.UpdateViewProjectionBuffer(view, proj);
    }

    public IViewPort CreateViewportBuffers(string name, int width, int height)
    {
        // var fullRenderViewInfo = _frameBufferHandler.CreateFrameBuffer(width, height);
        var pickingRenderViewInfo = _frameBufferHandler.CreateFrameBuffer(width, height, true);

        var viewport = new ViewPort(width, height)
        {
            Name = name,
            FullRenderView = new FrameBufferInfo(1, 1, 1, width, height),
            SelectionRenderView = pickingRenderViewInfo
        };
        return viewport;
    }

    public void SetViewPort(int width, int height, int x, int y) => GL.Viewport(x, y, width, height);

    public void RenderScene(IScene scene)
    {
    }

    public void Dispose()
    {
    }

    public void ClearPickingBuffer(IViewPort mainViewport)
    {
        _frameBufferHandler.ClearPickingBuffer(mainViewport.SelectionRenderView);
    }

    public void ClearViewportBuffer(IViewPort mainViewport)
    {
        _frameBufferHandler.ClearViewportBuffer(mainViewport.FullRenderView);
    }

    public void RenderToPickingBuffer(IViewPort mainViewport)
    {
        if (mainViewport == null)
        {
            _frameBufferHandler.ClearRenderBuffer(0);
            
        }
        else
        {
            _frameBufferHandler.ClearViewportBuffer(mainViewport.SelectionRenderView); //Kept for ImGui...
            _frameBufferHandler.RenderToFrameBuffer(mainViewport.SelectionRenderView);
        }
    }

    public void RenderToViewportBuffer(IViewPort mainViewport)
    {
        _frameBufferHandler.ClearViewportBuffer(mainViewport.FullRenderView);
        _frameBufferHandler.RenderToFrameBuffer(mainViewport.FullRenderView);
    }

    public void StopRenderToBuffer()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY)
    {
        _frameBufferHandler.ResizeFrameBuffer(mainViewport.FullRenderView, viewportSizeX, viewportSizeY);
        _frameBufferHandler.ResizeFrameBuffer(mainViewport.SelectionRenderView, viewportSizeX, viewportSizeY, true);
    }
}