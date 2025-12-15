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
        if (frameInput.IsMouseLeftButtonDown)
        {
            pickingDataComponent.HoveredEntityId = -1;
            pickingDataComponent.SelectedEntityIds = [objectId];
        }

        //Clear the hovered entity
    }

    private int ReadPickingId(PickingDataComponent pickingData)
    {
        int objectHoveringId = 0;
        int pboId = _viewport.SelectionRenderView.PixelBuffers[pickingData.BufferPickingIndex^1];
        unsafe
        {
            GL.BindBuffer(BufferTarget.PixelPackBuffer, pboId);
            void* rawPtr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
        
            if (rawPtr != (void*)0)
            {
                objectHoveringId = *(int*)rawPtr;
            
                // Note: If you ever return a null/0 ID, this is where you'd catch it.
            }

            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
        }

        return objectHoveringId;
    }
}