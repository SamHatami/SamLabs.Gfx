using System.Collections.Generic;

namespace SamLabs.Gfx.Engine.Components;

public struct ProceduralGeometryComponent : IComponent
{
    public ProceduralGeometryComponent()
    {
        GeometryType = null;
    }

    public string GeometryType { get; set; }
    public Dictionary<string, float> Parameters { get; set; } = new();
}
