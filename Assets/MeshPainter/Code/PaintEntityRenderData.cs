using System;
using UnityEngine;

namespace MeshPainter
{
	[Serializable]
	public struct PaintEntityRenderData
	{
		public Renderer renderer;
		public Transform transform;
		public MeshFilter meshFilter;
		
	}
}