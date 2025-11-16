using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;

namespace SamLabs.Gfx.StandAlone.Models;

public class OpenTKControl : OpenGlControlBase
{
    private GlInterface _gl;

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        _gl = gl;
    }
}