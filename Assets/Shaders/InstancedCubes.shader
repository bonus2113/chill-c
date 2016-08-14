Shader "Unlit/InstancedCubes"
{
	Properties
	{
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

			#pragma target 5.0
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 color : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			struct CellInformation {
				float3 pos;
				float  _pad1;
				float3 size;
				float  _pad2;
				float4 color;
			};

			StructuredBuffer<float3> VertexBuffer;
			StructuredBuffer<CellInformation> PointBuffer;
			int Resolution;

			v2f vert (uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
			{
			    CellInformation cell = PointBuffer.Load(instanceId);

			    float4 pos = float4(VertexBuffer.Load(vertexId) * 0.75/Resolution + cell.pos, 1);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, pos);
				o.color = cell.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
