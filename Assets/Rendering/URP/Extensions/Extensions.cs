using UnityEngine;

namespace Rhinox.Rendering.Extensions
{
    public static class UrpExtensions
    {
        static class ShaderConstants
        {
            public static readonly int ColorURP = Shader.PropertyToID("_BaseColor");
        }

        public static Color ColorURP(this Material mat)
        {
            return mat.GetColor(ShaderConstants.ColorURP);
        }

        public static void ColorURP(this Material mat, Color color)
        {
            mat.SetColor(ShaderConstants.ColorURP, color);
        }

        public static Color ColorURP(this Renderer ren) => ColorURP(ren, true);

        public static Color ColorURP(this Renderer ren, bool propertyBlockIfAvailable)
        {
            var block = new MaterialPropertyBlock();
            if (propertyBlockIfAvailable && ren.HasPropertyBlock())
            {
                ren.GetPropertyBlock(block);
                return block.GetColor(ShaderConstants.ColorURP);
            }

            return ren.sharedMaterial.GetColor(ShaderConstants.ColorURP);
        }

        public static void ColorURP(this Renderer ren, Color color)
        {
            var block = new MaterialPropertyBlock();
            ren.GetPropertyBlock(block);
            block.SetColor(ShaderConstants.ColorURP, color);
            ren.SetPropertyBlock(block);
        }
    }
}