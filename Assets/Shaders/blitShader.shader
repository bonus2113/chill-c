Shader "Unlit/blitShader"
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
			Blend SrcAlpha OneMinusSrcAlpha

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
				float uvStep : TEXCOORD4;
				float4 posSizeUVSpace : TEXCOORD5;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform int _TexSize;
			uniform float4 _PosSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvStep = 1.0f / _TexSize;
				o.posSizeUVSpace = _PosSize/_TexSize;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,1);

				// sample the texture
				float2 strokeUVPos = (i.uv - i.posSizeUVSpace.xy) / i.posSizeUVSpace.zw;

				float2 uvPos2 = i.posSizeUVSpace.xy;
				uvPos2.x = uvPos2.x - 1.0f;
				//float2 strokeUVPos2 = (i.uv - uvPos2) / i.posSizeUVSpace.zw;

				fixed4 strokeCol = tex2D(_MainTex, strokeUVPos);
				
				clip(strokeCol.a - 0.075f);
				
				//fixed4 strokeCol2 = tex2D(_MainTex, strokeUVPos2);
				col.xyz += fixed3(1, 1, 1);
				//col.a = 1.0f;
				col.a = strokeCol.a;
				return col;
			}
			ENDCG
		}
	}
}
