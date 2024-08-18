Shader "ABKaspo/A.U.R.W./WaterFree"
{
    Properties
    {
        [Header(Surface Settings)]
        _DeepWater("Deep Water", Color) = (0, 0.7550064, 0.9339623, 1)
        _ShallowWater("Shallow Water", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Float) = 0.85
        _Water_Near("Water Near", Color) = (0.05490196, 0.7254902, 0.8980393, 0.4117647)
        _Depth("Depth", Float) = 1
        _DepthStrength("Depth Strength", Range(0, 2)) = 1
        [ToggleUI]_FoamRender("Foam Render", Float) = 0
        [ToggleUI]_Foam_Texture("Foam Texture", Float) = 1
        [ToggleUI]_Normal_Mapping("Normal Mapping", Float) = 1
        [ToggleUI]_Second_Normal_Render("Second Normal Render", Float) = 0
        [ToggleUI]_Alpha("Alpha", Float) = 1
        _Power_Fresnel("Power Fresnel", Range(0, 5)) = 2
        [Header(Foam)]
        [NoScaleOffset]_Foam("Foam", 2D) = "white" {}
        _Foam_Tiling("Foam Tiling", Float) = 1
        _FoamXAnimation("Foam X Animation", Range(-1, 1)) = 1
        _FoamYAnimation("Foam Y Animation", Range(-1, 1)) = 1
        _FoamSpeed("Foam Speed", Float) = 1
        [Header(Normal Mapping)]
        [Normal][NoScaleOffset]_First_Normal("First Normal", 2D) = "bump" {}
        [Normal][NoScaleOffset]_Second_Normal("Second Normal", 2D) = "bump" {}
        [Normal][NoScaleOffset]_Foam_Normal("Foam Normal", 2D) = "bump" {}
        _Normal_Speed("Normal Speed", Float) = 0
        _Normal_Tiling("Normal Tiling", Float) = 1
        _NormalXAnimation("Normal X Animation", Float) = 0
        _NormalYAnimation("Normal Y Animation", Float) = 0
        _NormalMultipler("Normal Multipler", Float) = -1
        _Normal_Strength("Normal Strength", Float) = 0.85
        [HideInInspector][NonModifiableTextureData][Normal][NoScaleOffset]_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1("Texture2D", 2D) = "bump" {}
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float3 interp4 : INTERP4;
             float2 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float3 interp7 : INTERP7;
             float4 interp8 : INTERP8;
             float4 interp9 : INTERP9;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp6.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp7.xyz =  input.sh;
            #endif
            output.interp8.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp9.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp5.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp6.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp7.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp8.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp9.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0 = _Normal_Mapping;
            float _Property_965b70e8103b4a40b1094c0267b5006b_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0 = UnityBuildTexture2DStructNoScale(_Foam_Normal);
            float _Property_f74df507f38f407387cec81c219a56e6_Out_0 = _Foam_Tiling;
            float _Property_4c7fbe0d9af941e992af12111d4177d1_Out_0 = _FoamXAnimation;
            float _Property_53b9317383164956814502864166e2e4_Out_0 = _FoamYAnimation;
            float2 _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0 = float2(_Property_4c7fbe0d9af941e992af12111d4177d1_Out_0, _Property_53b9317383164956814502864166e2e4_Out_0);
            float2 _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0, _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2);
            float _Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0 = 20;
            float2 _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2;
            Unity_Divide_float2(_Multiply_59c1141543af476f8507b8138a7e1ced_Out_2, (_Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0.xx), _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2);
            float _Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0 = _FoamSpeed;
            float2 _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2;
            Unity_Multiply_float2_float2(_Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2);
            float2 _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2, _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3);
            float4 _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3));
            _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0);
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_R_4 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.r;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_G_5 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.g;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_B_6 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.b;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_A_7 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.a;
            float _Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0 = _FoamXAnimation;
            float _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0 = -1;
            float _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2;
            Unity_Multiply_float_float(_Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2);
            float _Property_2455e8a986384cf5b726d02f5aaf016d_Out_0 = _FoamYAnimation;
            float _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2;
            Unity_Multiply_float_float(_Property_2455e8a986384cf5b726d02f5aaf016d_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0 = float2(_Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0, _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2);
            float _Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0 = 20;
            float2 _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2;
            Unity_Divide_float2(_Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2, (_Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0.xx), _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2);
            float2 _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2;
            Unity_Multiply_float2_float2(_Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2);
            float2 _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2, _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3);
            float4 _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3));
            _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0);
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_R_4 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.r;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_G_5 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.g;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_B_6 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.b;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_A_7 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.a;
            float4 _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2;
            Unity_Add_float4(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0, _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0, _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2);
            float _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0 = _Normal_Strength;
            float3 _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2;
            Unity_NormalStrength_float((_Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2.xyz), _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2);
            float _Property_bce6a178b5d64327a7072cd2262697f3_Out_0 = _Second_Normal_Render;
            UnityTexture2D _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0 = UnityBuildTexture2DStructNoScale(_First_Normal);
            float _Property_7d508aa84b584868b40a4456ee985db8_Out_0 = _Normal_Tiling;
            float _Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0 = _NormalXAnimation;
            float _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0 = _NormalYAnimation;
            float2 _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0 = float2(_Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0, _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0);
            float2 _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0, _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2);
            float _Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0 = 20;
            float2 _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2;
            Unity_Divide_float2(_Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2, (_Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0.xx), _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2);
            float _Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0 = _Normal_Speed;
            float2 _Multiply_2adde2a7485a4698894fccc16759235a_Out_2;
            Unity_Multiply_float2_float2(_Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2);
            float2 _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2, _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3);
            float4 _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.tex, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.samplerstate, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.GetTransformedUV(_TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3));
            _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0);
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_R_4 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.r;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_G_5 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.g;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_B_6 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.b;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_A_7 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.a;
            UnityTexture2D _Property_47cdcbc376a741a9aececd2abda32a97_Out_0 = UnityBuildTexture2DStructNoScale(_Second_Normal);
            float _Property_7a8bb450a9964458925181d6259b0948_Out_0 = _NormalXAnimation;
            float _Property_fc29f03031b144508de1be295490f048_Out_0 = _NormalMultipler;
            float _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2;
            Unity_Multiply_float_float(_Property_7a8bb450a9964458925181d6259b0948_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2);
            float _Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0 = _NormalYAnimation;
            float _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2;
            Unity_Multiply_float_float(_Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0 = float2(_Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0, _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2);
            float _Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0 = 20;
            float2 _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2;
            Unity_Divide_float2(_Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2, (_Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0.xx), _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2);
            float2 _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2;
            Unity_Multiply_float2_float2(_Divide_767062c210e04a4d98246a2ef83dbe98_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2);
            float2 _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2, _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3);
            float4 _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0 = SAMPLE_TEXTURE2D(_Property_47cdcbc376a741a9aececd2abda32a97_Out_0.tex, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.samplerstate, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.GetTransformedUV(_TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3));
            _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0);
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_R_4 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.r;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_G_5 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.g;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_B_6 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.b;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_A_7 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.a;
            float4 _Add_4739c9727837418ebab455099e10a667_Out_2;
            Unity_Add_float4(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Add_4739c9727837418ebab455099e10a667_Out_2);
            float4 _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3;
            Unity_Branch_float4(_Property_bce6a178b5d64327a7072cd2262697f3_Out_0, _Add_4739c9727837418ebab455099e10a667_Out_2, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3);
            float _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0 = _Normal_Strength;
            float3 _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2;
            Unity_NormalStrength_float((_Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3.xyz), _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2);
            float3 _Branch_7464d7c4204e4690b0747d923a96f411_Out_3;
            Unity_Branch_float3(_Property_965b70e8103b4a40b1094c0267b5006b_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, _Branch_7464d7c4204e4690b0747d923a96f411_Out_3);
            float _Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0 = _Depth;
            float _Property_8326b2dff743433686d3bcc7419f93ad_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_968ac92f08fe48af98f29be510b75acb;
            _Depth_968ac92f08fe48af98f29be510b75acb.ScreenPosition = IN.ScreenPosition;
            float _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0, _Property_8326b2dff743433686d3bcc7419f93ad_Out_0, _Depth_968ac92f08fe48af98f29be510b75acb, _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1);
            float3 _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3;
            Unity_Lerp_float3(_Branch_7464d7c4204e4690b0747d923a96f411_Out_3, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, (_Depth_968ac92f08fe48af98f29be510b75acb_Depth_1.xxx), _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3);
            float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).GetTransformedUV(IN.uv0.xy));
            _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0);
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_R_4 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.r;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_G_5 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.g;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_B_6 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.b;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_A_7 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.a;
            float3 _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            Unity_Branch_float3(_Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0, _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3, (_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.xyz), _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3);
            float _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0 = _Smoothness;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.NormalTS = _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = 0;
            surface.Smoothness = _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float3 interp4 : INTERP4;
             float2 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float3 interp7 : INTERP7;
             float4 interp8 : INTERP8;
             float4 interp9 : INTERP9;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp6.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp7.xyz =  input.sh;
            #endif
            output.interp8.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp9.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp5.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp6.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp7.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp8.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp9.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0 = _Normal_Mapping;
            float _Property_965b70e8103b4a40b1094c0267b5006b_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0 = UnityBuildTexture2DStructNoScale(_Foam_Normal);
            float _Property_f74df507f38f407387cec81c219a56e6_Out_0 = _Foam_Tiling;
            float _Property_4c7fbe0d9af941e992af12111d4177d1_Out_0 = _FoamXAnimation;
            float _Property_53b9317383164956814502864166e2e4_Out_0 = _FoamYAnimation;
            float2 _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0 = float2(_Property_4c7fbe0d9af941e992af12111d4177d1_Out_0, _Property_53b9317383164956814502864166e2e4_Out_0);
            float2 _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0, _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2);
            float _Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0 = 20;
            float2 _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2;
            Unity_Divide_float2(_Multiply_59c1141543af476f8507b8138a7e1ced_Out_2, (_Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0.xx), _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2);
            float _Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0 = _FoamSpeed;
            float2 _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2;
            Unity_Multiply_float2_float2(_Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2);
            float2 _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2, _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3);
            float4 _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3));
            _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0);
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_R_4 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.r;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_G_5 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.g;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_B_6 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.b;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_A_7 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.a;
            float _Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0 = _FoamXAnimation;
            float _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0 = -1;
            float _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2;
            Unity_Multiply_float_float(_Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2);
            float _Property_2455e8a986384cf5b726d02f5aaf016d_Out_0 = _FoamYAnimation;
            float _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2;
            Unity_Multiply_float_float(_Property_2455e8a986384cf5b726d02f5aaf016d_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0 = float2(_Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0, _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2);
            float _Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0 = 20;
            float2 _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2;
            Unity_Divide_float2(_Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2, (_Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0.xx), _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2);
            float2 _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2;
            Unity_Multiply_float2_float2(_Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2);
            float2 _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2, _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3);
            float4 _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3));
            _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0);
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_R_4 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.r;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_G_5 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.g;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_B_6 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.b;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_A_7 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.a;
            float4 _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2;
            Unity_Add_float4(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0, _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0, _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2);
            float _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0 = _Normal_Strength;
            float3 _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2;
            Unity_NormalStrength_float((_Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2.xyz), _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2);
            float _Property_bce6a178b5d64327a7072cd2262697f3_Out_0 = _Second_Normal_Render;
            UnityTexture2D _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0 = UnityBuildTexture2DStructNoScale(_First_Normal);
            float _Property_7d508aa84b584868b40a4456ee985db8_Out_0 = _Normal_Tiling;
            float _Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0 = _NormalXAnimation;
            float _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0 = _NormalYAnimation;
            float2 _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0 = float2(_Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0, _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0);
            float2 _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0, _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2);
            float _Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0 = 20;
            float2 _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2;
            Unity_Divide_float2(_Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2, (_Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0.xx), _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2);
            float _Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0 = _Normal_Speed;
            float2 _Multiply_2adde2a7485a4698894fccc16759235a_Out_2;
            Unity_Multiply_float2_float2(_Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2);
            float2 _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2, _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3);
            float4 _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.tex, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.samplerstate, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.GetTransformedUV(_TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3));
            _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0);
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_R_4 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.r;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_G_5 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.g;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_B_6 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.b;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_A_7 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.a;
            UnityTexture2D _Property_47cdcbc376a741a9aececd2abda32a97_Out_0 = UnityBuildTexture2DStructNoScale(_Second_Normal);
            float _Property_7a8bb450a9964458925181d6259b0948_Out_0 = _NormalXAnimation;
            float _Property_fc29f03031b144508de1be295490f048_Out_0 = _NormalMultipler;
            float _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2;
            Unity_Multiply_float_float(_Property_7a8bb450a9964458925181d6259b0948_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2);
            float _Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0 = _NormalYAnimation;
            float _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2;
            Unity_Multiply_float_float(_Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0 = float2(_Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0, _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2);
            float _Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0 = 20;
            float2 _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2;
            Unity_Divide_float2(_Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2, (_Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0.xx), _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2);
            float2 _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2;
            Unity_Multiply_float2_float2(_Divide_767062c210e04a4d98246a2ef83dbe98_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2);
            float2 _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2, _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3);
            float4 _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0 = SAMPLE_TEXTURE2D(_Property_47cdcbc376a741a9aececd2abda32a97_Out_0.tex, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.samplerstate, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.GetTransformedUV(_TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3));
            _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0);
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_R_4 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.r;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_G_5 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.g;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_B_6 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.b;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_A_7 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.a;
            float4 _Add_4739c9727837418ebab455099e10a667_Out_2;
            Unity_Add_float4(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Add_4739c9727837418ebab455099e10a667_Out_2);
            float4 _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3;
            Unity_Branch_float4(_Property_bce6a178b5d64327a7072cd2262697f3_Out_0, _Add_4739c9727837418ebab455099e10a667_Out_2, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3);
            float _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0 = _Normal_Strength;
            float3 _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2;
            Unity_NormalStrength_float((_Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3.xyz), _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2);
            float3 _Branch_7464d7c4204e4690b0747d923a96f411_Out_3;
            Unity_Branch_float3(_Property_965b70e8103b4a40b1094c0267b5006b_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, _Branch_7464d7c4204e4690b0747d923a96f411_Out_3);
            float _Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0 = _Depth;
            float _Property_8326b2dff743433686d3bcc7419f93ad_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_968ac92f08fe48af98f29be510b75acb;
            _Depth_968ac92f08fe48af98f29be510b75acb.ScreenPosition = IN.ScreenPosition;
            float _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0, _Property_8326b2dff743433686d3bcc7419f93ad_Out_0, _Depth_968ac92f08fe48af98f29be510b75acb, _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1);
            float3 _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3;
            Unity_Lerp_float3(_Branch_7464d7c4204e4690b0747d923a96f411_Out_3, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, (_Depth_968ac92f08fe48af98f29be510b75acb_Depth_1.xxx), _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3);
            float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).GetTransformedUV(IN.uv0.xy));
            _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0);
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_R_4 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.r;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_G_5 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.g;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_B_6 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.b;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_A_7 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.a;
            float3 _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            Unity_Branch_float3(_Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0, _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3, (_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.xyz), _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3);
            float _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0 = _Smoothness;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.NormalTS = _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = 0;
            surface.Smoothness = _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALS
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float3 interp4 : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0 = _Normal_Mapping;
            float _Property_965b70e8103b4a40b1094c0267b5006b_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0 = UnityBuildTexture2DStructNoScale(_Foam_Normal);
            float _Property_f74df507f38f407387cec81c219a56e6_Out_0 = _Foam_Tiling;
            float _Property_4c7fbe0d9af941e992af12111d4177d1_Out_0 = _FoamXAnimation;
            float _Property_53b9317383164956814502864166e2e4_Out_0 = _FoamYAnimation;
            float2 _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0 = float2(_Property_4c7fbe0d9af941e992af12111d4177d1_Out_0, _Property_53b9317383164956814502864166e2e4_Out_0);
            float2 _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0, _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2);
            float _Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0 = 20;
            float2 _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2;
            Unity_Divide_float2(_Multiply_59c1141543af476f8507b8138a7e1ced_Out_2, (_Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0.xx), _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2);
            float _Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0 = _FoamSpeed;
            float2 _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2;
            Unity_Multiply_float2_float2(_Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2);
            float2 _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2, _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3);
            float4 _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3));
            _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0);
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_R_4 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.r;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_G_5 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.g;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_B_6 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.b;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_A_7 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.a;
            float _Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0 = _FoamXAnimation;
            float _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0 = -1;
            float _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2;
            Unity_Multiply_float_float(_Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2);
            float _Property_2455e8a986384cf5b726d02f5aaf016d_Out_0 = _FoamYAnimation;
            float _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2;
            Unity_Multiply_float_float(_Property_2455e8a986384cf5b726d02f5aaf016d_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0 = float2(_Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0, _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2);
            float _Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0 = 20;
            float2 _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2;
            Unity_Divide_float2(_Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2, (_Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0.xx), _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2);
            float2 _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2;
            Unity_Multiply_float2_float2(_Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2);
            float2 _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2, _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3);
            float4 _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3));
            _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0);
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_R_4 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.r;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_G_5 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.g;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_B_6 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.b;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_A_7 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.a;
            float4 _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2;
            Unity_Add_float4(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0, _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0, _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2);
            float _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0 = _Normal_Strength;
            float3 _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2;
            Unity_NormalStrength_float((_Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2.xyz), _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2);
            float _Property_bce6a178b5d64327a7072cd2262697f3_Out_0 = _Second_Normal_Render;
            UnityTexture2D _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0 = UnityBuildTexture2DStructNoScale(_First_Normal);
            float _Property_7d508aa84b584868b40a4456ee985db8_Out_0 = _Normal_Tiling;
            float _Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0 = _NormalXAnimation;
            float _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0 = _NormalYAnimation;
            float2 _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0 = float2(_Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0, _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0);
            float2 _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0, _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2);
            float _Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0 = 20;
            float2 _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2;
            Unity_Divide_float2(_Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2, (_Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0.xx), _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2);
            float _Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0 = _Normal_Speed;
            float2 _Multiply_2adde2a7485a4698894fccc16759235a_Out_2;
            Unity_Multiply_float2_float2(_Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2);
            float2 _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2, _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3);
            float4 _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.tex, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.samplerstate, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.GetTransformedUV(_TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3));
            _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0);
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_R_4 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.r;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_G_5 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.g;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_B_6 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.b;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_A_7 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.a;
            UnityTexture2D _Property_47cdcbc376a741a9aececd2abda32a97_Out_0 = UnityBuildTexture2DStructNoScale(_Second_Normal);
            float _Property_7a8bb450a9964458925181d6259b0948_Out_0 = _NormalXAnimation;
            float _Property_fc29f03031b144508de1be295490f048_Out_0 = _NormalMultipler;
            float _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2;
            Unity_Multiply_float_float(_Property_7a8bb450a9964458925181d6259b0948_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2);
            float _Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0 = _NormalYAnimation;
            float _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2;
            Unity_Multiply_float_float(_Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0 = float2(_Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0, _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2);
            float _Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0 = 20;
            float2 _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2;
            Unity_Divide_float2(_Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2, (_Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0.xx), _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2);
            float2 _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2;
            Unity_Multiply_float2_float2(_Divide_767062c210e04a4d98246a2ef83dbe98_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2);
            float2 _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2, _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3);
            float4 _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0 = SAMPLE_TEXTURE2D(_Property_47cdcbc376a741a9aececd2abda32a97_Out_0.tex, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.samplerstate, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.GetTransformedUV(_TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3));
            _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0);
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_R_4 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.r;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_G_5 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.g;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_B_6 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.b;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_A_7 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.a;
            float4 _Add_4739c9727837418ebab455099e10a667_Out_2;
            Unity_Add_float4(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Add_4739c9727837418ebab455099e10a667_Out_2);
            float4 _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3;
            Unity_Branch_float4(_Property_bce6a178b5d64327a7072cd2262697f3_Out_0, _Add_4739c9727837418ebab455099e10a667_Out_2, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3);
            float _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0 = _Normal_Strength;
            float3 _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2;
            Unity_NormalStrength_float((_Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3.xyz), _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2);
            float3 _Branch_7464d7c4204e4690b0747d923a96f411_Out_3;
            Unity_Branch_float3(_Property_965b70e8103b4a40b1094c0267b5006b_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, _Branch_7464d7c4204e4690b0747d923a96f411_Out_3);
            float _Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0 = _Depth;
            float _Property_8326b2dff743433686d3bcc7419f93ad_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_968ac92f08fe48af98f29be510b75acb;
            _Depth_968ac92f08fe48af98f29be510b75acb.ScreenPosition = IN.ScreenPosition;
            float _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0, _Property_8326b2dff743433686d3bcc7419f93ad_Out_0, _Depth_968ac92f08fe48af98f29be510b75acb, _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1);
            float3 _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3;
            Unity_Lerp_float3(_Branch_7464d7c4204e4690b0747d923a96f411_Out_3, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, (_Depth_968ac92f08fe48af98f29be510b75acb_Depth_1.xxx), _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3);
            float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).GetTransformedUV(IN.uv0.xy));
            _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0);
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_R_4 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.r;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_G_5 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.g;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_B_6 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.b;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_A_7 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.a;
            float3 _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            Unity_Branch_float3(_Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0, _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3, (_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.xyz), _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.NormalTS = _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma shader_feature _ EDITOR_VISUALIZATION
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        #define _FOG_FRAGMENT 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyzw =  input.texCoord1;
            output.interp4.xyzw =  input.texCoord2;
            output.interp5.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.texCoord1 = input.interp3.xyzw;
            output.texCoord2 = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float3 interp4 : INTERP4;
             float2 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float3 interp7 : INTERP7;
             float4 interp8 : INTERP8;
             float4 interp9 : INTERP9;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp6.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp7.xyz =  input.sh;
            #endif
            output.interp8.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp9.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp5.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp6.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp7.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp8.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp9.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0 = _Normal_Mapping;
            float _Property_965b70e8103b4a40b1094c0267b5006b_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0 = UnityBuildTexture2DStructNoScale(_Foam_Normal);
            float _Property_f74df507f38f407387cec81c219a56e6_Out_0 = _Foam_Tiling;
            float _Property_4c7fbe0d9af941e992af12111d4177d1_Out_0 = _FoamXAnimation;
            float _Property_53b9317383164956814502864166e2e4_Out_0 = _FoamYAnimation;
            float2 _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0 = float2(_Property_4c7fbe0d9af941e992af12111d4177d1_Out_0, _Property_53b9317383164956814502864166e2e4_Out_0);
            float2 _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0, _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2);
            float _Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0 = 20;
            float2 _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2;
            Unity_Divide_float2(_Multiply_59c1141543af476f8507b8138a7e1ced_Out_2, (_Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0.xx), _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2);
            float _Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0 = _FoamSpeed;
            float2 _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2;
            Unity_Multiply_float2_float2(_Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2);
            float2 _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2, _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3);
            float4 _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3));
            _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0);
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_R_4 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.r;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_G_5 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.g;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_B_6 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.b;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_A_7 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.a;
            float _Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0 = _FoamXAnimation;
            float _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0 = -1;
            float _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2;
            Unity_Multiply_float_float(_Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2);
            float _Property_2455e8a986384cf5b726d02f5aaf016d_Out_0 = _FoamYAnimation;
            float _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2;
            Unity_Multiply_float_float(_Property_2455e8a986384cf5b726d02f5aaf016d_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0 = float2(_Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0, _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2);
            float _Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0 = 20;
            float2 _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2;
            Unity_Divide_float2(_Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2, (_Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0.xx), _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2);
            float2 _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2;
            Unity_Multiply_float2_float2(_Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2);
            float2 _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2, _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3);
            float4 _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3));
            _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0);
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_R_4 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.r;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_G_5 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.g;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_B_6 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.b;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_A_7 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.a;
            float4 _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2;
            Unity_Add_float4(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0, _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0, _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2);
            float _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0 = _Normal_Strength;
            float3 _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2;
            Unity_NormalStrength_float((_Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2.xyz), _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2);
            float _Property_bce6a178b5d64327a7072cd2262697f3_Out_0 = _Second_Normal_Render;
            UnityTexture2D _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0 = UnityBuildTexture2DStructNoScale(_First_Normal);
            float _Property_7d508aa84b584868b40a4456ee985db8_Out_0 = _Normal_Tiling;
            float _Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0 = _NormalXAnimation;
            float _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0 = _NormalYAnimation;
            float2 _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0 = float2(_Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0, _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0);
            float2 _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0, _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2);
            float _Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0 = 20;
            float2 _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2;
            Unity_Divide_float2(_Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2, (_Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0.xx), _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2);
            float _Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0 = _Normal_Speed;
            float2 _Multiply_2adde2a7485a4698894fccc16759235a_Out_2;
            Unity_Multiply_float2_float2(_Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2);
            float2 _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2, _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3);
            float4 _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.tex, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.samplerstate, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.GetTransformedUV(_TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3));
            _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0);
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_R_4 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.r;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_G_5 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.g;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_B_6 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.b;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_A_7 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.a;
            UnityTexture2D _Property_47cdcbc376a741a9aececd2abda32a97_Out_0 = UnityBuildTexture2DStructNoScale(_Second_Normal);
            float _Property_7a8bb450a9964458925181d6259b0948_Out_0 = _NormalXAnimation;
            float _Property_fc29f03031b144508de1be295490f048_Out_0 = _NormalMultipler;
            float _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2;
            Unity_Multiply_float_float(_Property_7a8bb450a9964458925181d6259b0948_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2);
            float _Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0 = _NormalYAnimation;
            float _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2;
            Unity_Multiply_float_float(_Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0 = float2(_Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0, _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2);
            float _Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0 = 20;
            float2 _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2;
            Unity_Divide_float2(_Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2, (_Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0.xx), _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2);
            float2 _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2;
            Unity_Multiply_float2_float2(_Divide_767062c210e04a4d98246a2ef83dbe98_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2);
            float2 _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2, _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3);
            float4 _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0 = SAMPLE_TEXTURE2D(_Property_47cdcbc376a741a9aececd2abda32a97_Out_0.tex, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.samplerstate, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.GetTransformedUV(_TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3));
            _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0);
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_R_4 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.r;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_G_5 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.g;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_B_6 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.b;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_A_7 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.a;
            float4 _Add_4739c9727837418ebab455099e10a667_Out_2;
            Unity_Add_float4(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Add_4739c9727837418ebab455099e10a667_Out_2);
            float4 _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3;
            Unity_Branch_float4(_Property_bce6a178b5d64327a7072cd2262697f3_Out_0, _Add_4739c9727837418ebab455099e10a667_Out_2, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3);
            float _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0 = _Normal_Strength;
            float3 _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2;
            Unity_NormalStrength_float((_Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3.xyz), _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2);
            float3 _Branch_7464d7c4204e4690b0747d923a96f411_Out_3;
            Unity_Branch_float3(_Property_965b70e8103b4a40b1094c0267b5006b_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, _Branch_7464d7c4204e4690b0747d923a96f411_Out_3);
            float _Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0 = _Depth;
            float _Property_8326b2dff743433686d3bcc7419f93ad_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_968ac92f08fe48af98f29be510b75acb;
            _Depth_968ac92f08fe48af98f29be510b75acb.ScreenPosition = IN.ScreenPosition;
            float _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0, _Property_8326b2dff743433686d3bcc7419f93ad_Out_0, _Depth_968ac92f08fe48af98f29be510b75acb, _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1);
            float3 _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3;
            Unity_Lerp_float3(_Branch_7464d7c4204e4690b0747d923a96f411_Out_3, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, (_Depth_968ac92f08fe48af98f29be510b75acb_Depth_1.xxx), _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3);
            float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).GetTransformedUV(IN.uv0.xy));
            _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0);
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_R_4 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.r;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_G_5 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.g;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_B_6 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.b;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_A_7 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.a;
            float3 _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            Unity_Branch_float3(_Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0, _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3, (_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.xyz), _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3);
            float _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0 = _Smoothness;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.NormalTS = _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = 0;
            surface.Smoothness = _Property_fbe1b46b580448e9acd98b51b2fd94b7_Out_0;
            surface.Occlusion = 1;
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALS
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float3 interp4 : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0 = _Normal_Mapping;
            float _Property_965b70e8103b4a40b1094c0267b5006b_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0 = UnityBuildTexture2DStructNoScale(_Foam_Normal);
            float _Property_f74df507f38f407387cec81c219a56e6_Out_0 = _Foam_Tiling;
            float _Property_4c7fbe0d9af941e992af12111d4177d1_Out_0 = _FoamXAnimation;
            float _Property_53b9317383164956814502864166e2e4_Out_0 = _FoamYAnimation;
            float2 _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0 = float2(_Property_4c7fbe0d9af941e992af12111d4177d1_Out_0, _Property_53b9317383164956814502864166e2e4_Out_0);
            float2 _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_3b29bdfa9b2441b2a7f7ebc5132e0834_Out_0, _Multiply_59c1141543af476f8507b8138a7e1ced_Out_2);
            float _Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0 = 20;
            float2 _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2;
            Unity_Divide_float2(_Multiply_59c1141543af476f8507b8138a7e1ced_Out_2, (_Float_2e4023e4da724ef7bfdd6ebda0a741cc_Out_0.xx), _Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2);
            float _Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0 = _FoamSpeed;
            float2 _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2;
            Unity_Multiply_float2_float2(_Divide_d2d60003d79b46d3a464eb2d4bfdfc49_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2);
            float2 _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_c429140efd5c4bd08928d9c27a3bd5f7_Out_2, _TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3);
            float4 _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_eb65658c9ac444fc87ec3ac6eeaba68d_Out_3));
            _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0);
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_R_4 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.r;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_G_5 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.g;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_B_6 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.b;
            float _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_A_7 = _SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0.a;
            float _Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0 = _FoamXAnimation;
            float _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0 = -1;
            float _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2;
            Unity_Multiply_float_float(_Property_fc01600f59ae4488a1cd7e5dcc5abfa0_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2);
            float _Property_2455e8a986384cf5b726d02f5aaf016d_Out_0 = _FoamYAnimation;
            float _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2;
            Unity_Multiply_float_float(_Property_2455e8a986384cf5b726d02f5aaf016d_Out_0, _Float_8a925c4e2ea04e52b41e7dab2e918539_Out_0, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0 = float2(_Multiply_97219c4d7c0d444b8b804c530f18496e_Out_2, _Multiply_63ec17ca4a11484a969decd79cfdac9a_Out_2);
            float2 _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_778c3fa2da1e4115b01fa620cd13fe3b_Out_0, _Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2);
            float _Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0 = 20;
            float2 _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2;
            Unity_Divide_float2(_Multiply_900c0cdc8e684535a3bf54631a0ebabd_Out_2, (_Float_b7aaee791f104ff6b33f3212e6e5f1a7_Out_0.xx), _Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2);
            float2 _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2;
            Unity_Multiply_float2_float2(_Divide_540382e8b2fb402aabb37637abbd9f7b_Out_2, (_Property_32bcf01f6f9c4f19853cd6fe7a785c41_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2);
            float2 _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_f74df507f38f407387cec81c219a56e6_Out_0.xx), _Multiply_befd0adb8cbb448c83324975d91a721e_Out_2, _TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3);
            float4 _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.tex, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.samplerstate, _Property_c82aa3eb358f4bf99864d9c8a459d08a_Out_0.GetTransformedUV(_TilingAndOffset_d237df720cb14db39cb1b748f03a0f51_Out_3));
            _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0);
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_R_4 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.r;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_G_5 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.g;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_B_6 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.b;
            float _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_A_7 = _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0.a;
            float4 _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2;
            Unity_Add_float4(_SampleTexture2D_efd4c15688dd4601a1def668e730ff1b_RGBA_0, _SampleTexture2D_0d1c6b627fbe4994aa7dad4fc88a3958_RGBA_0, _Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2);
            float _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0 = _Normal_Strength;
            float3 _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2;
            Unity_NormalStrength_float((_Add_c4ef569dc364442ebe5835a3a4638e1c_Out_2.xyz), _Property_c573d6a73b8144a0b4f2942a5ec3ad86_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2);
            float _Property_bce6a178b5d64327a7072cd2262697f3_Out_0 = _Second_Normal_Render;
            UnityTexture2D _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0 = UnityBuildTexture2DStructNoScale(_First_Normal);
            float _Property_7d508aa84b584868b40a4456ee985db8_Out_0 = _Normal_Tiling;
            float _Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0 = _NormalXAnimation;
            float _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0 = _NormalYAnimation;
            float2 _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0 = float2(_Property_eace102e8e7c4a0a8785e56b966bcc65_Out_0, _Property_66c763f000fd46bdae697a73ebb3aa32_Out_0);
            float2 _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_e68d21e649844daba939d3625f5e7fd4_Out_0, _Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2);
            float _Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0 = 20;
            float2 _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2;
            Unity_Divide_float2(_Multiply_959600f69bfd40b28629a5d4015d2d88_Out_2, (_Float_2dae2a6ae6f146a0bdfc383bad5d6d78_Out_0.xx), _Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2);
            float _Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0 = _Normal_Speed;
            float2 _Multiply_2adde2a7485a4698894fccc16759235a_Out_2;
            Unity_Multiply_float2_float2(_Divide_f9b7f2b77d1948de9cf60bc805aa0e96_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2);
            float2 _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_2adde2a7485a4698894fccc16759235a_Out_2, _TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3);
            float4 _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.tex, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.samplerstate, _Property_8156975a6026498492edcf1ab2ffe3c7_Out_0.GetTransformedUV(_TilingAndOffset_05906389f5e74b01ab81252af06de42e_Out_3));
            _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0);
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_R_4 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.r;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_G_5 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.g;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_B_6 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.b;
            float _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_A_7 = _SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0.a;
            UnityTexture2D _Property_47cdcbc376a741a9aececd2abda32a97_Out_0 = UnityBuildTexture2DStructNoScale(_Second_Normal);
            float _Property_7a8bb450a9964458925181d6259b0948_Out_0 = _NormalXAnimation;
            float _Property_fc29f03031b144508de1be295490f048_Out_0 = _NormalMultipler;
            float _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2;
            Unity_Multiply_float_float(_Property_7a8bb450a9964458925181d6259b0948_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2);
            float _Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0 = _NormalYAnimation;
            float _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2;
            Unity_Multiply_float_float(_Property_3683ad9b0d75429c8a504a85fc2e8a62_Out_0, _Property_fc29f03031b144508de1be295490f048_Out_0, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0 = float2(_Multiply_1dac22c7654f46ebbc8fbe82c5fb9d91_Out_2, _Multiply_66b21625e50549948bc97b09ac8be2af_Out_2);
            float2 _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_b9e97623fbd04985b9e5c4fda1947a89_Out_0, _Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2);
            float _Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0 = 20;
            float2 _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2;
            Unity_Divide_float2(_Multiply_6cc4513e76bf4a6daa5fe3bb4490ac92_Out_2, (_Float_e3001b7e4b4445faa51ec11a352a59b8_Out_0.xx), _Divide_767062c210e04a4d98246a2ef83dbe98_Out_2);
            float2 _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2;
            Unity_Multiply_float2_float2(_Divide_767062c210e04a4d98246a2ef83dbe98_Out_2, (_Property_6fa29c73c41f41b5b9f3f07200f5714d_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2);
            float2 _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_7d508aa84b584868b40a4456ee985db8_Out_0.xx), _Multiply_b5973b3b150b4602b0ea8fd569a155ef_Out_2, _TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3);
            float4 _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0 = SAMPLE_TEXTURE2D(_Property_47cdcbc376a741a9aececd2abda32a97_Out_0.tex, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.samplerstate, _Property_47cdcbc376a741a9aececd2abda32a97_Out_0.GetTransformedUV(_TilingAndOffset_4424a04588484fc681b699e7e854a6e3_Out_3));
            _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0);
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_R_4 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.r;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_G_5 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.g;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_B_6 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.b;
            float _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_A_7 = _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0.a;
            float4 _Add_4739c9727837418ebab455099e10a667_Out_2;
            Unity_Add_float4(_SampleTexture2D_b186ebecffd14de59ff56052d83363eb_RGBA_0, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Add_4739c9727837418ebab455099e10a667_Out_2);
            float4 _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3;
            Unity_Branch_float4(_Property_bce6a178b5d64327a7072cd2262697f3_Out_0, _Add_4739c9727837418ebab455099e10a667_Out_2, _SampleTexture2D_994ba95b47a841618f30d4dcfb07d059_RGBA_0, _Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3);
            float _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0 = _Normal_Strength;
            float3 _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2;
            Unity_NormalStrength_float((_Branch_1a76319d5fee474ab21d9e1c705afd1e_Out_3.xyz), _Property_b3bc9b585e094cd697e3e85526e919fc_Out_0, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2);
            float3 _Branch_7464d7c4204e4690b0747d923a96f411_Out_3;
            Unity_Branch_float3(_Property_965b70e8103b4a40b1094c0267b5006b_Out_0, _NormalStrength_36c7d2e0d1f042d4839a9035792d5549_Out_2, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, _Branch_7464d7c4204e4690b0747d923a96f411_Out_3);
            float _Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0 = _Depth;
            float _Property_8326b2dff743433686d3bcc7419f93ad_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_968ac92f08fe48af98f29be510b75acb;
            _Depth_968ac92f08fe48af98f29be510b75acb.ScreenPosition = IN.ScreenPosition;
            float _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_fb370ad2370348dfa4f439293c2c8f2f_Out_0, _Property_8326b2dff743433686d3bcc7419f93ad_Out_0, _Depth_968ac92f08fe48af98f29be510b75acb, _Depth_968ac92f08fe48af98f29be510b75acb_Depth_1);
            float3 _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3;
            Unity_Lerp_float3(_Branch_7464d7c4204e4690b0747d923a96f411_Out_3, _NormalStrength_c7c2c6a5250741238ee11c106845169d_Out_2, (_Depth_968ac92f08fe48af98f29be510b75acb_Depth_1.xxx), _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3);
            float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1).GetTransformedUV(IN.uv0.xy));
            _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0);
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_R_4 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.r;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_G_5 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.g;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_B_6 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.b;
            float _SampleTexture2D_bf13c8829ac54582a83249243359cba2_A_7 = _SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.a;
            float3 _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            Unity_Branch_float3(_Property_f73df2d60f7b4f73adb9ff1131a59360_Out_0, _Lerp_1fe01e48fcb2488aa60f0c379f8986fc_Out_3, (_SampleTexture2D_bf13c8829ac54582a83249243359cba2_RGBA_0.xyz), _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.NormalTS = _Branch_59227cebcb224c5b9c44e2784b8629a1_Out_3;
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma shader_feature _ EDITOR_VISUALIZATION
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        #define _FOG_FRAGMENT 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyzw =  input.texCoord1;
            output.interp4.xyzw =  input.texCoord2;
            output.interp5.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.texCoord1 = input.interp3.xyzw;
            output.texCoord2 = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        #define REQUIRE_DEPTH_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceViewDirection;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1_TexelSize;
        float4 _DeepWater;
        float4 _ShallowWater;
        float _Depth;
        float _DepthStrength;
        float _Smoothness;
        float4 _Foam_TexelSize;
        float _Foam_Tiling;
        float _FoamXAnimation;
        float _FoamYAnimation;
        float _FoamSpeed;
        float4 _First_Normal_TexelSize;
        float4 _Second_Normal_TexelSize;
        float4 _Foam_Normal_TexelSize;
        float _Normal_Speed;
        float _Normal_Tiling;
        float _NormalXAnimation;
        float _NormalYAnimation;
        float _NormalMultipler;
        float _Normal_Strength;
        float _Foam_Texture;
        float _FoamRender;
        float _Second_Normal_Render;
        float _Normal_Mapping;
        float _Alpha;
        float4 _Water_Near;
        float _Power_Fresnel;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        SAMPLER(sampler_SampleTexture2D_bf13c8829ac54582a83249243359cba2_Texture_1);
        TEXTURE2D(_Foam);
        SAMPLER(sampler_Foam);
        TEXTURE2D(_First_Normal);
        SAMPLER(sampler_First_Normal);
        TEXTURE2D(_Second_Normal);
        SAMPLER(sampler_Second_Normal);
        TEXTURE2D(_Foam_Normal);
        SAMPLER(sampler_Foam_Normal);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneDepth_Linear01_float(float4 UV, out float Out)
        {
            Out = Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        struct Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float
        {
        float4 ScreenPosition;
        };
        
        void SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(float _Depth, float _Depth_Strength, Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float IN, out float Depth_1)
        {
        float _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1;
        Unity_SceneDepth_Linear01_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1);
        float _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2;
        Unity_Multiply_float_float(_SceneDepth_853e8af0fc834d2eb597c8db56346fa4_Out_1, _ProjectionParams.z, _Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2);
        float4 _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0 = IN.ScreenPosition;
        float _Split_6d91307fa3d3451ab93333eedb81d850_R_1 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[0];
        float _Split_6d91307fa3d3451ab93333eedb81d850_G_2 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[1];
        float _Split_6d91307fa3d3451ab93333eedb81d850_B_3 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[2];
        float _Split_6d91307fa3d3451ab93333eedb81d850_A_4 = _ScreenPosition_670f0f7ede1e456b9df76b47923cf17e_Out_0[3];
        float _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0 = _Depth;
        float _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2;
        Unity_Add_float(_Split_6d91307fa3d3451ab93333eedb81d850_A_4, _Property_6841f1d0c73b4acd8ad196d6b58224d9_Out_0, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2);
        float _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2;
        Unity_Subtract_float(_Multiply_ac0805f9642149e39ff6ce35d830c2f3_Out_2, _Add_9f9ae1dfc4774565b177d2ee6f56a37e_Out_2, _Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2);
        float _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0 = _Depth_Strength;
        float _Multiply_60873f20ffce4833af55d5876006704f_Out_2;
        Unity_Multiply_float_float(_Subtract_4884d969ee6a451788aa184fdcc687a9_Out_2, _Property_fc71989657b54026b2f08a6b30d83a4f_Out_0, _Multiply_60873f20ffce4833af55d5876006704f_Out_2);
        float _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        Unity_Clamp_float(_Multiply_60873f20ffce4833af55d5876006704f_Out_2, 0, 1, _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3);
        Depth_1 = _Clamp_302a6367ab3c438f8658dcdee01cdd19_Out_3;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_0557055a64304ee9b44602fd6da43941_Out_0 = _FoamRender;
            float4 _Property_45e7ff94d8a444dea81717742f7d6916_Out_0 = _ShallowWater;
            float _Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0 = _Foam_Texture;
            UnityTexture2D _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0 = UnityBuildTexture2DStructNoScale(_Foam);
            float _Property_57538a336e734bbf9e20269a3b28e342_Out_0 = _Foam_Tiling;
            float _Property_1e4e5bda1a774927b528d3bbab508132_Out_0 = _FoamXAnimation;
            float _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0 = _FoamYAnimation;
            float2 _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0 = float2(_Property_1e4e5bda1a774927b528d3bbab508132_Out_0, _Property_042b0ebff6634d108a9fa878cfe91e6e_Out_0);
            float2 _Multiply_c850f39a107744de851150dbc121232b_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_81fc3d33583d4d21991ab57c2049231e_Out_0, _Multiply_c850f39a107744de851150dbc121232b_Out_2);
            float _Float_dfb50eadd77b424089376b6f06210f8c_Out_0 = 20;
            float2 _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2;
            Unity_Divide_float2(_Multiply_c850f39a107744de851150dbc121232b_Out_2, (_Float_dfb50eadd77b424089376b6f06210f8c_Out_0.xx), _Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2);
            float _Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0 = _FoamSpeed;
            float2 _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2;
            Unity_Multiply_float2_float2(_Divide_35a034cead0a4ab2ae6b0222a47e4dbf_Out_2, (_Property_9993ad4fcbb54978b47f8c01e8c73547_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2);
            float2 _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_36f3de00edef4f07bad3caf0d55cc6c5_Out_2, _TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3);
            float4 _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_038b61925f84454db89e500812f19a7c_Out_3));
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_R_4 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.r;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_G_5 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.g;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_B_6 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.b;
            float _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_A_7 = _SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0.a;
            float _Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0 = _FoamXAnimation;
            float _Float_54d63f33268443aeb393122fd76d6470_Out_0 = -1;
            float _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2;
            Unity_Multiply_float_float(_Property_8b3e83417d9e4b06901a67b8f6decb11_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2);
            float _Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0 = _FoamYAnimation;
            float _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2;
            Unity_Multiply_float_float(_Property_7a2fcfa8eb024c499ad456bac07e790d_Out_0, _Float_54d63f33268443aeb393122fd76d6470_Out_0, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0 = float2(_Multiply_de1b6648f87940058fab2e3584d0cbac_Out_2, _Multiply_b56f1162edb542e0bce647eec2c6f6e2_Out_2);
            float2 _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_6eecbdf60ad646cfb3d830163df8ff6e_Out_0, _Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2);
            float _Float_8db8e9ceab8343fcb394a7235725f60b_Out_0 = 20;
            float2 _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2;
            Unity_Divide_float2(_Multiply_5b0b26ce8a5541558a540dcb2ea26520_Out_2, (_Float_8db8e9ceab8343fcb394a7235725f60b_Out_0.xx), _Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2);
            float _Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0 = _FoamSpeed;
            float2 _Multiply_e850d514781b413ba91b268d80287375_Out_2;
            Unity_Multiply_float2_float2(_Divide_a71ff9aebd7e40029a5387fb1a8cc06d_Out_2, (_Property_c17ef97ba8eb47ef8b2cf4e3051bb522_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2);
            float2 _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_57538a336e734bbf9e20269a3b28e342_Out_0.xx), _Multiply_e850d514781b413ba91b268d80287375_Out_2, _TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3);
            float4 _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.tex, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.samplerstate, _Property_ea675e6323fb48e69ec2c224d1bdbc49_Out_0.GetTransformedUV(_TilingAndOffset_b114af8b490346b6938c2a8d98ecb941_Out_3));
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_R_4 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.r;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_G_5 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.g;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_B_6 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.b;
            float _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_A_7 = _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0.a;
            float4 _Add_cf8fd1be6c2a402c932d72201a156444_Out_2;
            Unity_Add_float4(_SampleTexture2D_242565b46b104197a47ce33dc5bbbdd5_RGBA_0, _SampleTexture2D_92b275d28ad3482791dfe14f926d859f_RGBA_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2);
            float4 _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3;
            Unity_Branch_float4(_Property_a4e613beaa7b49dcb2cbf7a9d4265571_Out_0, _Add_cf8fd1be6c2a402c932d72201a156444_Out_2, _Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3);
            float4 _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2;
            Unity_Add_float4(_Property_45e7ff94d8a444dea81717742f7d6916_Out_0, _Branch_3978b2b74fbc487d8718af69010ae99e_Out_3, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2);
            float4 _Property_4ea671059b78439db0403f99c8fd787a_Out_0 = _DeepWater;
            float4 _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3;
            Unity_Branch_float4(_Property_0557055a64304ee9b44602fd6da43941_Out_0, _Add_6ecf56aae5fb43ca90a10b1399490edf_Out_2, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, _Branch_4dfd508e51684ee187679e4e5858f67f_Out_3);
            float4 _Property_2da57c390d2b4526bab393419a721bb8_Out_0 = _Water_Near;
            float _Property_dbefd7dd90694e418c0bedaa83042997_Out_0 = _Power_Fresnel;
            float _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_dbefd7dd90694e418c0bedaa83042997_Out_0, _FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3);
            float4 _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3;
            Unity_Lerp_float4(_Property_2da57c390d2b4526bab393419a721bb8_Out_0, _Property_4ea671059b78439db0403f99c8fd787a_Out_0, (_FresnelEffect_0a56adfbe50b416bbb79042bf61bef93_Out_3.xxxx), _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3);
            float _Property_2b2888111511433ab8e4416a30568eaf_Out_0 = _Depth;
            float _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0 = _DepthStrength;
            Bindings_Depth_a1d7411f5157ed14d9ded212ea87a317_float _Depth_0b61cdb5ec1a489ba4075277462fc2b2;
            _Depth_0b61cdb5ec1a489ba4075277462fc2b2.ScreenPosition = IN.ScreenPosition;
            float _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1;
            SG_Depth_a1d7411f5157ed14d9ded212ea87a317_float(_Property_2b2888111511433ab8e4416a30568eaf_Out_0, _Property_55193921eecc4aa88f9fbccdbfd36076_Out_0, _Depth_0b61cdb5ec1a489ba4075277462fc2b2, _Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1);
            float4 _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3;
            Unity_Lerp_float4(_Branch_4dfd508e51684ee187679e4e5858f67f_Out_3, _Lerp_2da6ed3b01124a49af9564f6fef12c9c_Out_3, (_Depth_0b61cdb5ec1a489ba4075277462fc2b2_Depth_1.xxxx), _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3);
            float _Property_b82851c87ff54acdb3c2b6205d813927_Out_0 = _Alpha;
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_R_1 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[0];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_G_2 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[1];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_B_3 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[2];
            float _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4 = _Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3[3];
            float _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            Unity_Branch_float(_Property_b82851c87ff54acdb3c2b6205d813927_Out_0, _Split_46557bcee58d44d6ae8b6dc945e67bcc_A_4, 1, _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3);
            surface.BaseColor = (_Lerp_8e752c9def5b4a0da918c985a6a2ad78_Out_3.xyz);
            surface.Alpha = _Branch_182cbdeaee4b417bae3c2803dabc97d0_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}