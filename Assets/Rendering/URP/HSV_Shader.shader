Shader "Custom/HSV_Shader"
{
    Properties {
        _MainTex("Texture", 2D)                         = "white" {}
    }
    SubShader {
        Pass {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            TEXTURE2D_X(_MainTex);
            
            float4 _MainTex_TexelSize;
            
            float4 toGray(float4 color){
                return (color.x + color.y + color.z)/3;
            }
            
            float4 frag(Varyings i) : SV_Target {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
                float4 color = LOAD_TEXTURE2D_X(_MainTex, _MainTex_TexelSize.zw * uv);

                return toGray(color);
            }
            ENDHLSL
        }
    }
}