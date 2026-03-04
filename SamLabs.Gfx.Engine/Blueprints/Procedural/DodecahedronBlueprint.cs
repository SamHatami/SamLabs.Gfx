using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Procedural;

public class DodecahedronBlueprint : ProceduralBlueprintBase
{
    public DodecahedronBlueprint(IComponentRegistry componentRegistry)
        : base(componentRegistry)
    {
    }

    public override string Name => EntityNames.Dodecahedron;
    public override string GeometryType => EntityNames.Dodecahedron;

    public override Dictionary<string, float> GetDefaultParameters()
    {
        return new Dictionary<string, float>
        {
            ["size"] = 1.0f
        };
    }

    public override MeshDataComponent GenerateMesh(Dictionary<string, float> parameters)
    {
        var size = parameters.TryGetValue("size", out var s) ? s : 1.0f;
        return MeshCreator.CreatePlane(size, size, "Dodecahedron");
    }
}
