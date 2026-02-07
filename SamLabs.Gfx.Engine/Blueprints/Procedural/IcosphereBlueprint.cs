using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Blueprints.Procedural;

public class IcosphereBlueprint : ProceduralBlueprintBase
{
    public IcosphereBlueprint(ShaderService shaderService, IComponentRegistry componentRegistry)
        : base(shaderService, componentRegistry)
    {
    }

    public override string Name => EntityNames.Icosphere;
    public override string GeometryType => EntityNames.Icosphere;

    public override Dictionary<string, float> GetDefaultParameters()
    {
        return new Dictionary<string, float>
        {
            ["radius"] = 1.0f,
            ["subdivisions"] = 1.0f
        };
    }

    public override MeshDataComponent GenerateMesh(Dictionary<string, float> parameters)
    {
        var radius = parameters.TryGetValue("radius", out var r) ? r : 1.0f;
        return MeshCreator.CreatePlane(radius * 2.0f, radius * 2.0f, "Icosphere");
    }
}
