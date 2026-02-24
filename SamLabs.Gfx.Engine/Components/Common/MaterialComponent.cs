namespace SamLabs.Gfx.Engine.Components.Common;

public struct MaterialComponent:IComponent
{
    public MaterialComponent()
    {
        UniformValues = new Dictionary<string, object>();
    }

    public string Name { get; set; }
    public string ShaderName { get; set; }
    public string PickingShaderName { get; set; }
    public Dictionary<string, object> UniformValues { get; set; }
}
