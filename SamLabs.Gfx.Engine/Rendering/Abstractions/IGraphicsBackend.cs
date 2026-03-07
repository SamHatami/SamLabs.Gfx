using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

/// <summary>
/// Phase 1a renderer seam for backend-specific GPU operations.
/// Current OpenGL implementation remains OpenTK-backed.
/// </summary>
public interface IGraphicsBackend
{
    // Lifecycle
    void Initialize();

    // Camera/View
    void SetViewProjection(Matrix4 view, Matrix4 proj, Vector3 cameraPosition);

    // Framebuffer control
    IViewPort CreateViewportBuffers(string name, int width, int height);
    void RenderToPickingBuffer(IViewPort mainViewport);
    void RenderToViewportBuffer(IViewPort viewport);
    void StopRenderToBuffer();
    void ResizeViewportBuffers(IViewPort mainViewport, int viewportSizeX, int viewportSizeY);

    // Picking backend seam
    IPickingBackend Picking { get; }

    // Shader access
    GLShader? GetShader(string shaderName);
    void ReloadShader(string fullShaderPath);
}
