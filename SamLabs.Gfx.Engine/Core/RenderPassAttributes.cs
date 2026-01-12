using SamLabs.Gfx.Engine.Core.Utility;

namespace SamLabs.Gfx.Engine.Core;

public class RenderPassAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MainStart() : Attribute
    {
        public int Order { get; } = RenderPassOrders.MainRenderPass;
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class RenderOrder(int order) : Attribute
    {
        public int Order { get; } = order;
    }


}