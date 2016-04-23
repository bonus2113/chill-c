Shader "Unlit/FountainRaymarch"
{
	Properties
	{
		_DensityTex ("Density", 3D) = "white" {}
    _VolumeDimensions("VolumeDimensions", Vector) = (0, 0, 0)
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
			// make fog work
			#pragma multi_compile_fog
      #pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
        float3 worldPos : TEXCOORD1;
			};

			sampler3D _DensityTex;
			float4 _DensityTex_ST;

      float3 _VolumeDimensions;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.worldPos = mul(_Object2World, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.uv, _DensityTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
			}


      bool greaterThanEqual(float3 value, float3 comparison) {
        return (value.x >= comparison.x && value.y >= comparison.y && value.z >= comparison.z);
      }

      bool lessThan(float3 value, float3 comparison) {
        return (value.x < comparison.x && value.y < comparison.y && value.z < comparison.z);
      }

      struct PS_Output {
        float4 col:COLOR;
        float dep : DEPTH;
      };
			
      PS_Output frag (v2f i) : SV_Target
      {

        float3 startPos = mul(_World2Object, float4(i.worldPos, 1)).xyz;

        float3 eyePos = mul(_World2Object, float4(_WorldSpaceCameraPos, 1)).xyz;
        float3 rayDir = normalize(startPos - eyePos);

        const float epsilon = 0.00001;
        if (abs(rayDir.x) <= epsilon) rayDir.x = epsilon * sign(rayDir.x);
        if (abs(rayDir.y) <= epsilon) rayDir.y = epsilon * sign(rayDir.y);
        if (abs(rayDir.z) <= epsilon) rayDir.z = epsilon * sign(rayDir.z);

        // Calculate inverse of ray direction once.
        float3 invRayDir = 1.0 / rayDir;

        float3 voxelPos = startPos + rayDir * 0.01;

        fixed4 finalColor = fixed4(0, 0, 0, 0);

        // Traverse through voxels until ray exits volume.
        [loop]
        while (greaterThanEqual(voxelPos, float3(-0.5, -0.5, -0.5)) && lessThan(voxelPos, float3(0.5, 0.5, 0.5))) {
          // Sample 3D texture at current position.
          float4 color = tex3Dlod(_DensityTex, float4(voxelPos + float3(0.5, 0.5, 0.5), 0));

          // Exit loop if a single sample has an alpha value greater than 0.
          if (color.a >= 0.1) {
            color.a = 1;
            finalColor = color;
            break;
          }

          // Move to next closest voxel along ray.
          float3 t0 = (voxelPos - startPos) * invRayDir;
          float3 t1 = (voxelPos + float3(1, 1, 1)/_VolumeDimensions - startPos) * invRayDir;
          float3 tmax = max(t0, t1);
          float t = min(tmax.x, min(tmax.y, tmax.z));
          if (tmax.x == t) voxelPos.x += sign(rayDir.x) / _VolumeDimensions;
          else if (tmax.y == t) voxelPos.y += sign(rayDir.y) / _VolumeDimensions;
          else if (tmax.z == t) voxelPos.z += sign(rayDir.z) / _VolumeDimensions;
        }

        if (finalColor.a <= 0.1) {
          discard;
        }

        float4 pos = float4(voxelPos, 1);
        PS_Output o;

        float4 v_clip_coord = mul(UNITY_MATRIX_VP, pos);
        float f_ndc_depth = v_clip_coord.z / v_clip_coord.w;
        o.dep = (1.0 - 0.0) * 0.5 * f_ndc_depth + (1.0 + 0.0) * 0.5;

				// sample the texture
        o.col = finalColor;

				return o;
			}
			ENDCG
		}
	}
}
