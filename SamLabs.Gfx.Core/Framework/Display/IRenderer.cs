using OpenTK.Mathematics;

namespace SamLabs.Gfx.Core.Framework.Display;

public interface IRenderer
{
    public void Initialize();
    public int GetShaderProgram(string shaderName);
    public void SetCamera(Matrix4 view, Matrix4 proj);
    public IViewPort CreateViewportBuffers(string name, int width, int height);
    public void RenderToPickingBuffer(IViewPort mainViewport);
    public void RenderToViewportBuffer(IViewPort viewport);
    public void ClearPickingBuffer(IViewPort mainViewport);
    public void ClearViewportBuffer(IViewPort mainViewport);
    public void StopRenderToBuffer();
    public void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY);
}