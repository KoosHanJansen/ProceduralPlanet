Shader "Planet/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "../Include/Math.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
				o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                return o;
            }

            sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			float3 planetCenter, dirToSun, scatteringCoefficients;
			float planetRadius, atmosphereRadius, densityFallofff, intensity;
			int numOpticalDepthPoints, numInScatteringPoints;

			//Returns density value between 0 and 1
			float densityAtPoint(float3 densitySamplePoint) //Nothing wrong here 
			{
				float heightAboveSurface = length(densitySamplePoint - planetCenter) - planetRadius;
				float height01 = heightAboveSurface / (atmosphereRadius - planetRadius);
				float localDensity = exp(-height01 * densityFallofff) * (1 - height01);
				return localDensity;
			}

			float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength)
			{
				float3 densitySamplePoint = rayOrigin;
				float stepSize = rayLength / (numOpticalDepthPoints - 1);
				float opticalDepth = 0;

				for (int i = 0; i < numOpticalDepthPoints; i++) {
					float localDensity = densityAtPoint(densitySamplePoint);
					opticalDepth += localDensity * stepSize;
					densitySamplePoint += rayDir * stepSize;
				}

				return opticalDepth;
			}

			float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol)
			{
				float3 inScatterPoint = rayOrigin;
				float stepSize = rayLength / (numInScatteringPoints - 1);
				float3 inScatteredLight = 0;
				float viewRayOpticalDepth = 0;

				for (int i = 0; i < numInScatteringPoints; i++)
				{
					float sunRayLength = raySphere(planetCenter, atmosphereRadius, inScatterPoint, dirToSun).y;
					float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
					viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i);
					float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * scatteringCoefficients);
					float localDensity = densityAtPoint(inScatterPoint);

					inScatteredLight += localDensity * transmittance * scatteringCoefficients * stepSize; 
					inScatterPoint += rayDir * stepSize;
				}
				float originalColTransmittance = exp(-viewRayOpticalDepth);
				return originalCol * originalColTransmittance + inScatteredLight;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 originalCol = tex2D(_MainTex, i.uv);
				float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);

				float3 rayOrigin = _WorldSpaceCameraPos;
				float3 rayDir = normalize(i.viewVector);

				float2 hitInfo = raySphere(planetCenter, atmosphereRadius, rayOrigin, rayDir);
				float dstToAtmosphere = hitInfo.x;
				float dstThroughAtmosphere = min(hitInfo.y, sceneDepth - dstToAtmosphere);

				if (dstThroughAtmosphere > 0)
				{
					float3 pointInAtmosphere = rayOrigin + rayDir * dstToAtmosphere;
					float3 light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere, originalCol);
					return float4(light, 0);
				}
				
				//return dstThroughAtmosphere / (atmosphereRadius * 2);// * float4(rayDir.rgb * 0.5 + 0.5, 0);
				return originalCol;
            }
            ENDCG
        }
    }
}
