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
    
    [AttributeUsage(AttributeTargets.Class)]
    public class MainEnd : Attribute
    {
        public int Order { get; } = RenderPassOrders.MainRenderPass;
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class GizmoPickingStart() : Attribute
    {
        public int Order { get; } = RenderPassOrders.GizmoPickingRenderPass;
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class GizmoPickingEnd() : Attribute
    {
        public int Order { get; } = RenderPassOrders.GizmoPickingRenderPass;
    }

}