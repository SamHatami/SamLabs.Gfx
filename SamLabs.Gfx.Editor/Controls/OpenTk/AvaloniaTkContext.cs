using System;
using Avalonia.OpenGL;

namespace SamLabs.Gfx.Editor.Controls.OpenTk;

/// <summary>
/// Small adapter that exposes Avalonia's proc resolver in a form Silk.NET can consume.
/// </summary>
internal sealed class AvaloniaTkContext
{
    private readonly GlInterface _glInterface;

    public AvaloniaTkContext(GlInterface glInterface)
    {
        _glInterface = glInterface;
    }

    public IntPtr GetProcAddress(string procName) => _glInterface.GetProcAddress(procName);
}
