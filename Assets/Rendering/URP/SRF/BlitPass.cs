
using System.Collections.Generic;
using Rhinox.Lightspeed;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rhinox.Rendering.Universal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class BlitPass : ScriptableRenderPass
    {
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }
        
        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        protected RenderTargetIdentifier source { get; set; }
        protected RenderTargetIdentifier destination { get; set; }

        protected RenderTargetHandle m_TemporaryColorTexture;
        protected string m_ProfilerTag;

        private Dictionary<int, Texture> _availableTextures;

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag)
            : this(renderPassEvent, tag)
        {
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
        }
        
        public BlitPass(RenderPassEvent renderPassEvent, string tag)
            : this(tag)
        {
            this.renderPassEvent = renderPassEvent;
        }
        
        public BlitPass(string tag)
        {
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");
        }
        

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(int source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination.id;
        }
        
        public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            this.source = source;
            this.destination = destination;
        }
        
        public void Setup(int source, int destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void RegisterTexture(int id, Texture tex)
        {
            if (_availableTextures == null)
                _availableTextures = new Dictionary<int, Texture>();
            _availableTextures[id] = tex;
        }
        
        public void RegisterTexture(string tag, Texture tex) => RegisterTexture(Shader.PropertyToID(tag), tex);

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            // desc.depthBufferBits = 0;

            if (!_availableTextures.IsNullOrEmpty())
            {
                foreach (var (id, tex) in _availableTextures)
                    cmd.SetGlobalTexture(id, tex);
            }

            // Can't read and write to same color target, create a temp render target to blit. 
            if (destination == RenderTargetHandle.CameraTarget.id || destination == source)
            {
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, desc, filterMode);
                
                if (blitMaterial == null)
                    cmd.Blit(source, m_TemporaryColorTexture.Identifier());
                else 
                    cmd.Blit(source, m_TemporaryColorTexture.Identifier(), blitMaterial, blitShaderPassIndex);
                
                cmd.Blit(m_TemporaryColorTexture.Identifier(), destination);
                
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
            }
            else
            {
                cmd.Blit(source, destination, blitMaterial, blitShaderPassIndex);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}