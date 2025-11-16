using System;
using Avalonia.OpenGL;
using OpenTK;

//Source: https://github.com/DigitalBox98/Avalonia-OpenTK-Sample/tree/main
//https://opentk.net/api/OpenTK.IBindingsContext.html
namespace SamLabs.Gfx.StandAlone.Models.OpenTk;

/// <summary>
/// Wrapper to expose GetProcAddress from Avalonia in a manner that OpenTK can consume. 
/// </summary>
class AvaloniaTkContext : IBindingsContext
{
    private readonly GlInterface _glInterface;

    public AvaloniaTkContext(GlInterface glInterface)
    {
        _glInterface = glInterface;
    }

    public IntPtr GetProcAddress(string procName) => _glInterface.GetProcAddress(procName);
}