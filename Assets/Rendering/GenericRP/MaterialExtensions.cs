using UnityEngine;
using UnityEngine.Rendering;
#if UNIVERSAL_PIPELINE
using Rhinox.Rendering.Universal;
#endif

namespace Rhinox.Rendering.Extensions
{
    public static class MaterialExtensions
    {
        static class ShaderConstants
        {
            public static readonly int Color = Shader.PropertyToID("_Color");
            public static readonly int ColorURP = Shader.PropertyToID("_BaseColor");
        }


        private static int GetColorProperty(RenderPipelineType pipeline)
        {
            if (pipeline == RenderPipelineType.Auto)
                pipeline = RenderPipelineUtility.ResolveRenderPipelineType();

            switch (pipeline)
            {
                case RenderPipelineType.BuiltIn:
                default:
                    return ShaderConstants.Color;
            }
        }

        
        public static Color GetColor(this Renderer ren) => GetColor(ren, true);

        public static Color GetColor(this Renderer ren, bool propertyBlockIfAvailable)
        {
            var block = new MaterialPropertyBlock();
            if (propertyBlockIfAvailable && ren.HasPropertyBlock())
            {
                ren.GetPropertyBlock(block);
                return block.GetColor(ShaderConstants.ColorURP);
            }

            return ren.sharedMaterial.GetColor(ShaderConstants.ColorURP);
        }

        public static void SetColor(this Renderer ren, Color color)
        {
            var block = new MaterialPropertyBlock();
            ren.GetPropertyBlock(block);
            block.SetColor(ShaderConstants.ColorURP, color);
            ren.SetPropertyBlock(block);
        }
    }
}