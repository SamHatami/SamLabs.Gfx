using Avalonia.OpenGL;
using Silk.NET.OpenGL;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Editor.Controls.OpenTk;

internal sealed class AvaloniaTkContext
{
    public AvaloniaTkContext(GlInterface glInterface)
    {
        SilkGlContextProvider.SetResolver(procName => glInterface.GetProcAddress(procName));
        Gl = SilkGlContextProvider.GetGl();
    }

    public GL Gl { get; }
}
