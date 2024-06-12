using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshPainter
{
    [Serializable]
    public struct MeshPainterRenderPassData
    {
        [SerializeField] private Material _unwrapUVMaterial;
        [SerializeField] private Material _paintUnwrappedUVMaterial;

        // ----------------------------------------------------------------------------

        public Material UnwrapUVMaterial => _unwrapUVMaterial;
        public Material PaintUnwrappedUVMaterial => _paintUnwrappedUVMaterial;

        // ----------------------------------------------------------------------------
        
        public void Asserts()
        {
            Debug.Assert(null != _unwrapUVMaterial,
                "No MeshPainter Material has been assigned in MeshPainterRenderPassData");
            Debug.Assert(null != _paintUnwrappedUVMaterial,
                "No MeshPainter Material has been assigned in MeshPainterRenderPassData");
        }
    }
}