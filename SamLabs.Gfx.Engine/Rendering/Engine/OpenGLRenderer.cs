using Microsoft.Extensions.Logging;
using OpenTK.Mathematics;
using Silk.NET.OpenGL;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class OpenGLRenderer : IDisposable, IRenderer
{
    private ShaderService _shaderService;
    private readonly UniformBufferService _uniformBufferService;
    private readonly FrameBufferService _frameBufferService;
    private readonly MaterialLibrary _materialLibrary;
    private readonly ILogger<OpenGLRenderer> _logger;

    private static GL Gl => SilkGlContextProvider.GetGl();

    public OpenGLRenderer(ShaderService shaderService, UniformBufferService uniformBufferService,
        FrameBufferService frameBufferService, MaterialLibrary materialLibrary, ILogger<OpenGLRenderer> logger)
    {
        _uniformBufferService = uniformBufferService;
        _frameBufferService = frameBufferService;
        _materialLibrary = materialLibrary;
        _shaderService = shaderService;
        _logger = logger;
    }

    public void Initialize()
    {
        _uniformBufferService.RegisterViewProjectionBuffer();
        _uniformBufferService.CreateSingleIntUniform("objectId");
        _shaderService.RegisterShaders();
        _materialLibrary.InitializeLibrary();

        foreach (var shader in _shaderService.GetShaderPrograms())
            _uniformBufferService.BindUniformToProgram(shader.ProgramId, UniformBufferService.ViewProjectionName);
    }

    public GLShader? GetShader(string shaderName) => _shaderService.GetShader(shaderName);

    public void SetWireframes(bool wireframe) =>
        Gl.PolygonMode(GLEnum.FrontAndBack, wireframe ? GLEnum.Line : GLEnum.Fill);

    public void SetViewProjection(Matrix4 view, Matrix4 proj, Vector3 cameraPosition)
    {
        _uniformBufferService.UpdateViewProjectionBuffer(view, proj, cameraPosition);
    }

    public IViewPort CreateViewportBuffers(string name, int width, int height)
    {
        var pickingRenderViewInfo = _frameBufferService.CreateFrameBuffer(width, height, true);
        return new ViewPort(width, height)
        {
            Name = name,
            FullRenderView = new FrameBufferInfo(1, 1, 1, width, height),
            SelectionRenderView = pickingRenderViewInfo
        };
    }

    public void Dispose() { }

    public void ClearViewportBuffer(IViewPort mainViewport) =>
        _frameBufferService.ClearViewportBuffer(mainViewport.FullRenderView);

    public void RenderToPickingBuffer(IViewPort mainViewport)
    {
        if (mainViewport?.SelectionRenderView == null)
        {
            _frameBufferService.ClearRenderBuffer(0);
            return;
        }
        var info = mainViewport.SelectionRenderView;
        // Set viewport to match picking FBO exactly - stays bound for the picking draw + ReadPixels
        Gl.Viewport(0, 0, (uint)info.Width, (uint)info.Height);
        _frameBufferService.RenderToPickingBuffer(info);
    }

    public void RenderToViewportBuffer(IViewPort mainViewport)
    {
        _frameBufferService.ClearViewportBuffer(mainViewport.FullRenderView);
        _frameBufferService.RenderToFrameBuffer(mainViewport.FullRenderView);
    }

    public void StopRenderToBuffer() => Gl.BindFramebuffer(GLEnum.Framebuffer, 0);

    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY)
    {
        mainViewport.Width = viewportSizeX;
        mainViewport.Height = viewportSizeY;
        _frameBufferService.ResizeFrameBuffer(mainViewport.SelectionRenderView, viewportSizeX, viewportSizeY, true);
    }

    public void ReloadShader(string fullShaderPath) => _shaderService.ReloadShader(fullShaderPath);

    public IReadOnlyCollection<IRenderPass> RenderPasses { get; } = [];
}