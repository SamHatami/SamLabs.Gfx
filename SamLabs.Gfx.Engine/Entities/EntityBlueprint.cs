using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;

namespace SamLabs.Gfx.Engine.Entities;

public abstract class EntityBlueprint
{
    public abstract string Name { get; }
    public abstract void Build(Entity entity, MeshDataComponent meshData = default);
}