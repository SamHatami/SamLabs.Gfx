using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
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
    private int _pickingEntity;


    public GLReadPickingBufferSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }
    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        _viewport = renderContext.ViewPort;

        //Get the entity that holds the pickingdatacomponent

        if(_pickingEntity == -1)
            _pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>()[0];
        ref var pickingDataComponent = ref ComponentManager.GetComponent<PickingDataComponent>(_pickingEntity);
        var gizmoEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (gizmoEntities.IsEmpty) return;

        
        var objectId = ReadPickingId(pickingDataComponent);
        pickingDataComponent.HoveredEntityId = objectId;
        Console.WriteLine(objectId);
        // if (frameInput.IsMouseLeftButtonDown)
        // {
        //     pickingDataComponent.HoveredEntityId = -1;
        //     pickingDataComponent.SelectedEntityIds = [objectId];
        // }

        //Clear the hovered entity
    }

    private int ReadPickingId(PickingDataComponent pickingDataComponent)
    {
        int objectHoveringId = 0;
        unsafe
        {
            GL.BindBuffer(BufferTarget.PixelPackBuffer,
                _viewport.SelectionRenderView.PixelBuffers[pickingDataComponent.BufferPickingIndex^1]);
            var pboPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
            if (pboPtr != (void*)IntPtr.Zero)
                objectHoveringId = (int)Marshal.PtrToStructure((IntPtr)pboPtr, typeof(int));
            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
        }

        return objectHoveringId;
    }
}