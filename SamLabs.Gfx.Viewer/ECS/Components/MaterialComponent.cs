using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct MaterialComponent:IDataComponent
{
    public GLShader Shader { get; set; }
    public GLShader PickingShader { get; set; }
    public GLShader SelectionShader { get; set; } 
    
}