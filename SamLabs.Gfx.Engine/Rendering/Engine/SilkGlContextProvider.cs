using Silk.NET.OpenGL;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public static class SilkGlContextProvider
{
    private static Func<string, nint>? _resolver;
    private static GL? _gl;

    public static void SetResolver(Func<string, nint> resolver)
    {
        _resolver = resolver;
        _gl = GL.GetApi(resolver);
    }

    public static GL GetGl()
    {
        if (_gl == null)
            throw new InvalidOperationException("Silk.NET GL resolver not initialized.");

        return _gl;
    }
}
