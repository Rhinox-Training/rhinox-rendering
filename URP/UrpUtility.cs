using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rhinox.Rendering
{
    public static class UrpUtility
    {
        static class ShaderConstants
        {
            public static readonly int Color = Shader.PropertyToID("_BaseColor");
            public static readonly int AlbedoTexture = Shader.PropertyToID("_BaseMap");
        }
    
        public static bool TryGetAsset(out UniversalRenderPipelineAsset asset)
        {
            asset = null;
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset)
            {
                asset = urpAsset;
                return true;
            }
            return false;
        }

        public static bool IsInURP()
        {
            return TryGetAsset(out _);
        }

        public static Color GetColor(Material mat)
        {
            return mat.GetColor(ShaderConstants.Color);

        }
    
        public static void SetColor(Material mat, Color color)
        {
            mat.SetColor(ShaderConstants.Color, color);
        }
    }
}