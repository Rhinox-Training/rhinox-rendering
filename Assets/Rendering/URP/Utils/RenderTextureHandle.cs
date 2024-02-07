using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rhinox.Rendering.Universal
{
    public class RenderTextureHandle
    {
#if UNITY_2022_1_OR_NEWER
        RTHandle rtHandle;
#else
        RenderTargetHandle renderTargetHandle;
#endif

        public RenderTextureHandle(string prop)
        {
#if UNITY_2022_1_OR_NEWER
            rtHandle = RTHandles.Alloc(prop, name: prop);
#else
            renderTargetHandle.Init(prop);
#endif
        }
        
        public void GetTemporaryRT(CommandBuffer cmd, RenderTextureDescriptor desc, FilterMode filter)
        {
#if UNITY_2022_1_OR_NEWER
            cmd.GetTemporaryRT(Shader.PropertyToID(rtHandle.name), desc, filter);
#else
            cmd.GetTemporaryRT(renderTargetHandle.id, desc, filter);
#endif
        }

        public void ReleaseTemporaryRT(CommandBuffer cmd)
        {
#if UNITY_2022_1_OR_NEWER
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(rtHandle.name));
#else
            cmd.ReleaseTemporaryRT(renderTargetHandle.id);
#endif
        }
        
        public static implicit operator RenderTargetIdentifier(RenderTextureHandle handle)
        {
#if UNITY_2022_1_OR_NEWER
            return handle.rtHandle;
#else         
            return handle.renderTargetHandle.id;
#endif
        }
    }
}