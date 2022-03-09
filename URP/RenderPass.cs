using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderPass : ScriptableRenderPass
{
    [Serializable]
    public class Settings
    {
        public RenderPassEvent Event;
        public RenderTargetIdentifier TargetTextureID;
    }
    
    protected Settings _settings;
    protected CommandBuffer _cmdBuff;

    protected Material _material;
    
    protected virtual string PassName => nameof(RenderPass);

    public RenderPass(Settings settings, Material material)
    {
        this.renderPassEvent = settings.Event;

        _material = material;
        
        _settings = settings;
    }
    
    public void PrepareBuffer(ref RenderingData renderingData)
    {
        _cmdBuff = CommandBufferPool.Get(PassName);
            
        var cam = renderingData.cameraData.camera;
        
        _cmdBuff.SetViewProjectionMatrices(cam.worldToCameraMatrix, cam.projectionMatrix);
        
        _cmdBuff.SetRenderTarget(_settings.TargetTextureID);
        _cmdBuff.ClearRenderTarget(true, true, Color.clear);
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Must be prefaced with PrepareBuffer & EnqueueRenderer
        if (_cmdBuff == null) return;
            
        var cam = renderingData.cameraData.camera;
            
        context.ExecuteCommandBuffer(_cmdBuff);
            
        CommandBufferPool.Release(_cmdBuff);
        _cmdBuff = null;
    }

    public void EnqueueRenderers(Renderer[] renderers, bool includeInactive)
    {
        for (int i = 0; i < renderers.Length; ++i)
            EnqueueRenderer(renderers[i], includeInactive);
    }
    
    public void EnqueueRenderer(Renderer renderer, bool includeInactive)
    {
        if (_cmdBuff == null)
        {
            Debug.LogError("!! You must prepare the command buffer first.");
            return;
        }
            
        if (!renderer) return;
        if (!includeInactive && (!renderer.enabled || !renderer.gameObject.activeInHierarchy)) return;

            
        var subMeshCount = renderer.sharedMaterials.Length;
        for (int submeshIndex = 0; submeshIndex < subMeshCount; ++submeshIndex)
        {
            _cmdBuff.DrawRenderer(renderer, _material, submeshIndex, 0);
        }
    }
}