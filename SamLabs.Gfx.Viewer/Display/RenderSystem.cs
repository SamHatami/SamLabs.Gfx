using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Display.Render.Renderpasses;
using SamLabs.Gfx.Viewer.Interfaces;
using SamLabs.Gfx.Viewer.Systems.Interfaces;

namespace SamLabs.Gfx.Viewer.Display;

public class RenderSystem : IDisposable, IRenderer, IRenderSystem
{
    private ShaderManager _shaderManager;
    private readonly UniformBufferManager _uniformBufferManager;
    private readonly FrameBufferHandler _frameBufferHandler;
    private readonly ILogger<RenderSystem> _logger;
    private int _mvpLocation = -1;
    private int _vbo = 0;
    private int _vao = 0;
    private Matrix4? _view = Matrix4.Identity;
    private Matrix4? _proj = Matrix4.Identity;
    private int _vertexCount = 0;

    private List<IRenderPass> _renderPasses = []; //Sorted renderpasses 

    public RenderSystem(ShaderManager shaderManager, UniformBufferManager uniformBufferManager,
        FrameBufferHandler frameBufferHandler, ILogger<RenderSystem> logger)
    {
        _uniformBufferManager = uniformBufferManager;
        _frameBufferHandler = frameBufferHandler;
        _shaderManager = shaderManager;
        _logger = logger;
    }
    
    public void Update(in RenderContext renderContext)
    {
        _uniformBufferManager.UpdateViewProjectionBuffer(renderContext.ViewMatrix, renderContext.ProjectionMatrix);
        
        foreach (var renderPass in _renderPasses)
        {
            renderPass.Render();
        }
    }

    public void Initialize()
    {
        _uniformBufferManager.RegisterViewProjectionBuffer();
        _uniformBufferManager.CreateSingleIntUniform("objectId");
        _shaderManager.RegisterShaders();

        //bind View-Projection uniform to all the shader programs
        foreach (var shader in _shaderManager.GetShaderPrograms())
            _uniformBufferManager.BindUniformToProgram(shader, UniformBufferManager.ViewProjectionName);

        RegisterRenderPasses();
    }

    private void RegisterRenderPasses()
    {
        var selectionRenderPass = new SelectionRenderPass();
        _renderPasses.Add(selectionRenderPass);
        var viewportRenderPass = new ViewportRenderPass();
        _renderPasses.Add(viewportRenderPass);
        var highlightRenderPass = new HighlightRenderPass();
        _renderPasses.Add(highlightRenderPass);
    }

    public int GetShaderProgram(string shaderName)
    {
        return ShaderManager.GetShaderProgram(shaderName);
    }

    public void SetWireframes(bool wireframe)
    {
        if (wireframe)
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        else
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
    }

    public void SendViewProjectionToBuffer(Matrix4 view, Matrix4 proj)
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
        // _frameBufferHandler.ResizeFrameBuffer(mainViewport.FullRenderView, viewportSizeX, viewportSizeY);
        _frameBufferHandler.ResizeFrameBuffer(mainViewport.SelectionRenderView, viewportSizeX, viewportSizeY, true);
    }

    public IReadOnlyCollection<IRenderPass> RenderPasses { get; }

}