using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(RenderOrders.GizmoPickingRead)]
public class GLReadPickingBufferSystem : RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoPickingRead;
    private int _readPickingIndex;
    private IViewPort _viewport;

    
    public GLReadPickingBufferSystem(ComponentManager componentManager) : base(componentManager)
    {
    }
    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        _viewport = renderContext.ViewPort;

        //Get the entity that holds the pickingdatacomponent

        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<SelectableDataComponent>();
        if (pickingEntity.IsEmpty) return;
        var pickingDataComponent = ComponentManager.GetComponent<SelectableDataComponent>(pickingEntity[0]);

        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (gizmoEntities.IsEmpty) return;

        


        if (frameInput.IsMouseLeftButtonDown)
        {
         var objectId = ReadPickingId(pickingDataComponent);
        }
        //set the selected entity as the hovered entity
    }

    private int ReadPickingId(SelectableDataComponent selectableDataComponent)
    {
        int objectHoveringId = 0;
        unsafe
        {
            GL.BindBuffer(BufferTarget.PixelPackBuffer,
                _viewport.SelectionRenderView.PixelBuffers[selectableDataComponent.BufferPickingIndex]);
            var pboPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
            if (pboPtr != (void*)IntPtr.Zero)
                objectHoveringId = (int)Marshal.PtrToStructure((IntPtr)pboPtr, typeof(int));
            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
        }

        return objectHoveringId;
    }
}