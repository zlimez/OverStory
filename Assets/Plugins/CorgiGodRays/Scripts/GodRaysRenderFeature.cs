namespace CorgiGodRays
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class GodRaysRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public enum VolumeStepQuality
        {
            Low = 16,
            Med = 32,
            High = 64,
        }

        [System.Serializable]
        public enum VolumeTextureQuality
        {
            Low = 4,
            Med = 2,
            High = 1,
        }

        [System.Serializable]
        public enum BilateralBlurSamples
        {
            Low = 2,
            Med = 4,
            High = 7,
        }

        [System.Serializable]
        public class GodRaysSettings
        {
            [Tooltip("Resources used by the plugin internally.")]
            public GodRaysRenderData renderData;

            [Tooltip("Random jitter used when tracing rays through the depth and shadowmap buffers. If this value is higher you can get away with lower step counts - but you may want to cover it up with the blur options.")]
            [Range(0f, 1f)] public float Jitter = 0.25f;

            [Tooltip("The size of the internal godrays texture. Higher looks better but requires more GPU performance.")]
            public VolumeTextureQuality textureQuality = VolumeTextureQuality.Med;

            [Tooltip("Higher values of this require more GPU performance, but look nicer. Scales with texture quality. You can get away with higher step counts if texture quality is lower.")]
            public VolumeStepQuality stepQuality = VolumeStepQuality.Med;

            [Tooltip("Blur the internal godrays texture? Useful for lower quality settings.")]
            public bool blur = true;

            [Tooltip("How many times should the blur pass be ran? When using lower quality settings, you may want to increase this value.")]
            [Range(1, 4)] public int BlurCount = 2;

            [Tooltip("During the blur, how many samples should be taken? Higher values mean higher quality, but also more GPU performance overhead.")]
            public BilateralBlurSamples blurSamples = BilateralBlurSamples.Med;

            [Tooltip("When using a lower texture quality, enabling this may make things look nicer. Only has an effect if not on the highest texture quality.")]
            public bool depthAwareUpsampling = true;

            [Tooltip("If you want the godrays to work with the main directional light, enable this. Note: there is a small GPU performance overhead for enabling this.")]
            public bool allowMainLight = true;

            [Tooltip("If you want the godrays to work with lights other than the main directional light, enable this. Note: there is a small GPU performance overhead for enabling this.")]
            public bool allowAdditionalLights = true;

            [Tooltip("You may need to toggle this on or off depending on your exact SRP configuration. Usually leave this on.")]
            public bool useUnityDepthDirectly = true;

            [Tooltip("If you are using Unity's ScreenSpaceShadows ReaderFeature, you will need to enable this. If not, usually leave this off.")]
            public bool supportUnityScreenSpaceShadows = false;

            [Tooltip("Renders this effect after the transparent queue. If false, it will render after the opaque queue.")]
            public bool renderAfterTransparents = true;

            [Tooltip("If enabled, a curve will be used to control volumetrics intensity based on distance from the camera. Enabling this will incur a small GPU performance overhead.")]
            public bool useVariableIntensity = false;

            [Tooltip("If useVariableIntensity is enabled, this curve will be used to control volumetric intensity based on distance. This curve is turned into a texture at runtime.")]
            public AnimationCurve variableIntensityCurve;

            [Tooltip("Max distance to trace out volumetrics. The smaller this value is, the higher quality volumetrics can appear, but they will also fade out much sooner.")]
            public float maxDistance = 256f;

            [Tooltip("If enabled, light color of the main light and the additional lights will be encoded into the godrays texture. This will require more GPU performance, but will result in more accurate colors.")]
            public bool encodeLightColor = true;

            [Tooltip("If enabled, use higher quality textures (float instead of half) - some platforms may not support one or the other.")]
            public bool enableHighQualityTextures = false;
        }

        public GodRaysSettings settings;

        [System.NonSerialized] private GodRaysRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new GodRaysRenderPass();
            _renderPass.Initialize();

#if UNITY_EDITOR
            if (settings.renderData == null)
            {
                settings.renderData = GodRaysRenderData.FindData();
            }
#endif
        }

        private void OnDisable()
        {
            if (_renderPass != null)
            {
                _renderPass.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(_renderPass != null)
            {
                _renderPass.Dispose(); 
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (settings.renderData == null)
            {
                settings.renderData = GodRaysRenderData.FindData();
            }
#endif

            _renderPass.Setup(settings, renderer, ref renderingData);
            renderer.EnqueuePass(_renderPass);
        }

        private void OnValidate()
        {
            if (settings == null) return;

            if(settings.maxDistance < 1f || settings.maxDistance > 100000f)
            {
                settings.maxDistance = Mathf.Clamp(settings.maxDistance, 1f, 100000f);
            }
        }
    }
}
