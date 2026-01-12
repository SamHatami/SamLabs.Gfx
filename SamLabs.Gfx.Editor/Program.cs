using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.OpenGL;

namespace SamLabs.Gfx.Editor;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .With(new Win32PlatformOptions
            {
                RenderingMode = (Collection<Win32RenderingMode>)[Win32RenderingMode.Wgl],
                WglProfiles = new[] 
                { 
                    new GlVersion(GlProfileType.OpenGL, 4, 6)  // Or whatever version you need
                }
                
            }) //This line is the important one.
            .LogToTrace()
            .WithDeveloperTools();//Sam:for Avalonia devtools

    }
}