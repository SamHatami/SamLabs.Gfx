using Microsoft.Extensions.DependencyInjection;

namespace SamLabs.Gfx.Core.Framework.Display;

public interface IServiceModule
{
    IServiceCollection RegisterServices(IServiceCollection services);
}