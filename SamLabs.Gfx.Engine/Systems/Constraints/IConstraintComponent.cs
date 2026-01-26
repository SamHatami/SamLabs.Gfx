using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Systems.Constraints;

public interface IConstraintComponent:IComponent
{
    //Just for a marker for outside systems to identify constraint components, Sam has an idea of some type collection
    //so i can ask for all components that implement IConstraintComponent to turn off and on visibility or something
}