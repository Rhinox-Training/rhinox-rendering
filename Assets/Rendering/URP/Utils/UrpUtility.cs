using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rhinox.Lightspeed;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// #if UNITY_2021_2_OR_NEWER
// using UrpRendererData = UnityEngine.Rendering.Universal.ScriptableRendererData;
// #else
// using UrpRendererData = UnityEngine.Rendering.Universal.ForwardRendererData;
// #endif


namespace Rhinox.Rendering.Universal
{
    public static class UrpUtility
    {
        private static FieldInfo _rendererDataListFieldInfo;
        private static FieldInfo _defaultRendererDataFieldInfo;

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
        public static ScriptableRendererData[] GetRendererDatas()
        {
            if (!TryGetAsset(out var asset))
                return Array.Empty<ScriptableRendererData>();
        
            if (_rendererDataListFieldInfo == null)
                _rendererDataListFieldInfo = asset.GetType()
                    .GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            var value = _rendererDataListFieldInfo.GetValue(asset);
            if (value == null)
                Debug.LogError("m_RendererDataList seems to have changed signature. FIX ME!");
            
            return _rendererDataListFieldInfo.GetValue(asset) as ScriptableRendererData[];
        }
        
        private static int GetDefaultRendererIndex(UniversalRenderPipelineAsset asset)
        {
            if (_defaultRendererDataFieldInfo == null)
                _defaultRendererDataFieldInfo = asset.GetType()
                    .GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var value = _defaultRendererDataFieldInfo.GetValue(asset);
            if (value == null)
                Debug.LogError("m_DefaultRendererIndex seems to have changed signature. FIX ME!");

            return (int) value;
        }

        public static ScriptableRendererData GetDefaultRendererData()
        {
            var datas = GetRendererDatas();
            if (datas.IsNullOrEmpty())
                return null;
            int defaultRendererI = GetDefaultRendererIndex(UniversalRenderPipeline.asset);

            return datas[defaultRendererI];
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