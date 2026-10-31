using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace MeshPainter
{
	public class MeshPainterScriptableRenderPass : ScriptableRenderPass, IDisposable
	{
		private const int TEX_SIZE = 2048;
		private const string PROFILER_IDENT = "Mesh Painter Render Pass";
		private const string PAINT_HANDLE_TEX_NAME = "_MeshPainterPaintedTexture";

		//	Shader Properties...
		private static readonly int mainTex = Shader.PropertyToID("_MainTex");
		private static readonly int tex = Shader.PropertyToID("_paintedTex");
		private static readonly int brushOpacity = Shader.PropertyToID("_brushOpacity");
		private static readonly int brushStrength = Shader.PropertyToID("_brushStrength");
		private static readonly int brushSize = Shader.PropertyToID("_brushSize");
		private static readonly int paintColour = Shader.PropertyToID("_paintColour");
		private static readonly int objectToWorld = Shader.PropertyToID("_objectToWorld");
		private static readonly int mousePosition = Shader.PropertyToID("_mousePosition");
		
		private BrushRenderData brushRenderData;
		private MeshPainterRenderPassData meshPainterRenderPassData;
		private PaintEntityRenderData paintEntityRenderData;
		private RTHandle paintedTexture;
		
		private class PaintIntoTextureDrawPassData
		{
			public Color brushColor;
			public Material Material;	
			public Matrix4x4 ObjectToWorld;
			public Mesh Mesh;
			public Vector4 MousePosition;
			public float brushOpacity;
			public float brushSize;
			public float brushStrength;
		}
		
		public MeshPainterScriptableRenderPass(MeshPainterRenderPassData meshPainterRenderPassData)
		{
			this.meshPainterRenderPassData = meshPainterRenderPassData;
			profilingSampler = new ProfilingSampler(PROFILER_IDENT);
		}

		public void Setup(RenderPassEvent renderEvent, PaintEntityRenderData entityRenderData, BrushRenderData brushRenderData)
		{
			renderPassEvent = renderEvent;
			paintEntityRenderData = entityRenderData;
			this.brushRenderData = brushRenderData;
		}
		
		public void Dispose()
		{
			paintedTexture?.Release();
			paintedTexture = null;
		}
		
		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			//	Create/recreate the texture which we're painting into
			if (!EnsurePaintedTexture()) return;
			TextureHandle paintedHandle = renderGraph.ImportTexture(paintedTexture);
			//	Paint directly into our painting texture
			PaintIntoTarget(renderGraph, paintedHandle);
		}
		
		private bool EnsurePaintedTexture()
		{
			Renderer renderer = paintEntityRenderData.renderer;
			Mesh mesh = paintEntityRenderData.meshFilter.sharedMesh;
			if (renderer == null || mesh == null) return false;
			
			int width = TEX_SIZE;
			int height = TEX_SIZE;
			Texture texture = renderer.sharedMaterial.GetTexture(mainTex);
			if (texture != null)
			{
				width = texture.width;
				height = texture.height;
			}
			
			//	Do we need to recreate the RT which we paint into? 
			if (paintedTexture != null && paintedTexture.rt != null &&
			    paintedTexture.rt.width == width && paintedTexture.rt.height == height)
			{
				//	Painted Texture is still valid, carry on painting!
				return true;
			}
			
			//	Need to recreate the texture...
			paintedTexture?.Release();
			paintedTexture = RTHandles.Alloc(width, height,
				colorFormat: GraphicsFormat.R8G8B8A8_SRGB,
				filterMode: FilterMode.Bilinear,
				wrapMode: TextureWrapMode.Clamp,
				name: PAINT_HANDLE_TEX_NAME);
			
			//	Let everyone know about the new Texture we're painting into
			Shader.SetGlobalTexture(tex, paintedTexture.rt);
			paintEntityRenderData.renderer.sharedMaterial.SetTexture(tex, paintedTexture.rt);
			return true;
		}

		private void PaintIntoTarget(RenderGraph renderGraph, TextureHandle paintedHandle)
		{
			using IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Paint into texture", out PaintIntoTextureDrawPassData passData);
			passData.Mesh = paintEntityRenderData.meshFilter.sharedMesh;
			passData.Material = meshPainterRenderPassData.PaintUnwrappedUVMaterial;
			passData.ObjectToWorld = paintEntityRenderData.transform.localToWorldMatrix;;
			passData.MousePosition = brushRenderData.position;
			passData.brushColor = brushRenderData.color;
			passData.brushSize = brushRenderData.brushSize;
			passData.brushOpacity = brushRenderData.brushOpacity;
			passData.brushStrength = brushRenderData.brushStrength;
			
			builder.SetRenderAttachment(paintedHandle, 0);
			//	AllowGlobalStateModification is set to true so SetGlobalXYZ will work on the RasterCommandBuffer
			builder.AllowGlobalStateModification(true); 
			builder.SetRenderFunc(static (PaintIntoTextureDrawPassData data, RasterGraphContext context) =>
			{
				RasterCommandBuffer cmd = context.cmd;

				cmd.SetGlobalFloat(brushOpacity, data.brushOpacity);
				cmd.SetGlobalFloat(brushStrength, data.brushStrength);
				cmd.SetGlobalFloat(brushSize, data.brushSize);
				cmd.SetGlobalColor(paintColour, data.brushColor);
				cmd.SetGlobalMatrix(objectToWorld, data.ObjectToWorld);
				cmd.SetGlobalVector(mousePosition, data.MousePosition);
			
				cmd.DrawMesh(data.Mesh, Matrix4x4.identity, data.Material);
			});
		}
	}
}
