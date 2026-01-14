using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Components.Common;

public struct MaterialComponent:IComponent
{
    public GLShader Shader { get; set; }
    public GLShader PickingShader { get; set; }
    public GLShader SelectionShader { get; set; } 
    
}
