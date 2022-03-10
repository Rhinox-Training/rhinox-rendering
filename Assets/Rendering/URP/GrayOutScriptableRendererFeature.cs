using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rhinox.Rendering
{
    public class GrayOutScriptableRendererFeature : ScriptableRendererFeature
    {
        private const string _shader = "Custom/HSV_Shader";

        private Material _material;

        private RenderTexture _tempTexture;

        public override void Create()
        {
            _material = new Material(Shader.Find(_shader));
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            if (_tempTexture) RenderTexture.ReleaseTemporary(_tempTexture);

            _tempTexture = RenderTexture.GetTemporary(desc);
            _tempTexture.name = "Grayout Temp Target";

            var pass = new BlitPass(RenderPassEvent.AfterRenderingSkybox, _material, 0, "Gray Blit Pass");
            pass.Setup(renderer.cameraColorTarget, _tempTexture);
            renderer.EnqueuePass(pass);

            pass = new BlitPass(RenderPassEvent.AfterRenderingSkybox, "Final Blit Pass");
            pass.Setup(_tempTexture, renderer.cameraColorTarget);
            renderer.EnqueuePass(pass);
        }
    }
}