using SamLabs.Gfx.Viewer.Core.Utility;

namespace SamLabs.Gfx.Viewer.Core;

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