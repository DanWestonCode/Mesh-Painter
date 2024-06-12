Shader "MeshPainter/UVUnwrap"
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

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;

				float2 uv = v.uv;
				// handle inverted v coords
				#if UNITY_UV_STARTS_AT_TOP
				uv.y = 1. - uv.y;
				#endif

				// Move UVs from 0/1 into -1/+1
				uv = uv * 2-1.;

				o.uv = uv;
				o.vertex = float4(o.uv,0,1);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return float4 (0,0,0,1.0f);
			}
			ENDCG
		}
	}
}
