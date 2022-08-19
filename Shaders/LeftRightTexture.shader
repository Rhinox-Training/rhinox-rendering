Shader "Hidden/LeftRightTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                int eyeIndex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            int _ShowLeft = true;
            sampler2D _LeftTex;
            float4 _LeftTex_ST;

            int _ShowRight = true;
            sampler2D _RightTex;
            float4 _RightTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.eyeIndex = unity_StereoEyeIndex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 colL = _ShowLeft ? tex2D(_LeftTex, i.uv) : col;
                fixed4 colR = _ShowRight ? tex2D(_LeftTex, i.uv) : col;

                return i.eyeIndex ? colR : colL;
            }
            ENDCG
        }
    }
}
