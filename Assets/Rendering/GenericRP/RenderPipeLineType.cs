#if UNIVERSAL_PIPELINE
using Rhinox.Rendering.Universal;
#endif

namespace Rhinox.Rendering.Extensions
{
    public enum RenderPipeLineType
    {
        Auto,
        BuiltIn,
        URP,
        HDRP
    }

    public static class RenderPipelineUtility
    {
        // TODO add support for HDRP
        public static RenderPipeLineType ResolveAutoRenderPipelineType()
        {
#if UNIVERSAL_PIPELINE
            if (UrpUtility.IsInURP())
                return RenderPipeLineType.URP;
#endif

            return RenderPipeLineType.BuiltIn;
        }
    }
}