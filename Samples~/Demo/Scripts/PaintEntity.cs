using System.Collections.Generic;
using UniColorPicker;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace MeshPainter
{
	public class PaintEntity : MonoBehaviour
	{
		[SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private UniversalRendererData universalRenderData;
		
		[SerializeField] private Slider brushStrengthSlider;
		[SerializeField] private Slider brushOpacitySlider;
		[SerializeField] private Slider brushSizeSlider;
		[SerializeField] private Color color = Color.red;
		
		[SerializeField] private float rotationSpeed = 75.0f;
		
		private MeshPainterScriptableRenderFeature scriptableRenderFeature;
		private Camera mainCamera;
		private Vector4 lastMouse;
		
		
		private void Awake()
		{
			Debug.Assert(universalRenderData != null);
			Debug.Assert(brushOpacitySlider != null);
			Debug.Assert(brushSizeSlider != null);
			Debug.Assert(universalRenderData != null);

			brushOpacitySlider.value = 1;
			brushSizeSlider.value = .03f;
			brushStrengthSlider.value = 1;

			if (!TryGetMeshPainterRenderFeature(out scriptableRenderFeature))
			{
				Debug.LogError($"UniversalRenderData ({universalRenderData}) does not contain MeshPainterScriptableRenderFeature");
			}
			
			mainCamera = Camera.main;
		}

		private void OnEnable()
		{
			scriptableRenderFeature.SetPaintEntityRenderData(new PaintEntityRenderData
			{
				renderer = meshRenderer,
				transform = transform,
				meshFilter = meshFilter,
			});
		}

		private void Update()
		{
			//	Handle any input for rotating the entity around...
			RotateEntity();
			//	Determine if we're currently painting on this entity 
			ProcessMouseInput(out Vector4 mouse);
			//	Pass on new brush data to the render feature.
			scriptableRenderFeature.SetBrushRenderData(new BrushRenderData
			{
				color = color,
				position = mouse,
				brushSize = brushSizeSlider.value,
				brushOpacity = brushOpacitySlider.value,
				brushStrength = brushStrengthSlider.value
			});
		}

		private bool TryGetMeshPainterRenderFeature(out MeshPainterScriptableRenderFeature renderFeature)
		{
			List<ScriptableRendererFeature> rendererFeatures = universalRenderData.rendererFeatures;
			foreach (ScriptableRendererFeature rf in rendererFeatures)
			{
				if (rf.GetType() != typeof(MeshPainterScriptableRenderFeature))
				{
					continue;
				}

				renderFeature = (MeshPainterScriptableRenderFeature)rf;
				return true;
			}

			renderFeature = null;
			return false;
		}

		private void RotateEntity()
		{
			Vector3 axis = Vector3.zero;
			if(Input.GetKey(KeyCode.Q))
			{
				axis.y = 1;
			}
			else if (Input.GetKey(KeyCode.E))
			{
				axis.y = -1; 
			}
			
			if(Input.GetKey(KeyCode.W))
			{
				axis.x = -1;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				axis.x = 1; 
			}
			transform.Rotate(axis * (rotationSpeed * Time.deltaTime));
		}

		private void ProcessMouseInput(out Vector4 mouse)
		{
			mouse = Vector4.zero;
			//	Is the user attempting to paint?
			if (!Input.GetMouseButton(0)) return; 
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			//	Is the user casting the mouse over this instance?
			if (!Physics.Raycast(ray, out RaycastHit raycastHit)
			    || !raycastHit.transform.TryGetComponent(out PaintEntity entity)) return;
			if (entity != this) return;
		
			mouse = raycastHit.point;

			float tolerance = brushSizeSlider.value * 1.25f;
			//	Set w to 1 so the render pass knows this is a click!
			//mouse.w = (mouse - lastMouse).sqrMagnitude >= tolerance*tolerance ? 1 : 0; 

			if ((mouse - lastMouse).sqrMagnitude >= tolerance * tolerance)
			{
				mouse.w = 0;
			}
			else
			{
				mouse.w = 1;
			}
			lastMouse = mouse;
		}
	}
}