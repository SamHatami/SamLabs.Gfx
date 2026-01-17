using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Construction;

public struct ConstructionAxisDataComponent : IComponent
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }
}
