using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Rendering.Abstractions;

public interface IRenderer : IGraphicsBackend
{
    IReadOnlyCollection<IRenderPass> RenderPasses { get; }
}
