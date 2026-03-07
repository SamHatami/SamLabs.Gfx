using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.Components.Common;

public struct GpuMeshHandleComponent : IComponent
{
    public GpuMeshHandle Handle { get; set; }
    public bool IsDirty { get; set; }
}
