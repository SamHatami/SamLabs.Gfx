using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class UpdateSystem
{
    private readonly EntityManager _entityManager;
    public virtual int SystemPosition { get; }
    protected UpdateSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public abstract void Update(FrameInput frameInput);
}