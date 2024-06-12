using System;
using UnityEngine;

namespace MeshPainter
{
	[Serializable]
	public struct PaintEntityRenderData
	{
		[SerializeField] private Renderer _renderer;
		[SerializeField] private Transform _transform;
		[SerializeField] private MeshFilter _meshFilter;
		
		// ----------------------------------------------------------------------------

		public Renderer Renderer => _renderer;
		public Transform Transform => _transform;
		public Mesh Mesh => _meshFilter != null ? _meshFilter.sharedMesh : null;

		// ----------------------------------------------------------------------------

		public void Asserts()
		{
			Debug.Assert(null != _renderer);
			Debug.Assert(null != _transform);
			Debug.Assert(null != _meshFilter);
		}
	}
}