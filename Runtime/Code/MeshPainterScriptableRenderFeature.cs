using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MeshPainter
{
	public class MeshPainterScriptableRenderFeature : ScriptableRendererFeature
	{
		[SerializeField, Tooltip("When the Render Pass will be injected into the rendering process.")]
		private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

		[SerializeField] private Material paintIntoTextureMaterial;
		
		private MeshPainterScriptableRenderPass renderPass;
			
		private PaintEntityRenderData paintEntityRenderData;
		private BrushRenderData brushRenderData;

		/// <summary>
		///		Initialize the Scriptable Render Pass attached to this feature
		/// </summary>
		public override void Create()
		{
			renderPass?.Dispose();
			renderPass = new MeshPainterScriptableRenderPass(paintIntoTextureMaterial);
		}

		/// <summary>
		///		Inject the Mesh Painter Scriptable Render Pass into the renderer
		/// </summary>
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			//	Don't want this running in play mode...
			if (!Application.isPlaying || renderPass == null) return;
			
			renderPass.Setup(renderPassEvent, 
				paintEntityRenderData, 
				brushRenderData);
			renderer.EnqueuePass(renderPass);
		}
		
		/// <summary>
		///		Clean up resources
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (!disposing) return;
			renderPass?.Dispose();
			renderPass = null;
		}
		
		public void SetPaintEntityRenderData(PaintEntityRenderData context)
		{
			paintEntityRenderData = context;
		}
		
		public void SetBrushRenderData(BrushRenderData context)
		{
			brushRenderData = context;
		}
	}
}
