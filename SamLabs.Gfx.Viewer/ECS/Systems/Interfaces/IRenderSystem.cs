using SamLabs.Gfx.Viewer.Rendering.Passes;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;

public interface IRenderSystem
{
    void Update(in RenderContext renderContext);
}