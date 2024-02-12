#if UNIVERSAL_PIPELINE
using Rhinox.Rendering.Universal;
#endif

namespace Rhinox.Rendering.Extensions
{
    public enum RenderPipelineType
    {
        Auto,
        BuiltIn,
        URP,
        HDRP
    }

    public static class RenderPipelineUtility
    {
        // TODO add support for HDRP
        public static RenderPipelineType ResolveRenderPipelineType()
        {
#if UNIVERSAL_PIPELINE
            if (UrpUtility.IsInURP())
                return RenderPipelineType.URP;
#endif

            return RenderPipelineType.BuiltIn;
        }
    }
}