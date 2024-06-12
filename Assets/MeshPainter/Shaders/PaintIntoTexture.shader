Shader "MeshPainter/PaintIntoTexture"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD	 100
		ZTest  Off
		ZWrite Off
		Cull   Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct verts
			{
				float2 uv : TEXCOORD0;
				float3 worldPosition : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float _brushOpacity;
			float _brushStrength;
			float _brushSize;
			float4 _mousePosition;
			float4 _paintColour;
			float4x4 _objectToWorld;
			
			sampler2D _paintedTex;
			
			verts vert (appdata v)
			{
				verts o;

				float2 remappedUV = v.uv;
				// handle inverted v coords
				#if UNITY_UV_STARTS_AT_TOP
				remappedUV.y = 1. - remappedUV.y;
				#endif

				// Move UVs from 0/1 into -1/+1
				remappedUV = remappedUV * 2-1.;

				o.vertex = float4(remappedUV.xy, 0, 1);
				o.uv = v.uv;
				o.worldPosition = mul(_objectToWorld, v.vertex);
				return o;
			}

			fixed4 frag (verts i) : SV_Target
			{
				float4 col  = tex2D(_paintedTex, i.uv);
				float brush = 1.0f-smoothstep(_brushSize*_brushStrength, _brushSize, distance(_mousePosition.xyz, i.worldPosition));
				col = lerp(col, _paintColour, brush * _mousePosition.w);
				col = saturate(col);
				return col;
			}
			ENDCG
		}
	}
}
