Shader "Custom/UniColorPicker/HueShader"
{
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
            
            #define _COLORCOLOR_ON
            #include "UnityStandardParticles.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half3 hsv = half3(i.uv.y, 1, 1);
                half3 rgb = HSVtoRGB(hsv);
                return fixed4(rgb.x, rgb.y, rgb.z, 1);
            }
            ENDCG
        }
    }
}