// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Run;
using SamLabs.Gfx.Viewer.Primitives;


var serviceProvider = CompositionRoot.ConfigureServices();

var logger = serviceProvider.GetService<ILogger<Program>>();

try
{
    var sceneManger = serviceProvider.GetService<ISceneManager>();
    var defaultScene = sceneManger.GetCurrentScene();
    defaultScene.AddRenderable(new Plane());
    defaultScene.AddRenderable(Triangle.CreateSimpleTriangle());
    sceneManger.Run(defaultScene);
}
catch (Exception e)
{
    logger.LogError(e, "Could not start window");
}