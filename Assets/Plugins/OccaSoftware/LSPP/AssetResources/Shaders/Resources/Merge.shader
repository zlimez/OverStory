Shader "OccaSoftware/LSPP/Merge"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off ZTest Always
        Pass
        {
            Name "MergePass"

            HLSLPROGRAM
            
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "CrossUpsampling.hlsl"
            
            TEXTURE2D_X(_Source);
            TEXTURE2D_X(_Scattering_LSPP);
            
            
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
                float3 screenColor = SAMPLE_TEXTURE2D_X_LOD(_Source, linear_clamp_sampler, input.uv, 0);
                float3 upscaleResults = CrossSample(_Scattering_LSPP, input.uv, _ScreenParams.xy * 0.5, 2.0);
                //return SAMPLE_TEXTURE2D_X_LOD(_Scattering_LSPP, linear_clamp_sampler, input.uv, 0);
                return screenColor + upscaleResults;
            }
            ENDHLSL
        }
    }
}