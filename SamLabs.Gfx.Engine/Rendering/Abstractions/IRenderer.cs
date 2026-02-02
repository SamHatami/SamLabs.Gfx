using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IRenderer
{
    public void Initialize();
    public void SetViewProjection(Matrix4 view, Matrix4 proj, Vector3 cameraPosition);
    public IViewPort CreateViewportBuffers(string name, int width, int height);
    public void RenderToPickingBuffer(IViewPort mainViewport);
    public void RenderToViewportBuffer(IViewPort viewport);
    public void StopRenderToBuffer();
    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY);
    
    public IReadOnlyCollection<IRenderPass> RenderPasses { get; }

    GLShader? GetShader(string shaderName);
}