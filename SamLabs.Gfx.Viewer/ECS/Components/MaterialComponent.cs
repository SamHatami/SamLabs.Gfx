using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Shaders;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct MaterialComponent:IDataComponent
{
    public GLShader Shader { get; set; }
    public GLShader PickingShader { get; set; }
    public GLShader HighlightShader { get; set; }
    public GLShader SelectionShader { get; set; } 
    
}