Shader "Unlit/Ground"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GardenMap("GardenRenderTex", 2D) = "black"{}
		_GardenMap2("GardenRenderTex2", 2D) = "black"{}
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
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				LIGHTING_COORDS(3, 4)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GardenMap;
			sampler2D _GardenMap2;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = float4(float2(-v.vertex.x, -v.vertex.z)*0.8 + float2(0.5, 0.5),0,0);
				o.color = v.color;

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				//sample the texture
				fixed4 garden = tex2D(_GardenMap, i.uv);
				fixed4 garden2 = tex2D(_GardenMap2, i.uv);
				half4 col = tex2D(_MainTex, i.uv) * saturate(garden + garden2) * i.color;
				//half4 col = frac(i.uv);
				//half4 col = tex2D(_MainTex, i.uv);
				float atten = LIGHT_ATTENUATION(i);
				return col*atten;
				return garden;
			}
			ENDCG
		}
	}Fallback "VertexLit"
}
