using System;
using UnityEngine;

namespace MeshPainter
{
    [Serializable]
    public struct MeshPainterRenderPassData
    {
        [field: SerializeField] public Material UnwrapUVMaterial { get; private set; }
        [field: SerializeField] public Material PaintUnwrappedUVMaterial { get; private set; }
        
        public void Asserts()
        {
            Debug.Assert(UnwrapUVMaterial != null,
                "No MeshPainter UnwrapUVMaterial Material  has been assigned in MeshPainterRenderPassData");
            Debug.Assert(PaintUnwrappedUVMaterial != null,
                "No PaintUnwrappedUVMaterial MeshPainter Material has been assigned in MeshPainterRenderPassData");
        }
    }
}   