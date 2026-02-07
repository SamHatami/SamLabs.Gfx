using System.Collections.Generic;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Blueprints.Procedural;

public interface IProceduralGeometry
{
    string GeometryType { get; }
    Dictionary<string, float> GetDefaultParameters();
    MeshDataComponent GenerateMesh(Dictionary<string, float> parameters);
}
