using OpenTK.Mathematics;

namespace SamLabs.Gfx.Core.Framework.Display;

public interface IRenderer
{
    public void Initialize();
    public int GetShaderProgram(string shaderName);
    public void SetCamera(Matrix4 view, Matrix4 proj);
    public IViewPort CreateViewport(string name, int width, int height);
    
    public void BeginRenderToViewPort(IViewPort viewport);
    public void EndRenderToViewPort();

    public void ResizeViewportBuffer(IViewPort mainViewport, int viewportSizeX, int viewportSizeY);

}