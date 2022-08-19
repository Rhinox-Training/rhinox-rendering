using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhinox.Rendering.Extensions
{
    public static class RenderUtility
    {
        public static Color GetColor(Renderer renderer, int propertyID, bool propertyBlockIfAvailable)
        {
            var block = new MaterialPropertyBlock();
            if (propertyBlockIfAvailable && renderer.HasPropertyBlock())
            {
                renderer.GetPropertyBlock(block);
                return block.GetColor(propertyID);
            }

            return renderer.sharedMaterial.GetColor(propertyID);
        }
        
        public static MaterialPropertyBlock SetColor(Renderer renderer, int propertyID, Color color)
        {
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor(propertyID, color);
            renderer.SetPropertyBlock(block);
            return block;
        }
    }
}