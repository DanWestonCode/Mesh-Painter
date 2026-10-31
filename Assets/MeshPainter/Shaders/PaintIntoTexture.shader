Shader "MeshPainter/PaintIntoTexture"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off
        
        Blend One OneMinusSrcAlpha
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 clipPosition : SV_POSITION;
                float3 worldPosition : TEXCOORD0;
            };

            float _brushOpacity;
            float _brushStrength;
            float _brushSize;
            float4 _mousePosition;
            float4 _paintColour;
            float4x4 _objectToWorld;

            Varyings vert(Attributes input)
            {
                Varyings output;

                float2 remappedUV = input.uv;
                // Handle inverted v coords
#if UNITY_UV_STARTS_AT_TOP
                remappedUV.y = 1.0 - remappedUV.y;
#endif

                // Clip space is -1/1 
                remappedUV = remappedUV * 2.0 - 1.0;
                output.clipPosition = float4(remappedUV.xy, 0.0, 1.0);
                //  Get the world space position of the vertex so we can compare against the mouse position 
                output.worldPosition = mul(_objectToWorld, float4(input.vertex.xyz, 1.0)).xyz;
                return output;
            }

            half4 frag(Varyings i) : SV_Target
            {
                const float minValue = 1e-5;
                
                //  How far is this point from the set mouse/brush position?
                float dist = distance(_mousePosition.xyz, i.worldPosition);
                //  Determine the radius of "solid" colour
                float edge0 = (_brushSize * _brushStrength);
                //  Determine the point where colour will end. 
                float edge1 = max(_brushSize, edge0 + minValue); // chuck on min value to avoid undefined on smoothstep (1,1)
                //  interpolate between edge0 and 1 to get the correct amount of colour.
                float brush = 1.0f-smoothstep(edge0, edge1, dist);
                float alpha = brush * _brushOpacity * _mousePosition.w; // _mousePosition.w = 1 means the user is clicking
                return half4(_paintColour.rgb * alpha, alpha);
            }
            ENDHLSL
        }
    }
}