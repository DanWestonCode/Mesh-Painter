using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MeshPainter
{
	public class PaintEntity : MonoBehaviour
	{
		[SerializeField] private UniversalRendererData _universalRenderData;
		[SerializeField] private PaintEntityRenderData _paintEntityRenderData;
		
		// ----------------------------------------------------------------------------

		private MeshPainterScriptableRenderFeature _scriptableRenderFeature;
		
		// ----------------------------------------------------------------------------
		
		private void Awake()
		{
			Assert();

			if (!TryGetMeshPainterRenderFeature(out _scriptableRenderFeature))
			{
				Debug.LogError($"UniversalRenderData ({_universalRenderData}) does not contain MeshPainterScriptableRenderFeature");
			}
		}

		private void OnEnable()
		{
			_scriptableRenderFeature.SetContext(_paintEntityRenderData);
		}
		
		// ----------------------------------------------------------------------------

		private bool TryGetMeshPainterRenderFeature(out MeshPainterScriptableRenderFeature scriptableRenderFeature)
		{
			List<ScriptableRendererFeature> rendererFeatures = _universalRenderData.rendererFeatures;

			foreach (ScriptableRendererFeature renderFeature in rendererFeatures)
			{
				if (renderFeature.GetType() != typeof(MeshPainterScriptableRenderFeature))
				{
					continue;
				}

				scriptableRenderFeature = (MeshPainterScriptableRenderFeature)renderFeature;
				return true;
			}

			scriptableRenderFeature = null;
			return false;
		}
		
		private void Assert()
		{
			Debug.Assert(null != _universalRenderData);
			_paintEntityRenderData.Asserts();
		}
	}
}