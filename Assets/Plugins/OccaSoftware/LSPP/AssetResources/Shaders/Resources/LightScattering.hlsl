#ifndef OS_LSPP_PASS_INCLUDED
#define OS_LSPP_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

half GetRandomHalf(half2 Seed)
{
	return saturate(frac(sin(dot(Seed, float2(12.9898, 78.233))) * 43758.5453));
}

SamplerState linear_clamp_sampler;
int _FalloffDirective;

#if defined(USING_STEREO_MATRICES)
	#define FLIP -1
#else
	#define FLIP 1
#endif


float3 EstimateLightScattering( TEXTURE2D_X(OccluderTex), half2 UV, half FogDensity, bool SoftenScreenEdges, bool AnimateNoise, half MaxRayDistanceScreen, int NumberOfSamples, half3 SunColorTint, bool LightMustBeOnScreen)
{
	
	float3 Color = float3(0, 0, 0);
	
	// Early Exit
	if (FogDensity <= 0.001)
		return Color;
	
	FogDensity *= 0.1;
	
	// Get light 
	Light mainLight = GetMainLight();
	half3 mainLightDirection = mainLight.direction;
	half3 mainLightColor = mainLight.color * SunColorTint;
	
	// Adjust simulated fog density by vector alignment
	half falloff = 1.0;
	
	UV = UnityStereoTransformScreenSpaceTex(UV);
	
	[branch]
	if (_FalloffDirective == 0)
	{
		half3 viewVector = mul(unity_CameraInvProjection, half4(UV * 2 - 1, 0.0, -1 * FLIP)).xyz;
		viewVector = mul(unity_CameraToWorld, half4(viewVector, 0.0)).xyz;
		half lightVectorAlignment = saturate(dot(mainLightDirection, normalize(viewVector)));
	
		lightVectorAlignment = lightVectorAlignment * lightVectorAlignment * lightVectorAlignment;
		falloff = lightVectorAlignment;
	}
	else
	{
		half3 forwardVector = unity_CameraToWorld._m02_m12_m22 * FLIP;
		half dotDir = saturate(dot(mainLightDirection, normalize(forwardVector)));
		dotDir = dotDir * dotDir * dotDir;
		falloff = dotDir;
	}
	
	
	FogDensity *= falloff;
	
	
	// Early Exit
	if(FogDensity <= 0.001)
		return Color;
	
	// Find light position in screen space
	half3 lightPosWS = mul(unity_WorldToCamera, half4(mainLightDirection, 0.0)).xyz;
	half4 lightPosCS = mul(unity_CameraProjection, half4(lightPosWS, 1.0));
	
	half2 lightPosUV = -lightPosCS.xy / lightPosCS.w;
	lightPosUV *= FLIP;
	lightPosUV = lightPosUV;
	if (LightMustBeOnScreen)
	{
		if (any(lightPosUV < -1.1) || any(lightPosUV > 1.1))
		{
			return Color;
		}
	}
	
	
	// Find light direction
	half2 UVRemap = (UV * 2.0) - 1.0;
	half2 directionToLight = lightPosUV - UVRemap;
	
	
	// Get Noise
	half seedOffset = AnimateNoise ? _Time.x : 0;
	half r = GetRandomHalf(UV + seedOffset);
	
	// Determine Step Length
	half invSampleCount = 1.0 / (half)NumberOfSamples;
	half stepLength = MaxRayDistanceScreen * invSampleCount;
	half2 offset = r * directionToLight * stepLength;
	
	
	// Setup for loop
	half alpha = 1.0;
	half invMaxDistance = 1.0 / MaxRayDistanceScreen;
	//half decayFactor = exp(-FogDensity * stepLength * invMaxDistance);
	half3 illuminationFactor = FogDensity * mainLightColor * stepLength * invMaxDistance;
	
	/*
	bool clipAtScreenEdge = false;
	if (clipAtScreenEdge)
	{
		if (any(lightPosUV < -1) || any(lightPosUV > 1))
		{
			return Color;
		}
	}
	*/
	for (int i = 0; i < NumberOfSamples; i++)
	{
		half2 samplePosition = UV + (directionToLight * stepLength * i) + offset;
		
		half decayFactor = exp(-FogDensity * stepLength * i * invMaxDistance);
		
		// Assume no occlusion if outside bounds
		half occlusion = 1.0;
		if (all(samplePosition > 0) || all(samplePosition < 1))
		{
			occlusion = SAMPLE_TEXTURE2D_X_LOD(OccluderTex, linear_clamp_sampler, samplePosition, 0).r;
		}
		
		// Blend into corners
		if (SoftenScreenEdges)
		{
			half2 adj = smoothstep(0.9, 1.0, abs((samplePosition * 2.0) - 1.0));
			occlusion = lerp(occlusion, 1.0, max(adj.x, adj.y));
		}
		
		half occlusionFalloff = i * invSampleCount;
		occlusionFalloff *= occlusionFalloff;
		//occlusion = lerp(occlusion, 1.0, occlusionFalloff); // Assume that this sample is less likely to occlude this pixel as the sample distance increases
		
		// Evaluate Color Contribution and Transmittance 
		Color += illuminationFactor * alpha * occlusion;
		alpha *= decayFactor;
	}
	
	return Color;
}
#endif