// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Run;


var serviceProvider = CompositionRoot.ConfigureServices();

var logger = serviceProvider.GetService<ILogger<Program>>();

try
{
    var sceneManger = serviceProvider.GetService<ISceneManager>();
    var defaultScene = sceneManger.GetCurrentScene();
    sceneManger.Run(defaultScene);
}
catch (Exception e)
{
    logger.LogError(e, "Could not start window");
}