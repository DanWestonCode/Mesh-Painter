using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MeshPainter
{
	public class MeshPainterScriptableRenderPass : ScriptableRenderPass
	{
		private const string PROFILER_IDENT = "Mesh Painter Render Pass";

		// ----------------------------------------------------------------------------

		private readonly int _unwrapedUVs = Shader.PropertyToID("_unwrapUVs");
		private readonly int _mainTex = Shader.PropertyToID("_MainTex");
		private readonly int  _tex = Shader.PropertyToID("_paintedTex");
		private readonly int _brushOpacity = Shader.PropertyToID("_brushOpacity");
		private readonly int _brushStrength = Shader.PropertyToID("_brushStrength");
		private readonly int _brushSize = Shader.PropertyToID("_brushSize");
		private readonly int _paintColour = Shader.PropertyToID("_paintColour");
		private readonly int _objectToWorld = Shader.PropertyToID("_objectToWorld");
		private readonly int _mousePosition = Shader.PropertyToID("_mousePosition");

		private RTHandle unwrappedUVs;
		
		// ----------------------------------------------------------------------------

		private Material _unwrapUVMaterial;
		private Material _paintUnwrappedUVSMaterial;
		private PaintEntityRenderData _paintEntityRenderData;
		private RenderTargetIdentifier _cameraColorTarget;
		private RenderTexture _unwrappedUVRenderTexture;
		private bool _hasGeneratedUnwrappedUVTexture = false;
		private RaycastHit[] _raycastHits = new RaycastHit[1];
		private Camera _camera;

		// ----------------------------------------------------------------------------
		
		public MeshPainterScriptableRenderPass(Material unwrapUVMaterial, Material paintUnwrappedUvsMaterial)
		{
			_unwrapUVMaterial = unwrapUVMaterial;
			_paintUnwrappedUVSMaterial = paintUnwrappedUvsMaterial;
			profilingSampler = new ProfilingSampler(PROFILER_IDENT);
		}
		
		public void Setup(RenderPassEvent renderEvent,
			PaintEntityRenderData paintEntityRenderData,
			RenderTargetIdentifier cameraColorTarget,
			Camera camera)
		{
			renderPassEvent = renderEvent;
			_paintEntityRenderData = paintEntityRenderData;
			_cameraColorTarget = cameraColorTarget;
			_camera = camera;
		}
		
		// ----------------------------------------------------------------------------
		
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			int width = 2048;
			int height = 2048;
			
			Texture texture = _paintEntityRenderData.Renderer.sharedMaterial.GetTexture(_mainTex);
			if (null != texture)
			{
				width = texture.width;
				height = texture.height;
			}
            
			cmd.GetTemporaryRT(_unwrapedUVs, 
				width, 
				height, 
				24,
				FilterMode.Bilinear, 
				RenderTextureFormat.ARGB32,
				RenderTextureReadWrite.Default, 
				Mathf.Max(1, QualitySettings.antiAliasing));

			if (null == _unwrappedUVRenderTexture)
			{
				_unwrappedUVRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32,
					RenderTextureReadWrite.Default);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, profilingSampler))
			{
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				SetGlobalShaderParameters(cmd);
				GetMouseInput(cmd);
				DrawMeshUVsUnwrapped(cmd);
				PaintUnwrappedUVsTexture(cmd);

				context.ExecuteCommandBuffer(cmd);
			}

			CommandBufferPool.Release(cmd);
		}

		// ----------------------------------------------------------------------------

		private void SetGlobalShaderParameters(CommandBuffer commandBuffer)
		{
			commandBuffer.SetGlobalTexture(_tex, _unwrappedUVRenderTexture);
			commandBuffer.SetGlobalFloat(_brushOpacity,1.0f);
			commandBuffer.SetGlobalFloat(_brushStrength,1.0f );
			commandBuffer.SetGlobalFloat(_brushSize,0.0315f );
			commandBuffer.SetGlobalColor(_paintColour, Color.blue);
			
			Matrix4x4 localToWorldMatrix = _paintEntityRenderData.Transform.localToWorldMatrix;
			commandBuffer.SetGlobalMatrix(_objectToWorld, localToWorldMatrix);
			
			_paintEntityRenderData.Renderer.sharedMaterial.SetTexture(_tex, _unwrappedUVRenderTexture);
		}

		private void GetMouseInput(CommandBuffer commandBuffer)
		{
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			Vector4 mouseHitPosition = ray.origin;//.positiveInfinity;

			if (Physics.RaycastNonAlloc(ray, _raycastHits) > 0)
			{
				RaycastHit raycastHit = _raycastHits[0];
				
				if (!raycastHit.transform.TryGetComponent(out PaintEntity paintEntity))
				{
					return;
				}
				
				mouseHitPosition = raycastHit.point;
				
				Debug.DrawLine(ray.origin, raycastHit.point, Color.green);
			}
			

			mouseHitPosition.w = Input.GetMouseButton(0)? 1 : 0;
			commandBuffer.SetGlobalVector(_mousePosition,  mouseHitPosition);
		}

		private void DrawMeshUVsUnwrapped(CommandBuffer commandBuffer)
		{
			if (_hasGeneratedUnwrappedUVTexture)
			{
				return;
			}
			_hasGeneratedUnwrappedUVTexture = true;
			
			CoreUtils.SetRenderTarget(commandBuffer, _unwrappedUVRenderTexture, ClearFlag.All, Color.clear);
			commandBuffer.ClearRenderTarget(true, true, Color.clear);
			
			Renderer renderer = _paintEntityRenderData.Renderer;
			if (null == renderer)
			{
				return;
			}
			
			commandBuffer.DrawMesh(_paintEntityRenderData.Mesh, Matrix4x4.identity, _unwrapUVMaterial);
			CoreUtils.SetRenderTarget(commandBuffer, _cameraColorTarget);
		}

		private void PaintUnwrappedUVsTexture(CommandBuffer commandBuffer)
		{
			CoreUtils.SetRenderTarget(commandBuffer, _unwrapedUVs, ClearFlag.All, Color.clear);
			commandBuffer.ClearRenderTarget(true, true, Color.clear);

			commandBuffer.DrawMesh(_paintEntityRenderData.Mesh, Matrix4x4.identity, _paintUnwrappedUVSMaterial);
			commandBuffer.Blit(_unwrapedUVs, _unwrappedUVRenderTexture);

			CoreUtils.SetRenderTarget(commandBuffer, _cameraColorTarget);
		}
        
		// ----------------------------------------------------------------------------
    }
}