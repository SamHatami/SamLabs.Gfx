using Avalonia.OpenGL;

namespace SamLabs.Gfx.StandAlone.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel()
    {
        var glContextInfo = new GlInterface.GlContextInfo(new GlVersion(GlProfileType.OpenGL, 4, 6), null);
    }
}