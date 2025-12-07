using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Shaders;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Rendering.Engine;

public class OpenGLRenderer : IDisposable, IRenderer
{
    private ShaderService _shaderService;
    private readonly UniformBufferService _uniformBufferService;
    private readonly FrameBufferService _frameBufferService;
    private readonly ILogger<OpenGLRenderer> _logger;
    private int _mvpLocation = -1;
    private int _vbo = 0;
    private int _vao = 0;
    private Matrix4? _view = Matrix4.Identity;
    private Matrix4? _proj = Matrix4.Identity;
    private int _vertexCount = 0;

    private List<IRenderPass> _renderPasses = []; //Sorted renderpasses 

    public OpenGLRenderer(ShaderService shaderService, UniformBufferService uniformBufferService,
        FrameBufferService frameBufferService, ILogger<OpenGLRenderer> logger)
    {
        _uniformBufferService = uniformBufferService;
        _frameBufferService = frameBufferService;
        _shaderService = shaderService;
        _logger = logger;
    }

    public void Initialize()
    {
        _uniformBufferService.RegisterViewProjectionBuffer();
        _uniformBufferService.CreateSingleIntUniform("objectId");
        _shaderService.RegisterShaders();

        //bind View-Projection uniform to all the shader programs
        foreach (var shader in _shaderService.GetShaderPrograms())
            _uniformBufferService.BindUniformToProgram(shader.ProgramId, UniformBufferService.ViewProjectionName);

    }

    public GLShader? GetShader(string shaderName) => _shaderService.GetShader(shaderName);

    public void SetWireframes(bool wireframe)
    {
        if (wireframe)
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        else
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
    }

    public void SetViewProjection(Matrix4 view, Matrix4 proj)
    {
        _view = view;
        _proj = proj;
        _uniformBufferService.UpdateViewProjectionBuffer(view, proj);
    }

    public IViewPort CreateViewportBuffers(string name, int width, int height)
    {
        // var fullRenderViewInfo = _frameBufferHandler.CreateFrameBuffer(width, height);
        var pickingRenderViewInfo = _frameBufferService.CreateFrameBuffer(width, height, true);

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
        _frameBufferService.ClearPickingBuffer(mainViewport.SelectionRenderView);
    }

    public void ClearViewportBuffer(IViewPort mainViewport)
    {
        _frameBufferService.ClearViewportBuffer(mainViewport.FullRenderView);
    }

    public void RenderToPickingBuffer(IViewPort mainViewport)
    {
        if (mainViewport == null)
        {
            _frameBufferService.ClearRenderBuffer(0);
        }
        else
        {
            _frameBufferService.ClearViewportBuffer(mainViewport.SelectionRenderView); //Kept for ImGui...
            _frameBufferService.RenderToFrameBuffer(mainViewport.SelectionRenderView);
        }
    }

    public void RenderToViewportBuffer(IViewPort mainViewport)
    {
        _frameBufferService.ClearViewportBuffer(mainViewport.FullRenderView);
        _frameBufferService.RenderToFrameBuffer(mainViewport.FullRenderView);
    }

    public void StopRenderToBuffer()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY)
    {
        // _frameBufferHandler.ResizeFrameBuffer(mainViewport.FullRenderView, viewportSizeX, viewportSizeY);
        _frameBufferService.ResizeFrameBuffer(mainViewport.SelectionRenderView, viewportSizeX, viewportSizeY, true);
    }

    public static void DrawMesh(GlMeshDataComponent mesh)
    {
        GL.BindVertexArray(mesh.Vao);
        GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
    
    public IReadOnlyCollection<IRenderPass> RenderPasses { get; }

}