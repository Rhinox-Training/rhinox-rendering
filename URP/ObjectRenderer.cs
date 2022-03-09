using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.Lightspeed;
using Rhinox.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class ObjectRenderer : ScriptableRendererFeature
{
    [Serializable]
    public struct ObjectRendererSettings
    {
        public RenderPassEvent Event;
        public int LayerMask;
        
        public Material Material;

        public List<ShaderTagId> ShaderPasses;

        [ToggleGroup(nameof(UseStencil), "Stencil")]
        public bool UseStencil;
        [ToggleGroup(nameof(UseStencil))]
        public int StencilReference;
        [HideReferenceObjectPicker, ToggleGroup(nameof(UseStencil))]
        public StencilState StencilState;
        
        [Tooltip("If empty, renders to the current camera.")]
        public RenderTexture TargetTexture;
    }

    public ObjectRendererSettings settings = new ObjectRendererSettings
    {
        Event = RenderPassEvent.BeforeRenderingSkybox,
        LayerMask = ~0
    };

    private ObjectRendererPass _pass;
    
    public override void Create()
    {
        RenderTargetIdentifier id;
        if (settings.TargetTexture == null)
            id = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
        else 
            id = new RenderTargetIdentifier(settings.TargetTexture);

        _pass = new ObjectRendererPass(id, settings.Material, settings.LayerMask)
        {
            renderPassEvent = settings.Event,
        };

        if (settings.UseStencil)
        {
            _pass.RenderStateBlock.stencilReference = settings.StencilReference;
            _pass.RenderStateBlock.stencilState = settings.StencilState;
        }

        if (!settings.ShaderPasses.IsNullOrEmpty())
        {
            foreach (var tagId in settings.ShaderPasses)
                _pass.ShaderTagIdList.AddUnique(tagId);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}

public class ObjectRendererPass : ScriptableRenderPass
{
    public List<ShaderTagId> ShaderTagIdList = new List<ShaderTagId>();

    public RenderStateBlock RenderStateBlock;
    public FilteringSettings FilteringSettings;

    private RenderTargetIdentifier _targetTextureID;
    private Material _material;

    protected string PassName => nameof(ObjectRendererPass);

    public ObjectRendererPass(RenderTargetIdentifier targetTextureID, Material material, int layerMask = ~0)
    {
        _targetTextureID = targetTextureID;
        _material = material;
        
        ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
        ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
        ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));

        RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        FilteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var desc = renderingData.cameraData.cameraTargetDescriptor;
            
        var cmdBuff = CommandBufferPool.Get(PassName);
            
        // var cam = renderingData.cameraData.camera;
        // cmdBuff.SetProjectionMatrix(cam.projectionMatrix);

        cmdBuff.SetRenderTarget(_targetTextureID);
        cmdBuff.ClearRenderTarget(true, true, Color.clear);

        DrawingSettings drawingSettings = CreateDrawingSettings(ShaderTagIdList, ref renderingData, SortingCriteria.BackToFront);
        drawingSettings.overrideMaterial = _material;
        drawingSettings.overrideMaterialPassIndex = 0;
        drawingSettings.enableDynamicBatching = true;
        drawingSettings.perObjectData = PerObjectData.None; // disable all lighting, reflection probe data, etc

        context.ExecuteCommandBuffer(cmdBuff);
        cmdBuff.Clear();
        
        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref FilteringSettings, ref RenderStateBlock);

        CommandBufferPool.Release(cmdBuff);
    }
}