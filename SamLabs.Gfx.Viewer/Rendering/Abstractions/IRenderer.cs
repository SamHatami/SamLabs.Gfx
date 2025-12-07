using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Rendering.Shaders;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Rendering.Abstractions;

public interface IRenderer
{
    public void Initialize();
    public void SetViewProjection(Matrix4 view, Matrix4 proj);
    public IViewPort CreateViewportBuffers(string name, int width, int height);
    public void RenderToPickingBuffer(IViewPort mainViewport);
    public void RenderToViewportBuffer(IViewPort viewport);
    public void StopRenderToBuffer();
    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY);
    
    public IReadOnlyCollection<IRenderPass> RenderPasses { get; }


    public void ClearPickingBuffer(IViewPort viewport);
    GLShader? GetShader(string shaderName);
}