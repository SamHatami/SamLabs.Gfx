using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Systems;

namespace SamLabs.Gfx.Engine.Core
{
    public class EngineContext
    {
        public SystemScheduler SystemScheduler { get; }
        public EntityRegistry EntityRegistry { get; }
        public EntityFactory EntityFactory { get; }
        public IRenderer Renderer { get; }
    }
}
