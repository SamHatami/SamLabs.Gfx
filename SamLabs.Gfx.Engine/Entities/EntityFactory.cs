using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Entities;

public class EntityFactory
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, EntityBlueprint> _blueprintRegistry = new();

    public EntityFactory(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IServiceProvider serviceProvider)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _serviceProvider = serviceProvider;
        RegisterBlueprints();
    }

    private void RegisterBlueprints()
    {
        var blueprintTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(EntityBlueprint).IsAssignableFrom(t));

        foreach (var type in blueprintTypes)
        {
            try
            {
                var blueprint = (EntityBlueprint)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                RegisterBlueprint(blueprint);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not register blueprint {type.Name}: {e.Message}");
            }
        }
    }

    public void RegisterBlueprint(EntityBlueprint blueprint)
    {
        _blueprintRegistry[blueprint.Name] = blueprint;
    }

    public Entity? CreateFromBlueprint(string name)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;
        
        var entity = _entityRegistry.CreateEntity();
        blueprint.Build(entity);
        
        return entity;
    }
    
    public Entity? CreateFromImport(string name, MeshDataComponent meshData)
    {
        if (!_blueprintRegistry.TryGetValue(name, out var blueprint))
            return null;
        
        var entity = _entityRegistry.CreateEntity();
        blueprint.Build(entity, meshData);
        
        return entity;
    }
}