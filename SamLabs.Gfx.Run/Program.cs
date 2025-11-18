// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Run;
using SamLabs.Gfx.Viewer.Display;
using SamLabs.Gfx.Viewer.Interfaces;

var serviceProvider = CompositionRoot.ConfigureServices();

var logger = serviceProvider.GetService<ILogger<Program>>();

try
{

    var glVersion =GL.GetString(StringName.Version);
    var glslVersion = GL.GetString(StringName.ShadingLanguageVersion);

    //main window
    //Create thread for rendering
    //Create background worker thread for loading assets
    //Create background worker thread for logic updates
    var sceneManger = serviceProvider.GetService<ISceneManager>();
    var renderer = serviceProvider.GetService<IRenderer>();
    
    var defaultScene = sceneManger.GetCurrentScene();
    var window = serviceProvider.GetService<ViewerWindow>();
    
    window.SetRenderer(renderer); //this is not nice

    window.Run(defaultScene);

}
catch (Exception e)
{
    logger.LogError(e, "Could not start window");
}