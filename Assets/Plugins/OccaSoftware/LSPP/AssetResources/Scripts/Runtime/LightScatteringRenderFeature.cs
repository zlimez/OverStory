
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.LSPP.Runtime
{
	public class LightScatteringRenderFeature : ScriptableRendererFeature
	{
		class LightScatteringRenderPass : ScriptableRenderPass
		{
			private Material occluderMaterial = null;
			private Material mergeMaterial = null;
			private Material blitMaterial = null;
			private Material lightScatterMaterial = null;

			private RenderTargetHandle occluderRT;
			private RenderTargetHandle lightScatteringRT;
			private RenderTargetHandle mergeRT;

			private const string occluderRtId = "_Occluders_LSPP";
			private const string lightScatterRtId = "_Scattering_LSPP";
			private const string mergeRtId = "_Merge_LSPP";

			private const string bufferPoolId = "LightScatteringPP";

			private LightScatteringPostProcess lspp;

			public LightScatteringRenderPass()
			{
				occluderRT.Init(occluderRtId);
				lightScatteringRT.Init(lightScatterRtId);
				mergeRT.Init(mergeRtId);
			}

			internal void SetupMaterials()
			{
				Shader lsppShader = Shader.Find("OccaSoftware/LSPP/LightScatter");
				Shader occluderShader = Shader.Find("OccaSoftware/LSPP/Occluders");
				Shader mergeShader = Shader.Find("OccaSoftware/LSPP/Merge");
				Shader blitShader = Shader.Find("OccaSoftware/LSPP/Blit");

				if (lsppShader != null && lightScatterMaterial == null)
					lightScatterMaterial = CoreUtils.CreateEngineMaterial(lsppShader);

				if (occluderShader != null && occluderMaterial == null)
					occluderMaterial = CoreUtils.CreateEngineMaterial(occluderShader);

				if (mergeShader != null && mergeMaterial == null)
					mergeMaterial = CoreUtils.CreateEngineMaterial(mergeShader);

				if (blitShader != null && blitMaterial == null)
					blitMaterial = CoreUtils.CreateEngineMaterial(blitShader);
			}

			internal bool HasAllMaterials()
			{
				if (lightScatterMaterial == null)
					return false;

				if (occluderMaterial == null)
					return false;

				if (mergeMaterial == null)
					return false;

				if (blitMaterial == null)
					return false;

				return true;
			}

			internal bool RegisterStackComponent()
			{
				lspp = VolumeManager.instance.stack.GetComponent<LightScatteringPostProcess>();

				if (lspp == null)
					return false;

				return lspp.IsActive();
			}


			public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
				descriptor.msaaSamples = 1;
				cmd.GetTemporaryRT(mergeRT.id, descriptor);

				descriptor.width /= 2;
				descriptor.height /= 2;
				cmd.GetTemporaryRT(lightScatteringRT.id, descriptor);
				cmd.GetTemporaryRT(occluderRT.id, descriptor);

				cmd.SetRenderTarget(occluderRT.Identifier());
				cmd.ClearRenderTarget(true, true, Color.black);

				cmd.SetRenderTarget(lightScatteringRT.Identifier());
				cmd.ClearRenderTarget(true, true, Color.black);

				cmd.SetRenderTarget(mergeRT.Identifier());
				cmd.ClearRenderTarget(true, true, Color.black);
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				UnityEngine.Profiling.Profiler.BeginSample("LSPP");

				// Early exit
				if (!HasAllMaterials())
					return;

				// Setup commandbuffer
				CommandBuffer cmd = CommandBufferPool.Get(bufferPoolId);

				// Disable Additional Decal Drawing.
				CoreUtils.SetKeyword(cmd, "_DBUFFER_MRT1", false);
				CoreUtils.SetKeyword(cmd, "_DBUFFER_MRT2", false);
				CoreUtils.SetKeyword(cmd, "_DBUFFER_MRT3", false);

				// Draw to occluder texture
				RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

				cmd.SetRenderTarget(occluderRT.Identifier());
				cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, occluderMaterial);

				// Set up scattering data texture
				UpdateLSPPMaterial();
				cmd.SetRenderTarget(lightScatteringRT.Identifier());
				cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, lightScatterMaterial);

				// Blit to screen
				cmd.SetRenderTarget(mergeRT.Identifier());
				cmd.SetGlobalTexture("_Source", source);
				cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mergeMaterial);

				cmd.SetRenderTarget(source);
				cmd.SetGlobalTexture("_Source", mergeRT.Identifier());
				cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, blitMaterial);

				// Clean up command buffer
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
				CommandBufferPool.Release(cmd);
				UnityEngine.Profiling.Profiler.EndSample();


				void UpdateLSPPMaterial()
				{
					lightScatterMaterial.SetFloat(Params.Density.Id, lspp.fogDensity.value);
					lightScatterMaterial.SetInt(Params.DoSoften.Id, BoolToInt(lspp.softenScreenEdges.value));
					lightScatterMaterial.SetInt(Params.DoAnimate.Id, BoolToInt(lspp.animateSamplingOffset.value));
					lightScatterMaterial.SetFloat(Params.MaxRayDistance.Id, lspp.maxRayDistance.value);
					lightScatterMaterial.SetInt(Params.SampleCount.Id, lspp.numberOfSamples.value);
					lightScatterMaterial.SetColor(Params.Tint.Id, lspp.tint.value);
					lightScatterMaterial.SetInt(Params.LightOnScreenRequired.Id, BoolToInt(lspp.lightMustBeOnScreen.value));
					lightScatterMaterial.SetInt(Params.FalloffDirective.Id, (int)lspp.falloffBasis.value);


					static int BoolToInt(bool a)
					{
						return a == false ? 0 : 1;
					}
				}
			}

			public override void OnCameraCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(occluderRT.id);
				cmd.ReleaseTemporaryRT(lightScatteringRT.id);
				cmd.ReleaseTemporaryRT(mergeRT.id);
			}
		}

		LightScatteringRenderPass lightScatteringPass;


		private void OnEnable()
		{
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += Recreate;
		}

		private void OnDisable()
		{
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= Recreate;
		}

		private void Recreate(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
		{
			Create();
		}

		private void LogWarningIfDepthTextureDisabled()
		{
			UniversalRenderPipelineAsset rpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
			if (rpAsset != null && !rpAsset.supportsCameraDepthTexture)
			{
				rpAsset.supportsCameraDepthTexture = true;
				Debug.LogWarning("LSPP: The currently active Universal Render Pipeline Asset did not have the Depth Texture option enabled. LSPP automatically enabled the Depth Texture setting on your Universal Render Pipeline Asset to ensure that LSPP can identify occluders.");
			}
		}

		public override void Create()
		{
			lightScatteringPass = new LightScatteringRenderPass();
			lightScatteringPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

			LogWarningIfDepthTextureDisabled();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (renderingData.cameraData.camera.cameraType == CameraType.Reflection)
				return;

			if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
				return;

			if (!lightScatteringPass.RegisterStackComponent())
				return;

			lightScatteringPass.SetupMaterials();
			if (!lightScatteringPass.HasAllMaterials())
				return;

			renderer.EnqueuePass(lightScatteringPass);
		}


		private static class Params
		{
			public readonly struct Param
			{
				public Param(string property)
				{
					Property = property;
					Id = Shader.PropertyToID(property);
				}

				readonly public string Property;
				readonly public int Id;
			}

			public static Param Density = new Param("_Density");
			public static Param DoSoften = new Param("_DoSoften");
			public static Param DoAnimate = new Param("_DoAnimate");
			public static Param MaxRayDistance = new Param("_MaxRayDistance");
			public static Param SampleCount = new Param("lspp_NumSamples");
			public static Param Tint = new Param("_Tint");
			public static Param LightOnScreenRequired = new Param("_LightOnScreenRequired");
			public static Param FalloffDirective = new Param("_FalloffDirective");

		}
	}
}