Shader "Hidden/LeftRightTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
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

                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                int eyeIndex : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO 
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

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                        
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = UnityStereoTransformScreenSpaceTex(v.uv);
                o.eyeIndex = unity_StereoEyeIndex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                
                fixed4 col = tex2D(_MainTex, uv);
                fixed4 colL = _ShowLeft ? tex2D(_LeftTex, uv) : col;
                fixed4 colR = _ShowRight ? tex2D(_RightTex, uv) : col;

                return i.eyeIndex ? colR : colL;
            }
            ENDCG
        }
    }
}
