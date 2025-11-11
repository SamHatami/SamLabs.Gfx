using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Run;
using SamLabs.Gfx.Wpf.ViewModels;
using System.Windows;

namespace SamLabs.Gfx.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider ServiceProvider { get; }
    public App()
    {
        ServiceProvider = CompositionRoot.Configure();
    }

    protected override void OnStartup(StartupEventArgs e)
    {

        base.OnStartup(e);

        BindViewModelsToViews();

    }

    private void BindViewModelsToViews()
    {
        var mainView = ServiceProvider.GetRequiredService<MainView>();
        var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

        mainView.DataContext = mainViewModel;

        this.MainWindow = mainView;
        mainView.Show();


    }
}