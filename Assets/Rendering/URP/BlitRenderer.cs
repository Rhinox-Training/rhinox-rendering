using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rhinox.Rendering.Universal
{
    [Serializable]
    public class BlitRenderer : ScriptableRendererFeature
    {
        [Serializable]
        public struct BlitSettings
        {
            public RenderPassEvent Event;
            public Material Material;

            public RenderTexture TargetTexture;
        }

        public BlitSettings settings;

        private BlitPass _pass;

        public override void Create()
        {
            _pass = new BlitPass(settings.Event, settings.Material, 0, name);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _pass.Setup(renderer.cameraColorTarget, settings.TargetTexture);

            renderer.EnqueuePass(_pass);
        }
    }
}