// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Run;
using SamLabs.Gfx.Viewer.Framework;

var serviceProvider = CompositionRoot.ConfigureServices();

var logger = serviceProvider.GetService<ILogger<Program>>();

try
{
    
    //main window
    //Create thread for rendering
    //Create background worker thread for loading assets
    //Create background worker thread for logic updates
    var sceneManger = serviceProvider.GetService<ISceneManager>();
    var renderer = serviceProvider.GetService<Renderer>();
    var defaultScene = sceneManger.GetCurrentScene();
    var window = serviceProvider.GetService<Window>();
    window.SetRenderer(renderer); //this is not nice

    window.Run(defaultScene);

}
catch (Exception e)
{
    logger.LogError(e, "Could not start window");
}