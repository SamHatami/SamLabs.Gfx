using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Passes;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class ResizeViewSystem : RenderSystem
{
    public override void Update(RenderContext renderContext)
    {
        if(!renderContext.ResizeRequested)
            return;
        
        //the viewport perhaps doesnt need to exist, or passed by reference in the rendercontext
        _renderer.ResizeViewportBuffers(renderContext.ViewWidth, renderContext.ViewHeight);
    }
}