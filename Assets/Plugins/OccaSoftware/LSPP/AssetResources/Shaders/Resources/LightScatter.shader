Shader "OccaSoftware/LSPP/LightScatter"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off ZTest Always
        Pass
        {
            Name "LightScatterPass"

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "LightScattering.hlsl"
            
            
            TEXTURE2D_X(_Occluders_LSPP);
            float _Density;
            bool _DoSoften;
            bool _DoAnimate;
            float _MaxRayDistance;
            int lspp_NumSamples;
            float3 _Tint;
            bool _LightOnScreenRequired;
            
            struct Attributes
            {
                float4 positionHCS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = float4(input.positionHCS.xyz, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                output.positionCS.y *= -1;
                #endif
                
                output.uv = input.uv;
                
                return output;
            }
            
            half3 Fragment (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return EstimateLightScattering(_Occluders_LSPP, input.uv, _Density, _DoSoften, _DoAnimate, _MaxRayDistance, lspp_NumSamples, _Tint, _LightOnScreenRequired);
            }
            ENDHLSL
        }
    }
}