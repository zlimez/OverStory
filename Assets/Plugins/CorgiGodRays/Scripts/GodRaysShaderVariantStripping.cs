
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CorgiGodRays
{

    // only supporting shader stripping on newer versions of URP 
#if UNITY_URP_12_OR_HIGHER
    public class GodRaysShaderVariantStripping : IPreprocessShaders
    {
        public int callbackOrder { get { return 1; } }

        private static GodRaysRenderData renderData = null;

        // apply keywords 
        private static ShaderKeyword DEPTH_AWARE_UPSAMPLE = new ShaderKeyword("DEPTH_AWARE_UPSAMPLE");
        private static ShaderKeyword GODRAYS_ADDITIVE_LIGHTS = new ShaderKeyword("GODRAYS_ADDITIVE_LIGHTS");

        // blur keywords 
        private static ShaderKeyword SAMPLE_COUNT_LOW = new ShaderKeyword("SAMPLE_COUNT_LOW");
        private static ShaderKeyword SAMPLE_COUNT_MED = new ShaderKeyword("SAMPLE_COUNT_MED");
        private static ShaderKeyword SAMPLE_COUNT_HIGH = new ShaderKeyword("SAMPLE_COUNT_HIGH");

        // generate keywords 
        private static ShaderKeyword VOLUME_STEPS_LOW = new ShaderKeyword("VOLUME_STEPS_LOW");
        private static ShaderKeyword VOLUME_STEPS_MED = new ShaderKeyword("VOLUME_STEPS_MED");
        private static ShaderKeyword VOLUME_STEPS_HIGH = new ShaderKeyword("VOLUME_STEPS_HIGH");

        private static List<UniversalRendererData> _universalRenderingDatas = new List<UniversalRendererData>();
        private static bool HAS_DEPTH_AWARE_UPSAMPLE;
        private static bool HAS_GODRAYS_ADDITIVE_LIGHTS;
        private static bool HAS_SAMPLE_COUNT_LOW;
        private static bool HAS_SAMPLE_COUNT_MED  ;
        private static bool HAS_SAMPLE_COUNT_HIGH ;
        private static bool HAS_VOLUME_STEPS_LOW  ;
        private static bool HAS_VOLUME_STEPS_MED  ;
        private static bool HAS_VOLUME_STEPS_HIGH ;

        private void TryRefreshDatas()
        {
            if(_universalRenderingDatas.Count == 0)
            {
                var rendererDataGuids = AssetDatabase.FindAssets("t:UniversalRendererData");
                foreach (var guid in rendererDataGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);

                    _universalRenderingDatas.Add(asset);
                }
            }
        }

        private void TryRefreshFeatures()
        {
            // reset 
            HAS_DEPTH_AWARE_UPSAMPLE = false;
            HAS_GODRAYS_ADDITIVE_LIGHTS = false;
            HAS_SAMPLE_COUNT_LOW = false;
            HAS_SAMPLE_COUNT_MED = false;
            HAS_SAMPLE_COUNT_HIGH = false;
            HAS_VOLUME_STEPS_LOW = false;
            HAS_VOLUME_STEPS_MED = false;
            HAS_VOLUME_STEPS_HIGH = false;

            foreach(var data in _universalRenderingDatas)
            {
                foreach(var feature in data.rendererFeatures)
                {
                    if(feature is GodRaysRenderFeature)
                    {
                        var godRaysRenderFeature = (GodRaysRenderFeature) feature;

                        if (godRaysRenderFeature.settings.depthAwareUpsampling) HAS_DEPTH_AWARE_UPSAMPLE = true;
                        if (godRaysRenderFeature.settings.allowAdditionalLights) HAS_GODRAYS_ADDITIVE_LIGHTS = true;

                        if (godRaysRenderFeature.settings.blurSamples == GodRaysRenderFeature.BilateralBlurSamples.Low) HAS_SAMPLE_COUNT_LOW = true;
                        if (godRaysRenderFeature.settings.blurSamples == GodRaysRenderFeature.BilateralBlurSamples.Med) HAS_SAMPLE_COUNT_MED = true;
                        if (godRaysRenderFeature.settings.blurSamples == GodRaysRenderFeature.BilateralBlurSamples.High) HAS_SAMPLE_COUNT_HIGH = true;

                        if (godRaysRenderFeature.settings.stepQuality == GodRaysRenderFeature.VolumeStepQuality.Low) HAS_VOLUME_STEPS_LOW = true;
                        if (godRaysRenderFeature.settings.stepQuality == GodRaysRenderFeature.VolumeStepQuality.Med) HAS_VOLUME_STEPS_MED = true;
                        if (godRaysRenderFeature.settings.stepQuality == GodRaysRenderFeature.VolumeStepQuality.High) HAS_VOLUME_STEPS_HIGH = true;
                    }
                }
            }
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (renderData == null) renderData = GodRaysRenderData.FindData();
            if (!renderData.StripUnusedShadersFromBuilds) return;

            TryRefreshDatas();
            TryRefreshFeatures();

            for (var i = data.Count - 1; i >= 0; --i)
            {
                var shaderData = data[i];
                var reject = false;

                if (!HAS_DEPTH_AWARE_UPSAMPLE && shaderData.shaderKeywordSet.IsEnabled(DEPTH_AWARE_UPSAMPLE)) reject = true;
                if (!HAS_GODRAYS_ADDITIVE_LIGHTS && shaderData.shaderKeywordSet.IsEnabled(GODRAYS_ADDITIVE_LIGHTS)) reject = true;
                if (!HAS_SAMPLE_COUNT_LOW && shaderData.shaderKeywordSet.IsEnabled(SAMPLE_COUNT_LOW)) reject = true;
                if (!HAS_SAMPLE_COUNT_MED && shaderData.shaderKeywordSet.IsEnabled(SAMPLE_COUNT_MED)) reject = true;
                if (!HAS_SAMPLE_COUNT_HIGH && shaderData.shaderKeywordSet.IsEnabled(SAMPLE_COUNT_HIGH)) reject = true;
                if (!HAS_VOLUME_STEPS_LOW && shaderData.shaderKeywordSet.IsEnabled(VOLUME_STEPS_LOW)) reject = true;
                if (!HAS_VOLUME_STEPS_MED && shaderData.shaderKeywordSet.IsEnabled(VOLUME_STEPS_MED)) reject = true;
                if (!HAS_VOLUME_STEPS_HIGH && shaderData.shaderKeywordSet.IsEnabled(VOLUME_STEPS_HIGH)) reject = true;

                if (reject)
                {
                    data.RemoveAt(i);
                }
            }
        }
    }
#endif

}
#endif