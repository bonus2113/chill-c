Shader "Unlit/Ground"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GardenMap("GardenRenderTex", 2D) = "black"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "LightMode" = "ForwardBase"  }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fwdadd_fullshadows

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				LIGHTING_COORDS(3, 4)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GardenMap;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 garden = tex2D(_GardenMap, i.uv);
				fixed4 col = tex2D(_MainTex, i.uv) * garden;
				float atten = LIGHT_ATTENUATION(i);
				return col*atten;
				//return garden;
			}
			ENDCG
		}
	}Fallback "VertexLit"
}
