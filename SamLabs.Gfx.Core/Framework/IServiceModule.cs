using Microsoft.Extensions.DependencyInjection;

namespace SamLabs.Gfx.Core.Framework;

public interface IServiceModule
{
    IServiceCollection RegisterServices(IServiceCollection services);
}