Shader "Custom/ChunkVertexShader"
{
    Properties
    {
		_Color ("Color", color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RadiusFromOrigin ("Radius from Origin", float) = 4096
		_NoiseScale ("Noise Scale", float) = 1
		_NoiseFrequency ("Noise Frequency", float) = 1
		_NoiseOffset ("Noise Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 4.6

		//#include "UnityCG.cginc"
		#include "noiseSimplex.cginc"

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		float _RadiusFromOrigin, _NoiseScale, _NoiseFrequency;
		float4 _NoiseOffset;

		void vert(inout appdata v)
		{
			float4 wPos = mul(unity_ObjectToWorld, v.vertex);
			//float noise = _NoiseScale * snoise(float3(v.vertex.x + _NoiseOffset.x, v.vertex.y + _NoiseOffset.y, v.vertex.z + _NoiseOffset.z) * _NoiseFrequency);
			//float noise = _NoiseScale * snoise(float3(wPos.x + _NoiseOffset.x, wPos.y + _NoiseOffset.y, wPos.z + _NoiseOffset.z) * _NoiseFrequency);
			float nlength = 1.0 / sqrt((wPos.x * wPos.x) + (wPos.y * wPos.y) + (wPos.z * wPos.z));
			float3 normalized = float3(wPos.x * nlength, wPos.y * nlength, wPos.z * nlength);
			wPos.x = normalized.x * _RadiusFromOrigin;
			wPos.y = normalized.y * _RadiusFromOrigin;
			wPos.z = normalized.z * _RadiusFromOrigin;
			//v.vertex.xyz = normalized * _RadiusFromOrigin;
			//v.vertex.y += noise;
			v.vertex = mul(unity_WorldToObject, wPos);
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
		}	
		ENDCG
    }
	Fallback "Diffuse"
}