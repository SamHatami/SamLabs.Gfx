using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Components.Common;

public struct MaterialComponent:IComponent
{
    public string Name { get; set; } 
    public Dictionary<string, object> UniformValues { get; set; } //A direct map to shader uniforms, but only those that the shader needs
    //Material library with different shaders fordifferent purposes created directly? we only take copies and place them on
    //each entity that needs them.
    //Materiallibrary can be called on when a blueprints is created to assign the correct shaders and uniform values.
    public GLShader Shader { get; set; }
    public GLShader PickingShader { get; set; }
    
}
