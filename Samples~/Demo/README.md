### Setup:
- Add a `MeshPainterScriptableRenderFeature` instance to your Universal Renderer.
- On the `MeshPainterScriptableRenderFeature` instance: 
    - Hook up `PaintIntoTexture.material` to its respective property. 
- Open the Demo scene in the sample folder. 
- In the scene, select JuliusCaesarBust, hookup your `UniversalRendererData` which has the `MeshPainterScriptableRenderFeature` instance, to the `PaintEntity` component. 
- If you are using the new Unity Input System Package, add `Unity.InputSystem` to `danwestoncode.meshpainter.sample.demo`. You will also need to update the Event System in the scene.
- Happy painting!