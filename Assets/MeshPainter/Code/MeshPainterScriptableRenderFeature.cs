using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MeshPainter
{
	public class MeshPainterScriptableRenderFeature : ScriptableRendererFeature
	{
		[SerializeField, Tooltip("When the Render Pass will be injected into the rendering process.")] 
		private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

		[SerializeField] 
		private MeshPainterRenderPassData _renderPassData;
		
		// ----------------------------------------------------------------------------

		private MeshPainterScriptableRenderPass _renderPass;
		private PaintEntityRenderData _paintEntitiesRenderData;
		
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Initialize the Scriptable Render Pass attached to this feature
		/// </summary>
		public override void Create()
		{
			_renderPassData.Asserts();
			_renderPass = new MeshPainterScriptableRenderPass(_renderPassData.UnwrapUVMaterial, 
				_renderPassData.PaintUnwrappedUVMaterial);
		}

		/// <summary>
		/// Inject the Mesh Painter Scriptable Render Pass into the renderer
		/// </summary>
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (!Application.isPlaying)
			{
				return;
			}
            
			renderer.EnqueuePass(_renderPass);
		}

		public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
		{
			Camera currentCamera = Camera.main;
			if (null == currentCamera)
			{
				return;
			}
			_renderPass.Setup(_renderPassEvent, _paintEntitiesRenderData, renderer.cameraColorTargetHandle, currentCamera);
		}

		// ----------------------------------------------------------------------------

		public void SetContext(in PaintEntityRenderData paintEntityRenderData)
		{
			_paintEntitiesRenderData = paintEntityRenderData;
		}
		
	}
}