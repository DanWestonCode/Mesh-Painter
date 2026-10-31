## Mesh Painter Scriptable Render Feature
<a href="http://www.youtube.com/watch?feature=player_embedded&v=PoGv3ErdMvw
" target="_blank"><img src="http://img.youtube.com/vi/PoGv3ErdMvw/0.jpg" 
alt="Mesh Painter Demo  " width="480" height="360" border="10" /></a>

### Installation: 
- In Unity go to Windows -> Package Management -> Package Manager
- Press the + icon and select Install package from git URL
- Paste https://github.com/DanWestonCode/Mesh-Painter.git

### About:
Mesh Painter is a Scriptable Render Feature, implemented using the Render Graph system. The Scriptable Render Feature enables the user to paint onto a target mesh at runtime.  

The technique in this project works by creating a persistent render texture at runtime which matches a given target mesh’s texture dimensions. Each frame the user's mouse is polled and when clicked will fire a raycast to determine if the target mesh is being pointed at. The world space position of the click is passed into the ‘PaintIntoTexture.Shader’ to paint the brush colour at the given location.

`PaintIntoTexture.Shader` unwraps a target mesh into the render texture, using the target mesh’s UV coordinates. Every pixel of the render texture corresponds to a point on the target mesh. Each pixel compares the mesh’s surface point’s world position to the passed mouse position, painting the brush colour when the pixel is within range of the input. 

The render texture is only cleared upon creation/resizing so the output of the shader is blended into the existing content of the texture, accumulating the result. 

The BlendPainting.shader’ finally composites the base texture of the mesh with the runtime paint texture.

### How to use:
`PaintIntoTexture.Shader` unwraps a target mesh into the render texture, using the target mesh’s UV coordinates. Every pixel of the render texture effectively corresponds to a point on the target mesh. Each pixel compares the mesh’s surface point’s world position to the mouse position, painting the brush colour when the pixel is within range of the input. The Render Texture is only cleared upon creation/resizing so the result of the shader are blended into the existing content of the texture, accumulating the result of the painting.The BlendPainting.shader’ finally composites the base texture of the mesh with the runtime paint texture.

The project comes with a sample demo project with a working example, open the Demo scene and inspect the `PaintEntity` script.

- Add  `MeshPainterScriptableRenderFeature` to the Universal Renderer Data`.
- Set the`renderPassEvent` to After Render Opaques.
- Provide a material instance for `paintIntoTextureMaterial`, this material should use the `PaintIntoTexture.shader`.
- During the runtime of your game, pass a `PaintEntityRenderData` instance to the `MeshPainterScriptableRenderFeature` instance. This should contain the MeshRenderer Transform and MeshFilter of the target you wish to pain 
Pass a ‘BrushRenderData’ instance to control any of the brush settings 

### TODOs:
- Currently the Render Feature only supports painting of one context at a time. This should be easily solvable. Changing the Feature to contain a collection of `PaintEntityRenderData` instances and adapting the pass to create a Render Texture for each instance. This would naturally cause a linear increase in performance cost. 
- The sample demo can be improved with how it handles the painting. Opacity and Strength are not well utilised as the brush is applied every frame regardless of movement, accumulating the result on one button press with no mouse movement.
- An option to add padding to the UV islands to avoid visible seams when painting. 

### References
- [Dynamic Mesh Painting in Unreal Engine 4](https://www.kodeco.com/6817-dynamic-mesh-painting-in-unreal-engine-4)
- [Write a render pass using the render graph system in URP](https://docs.unity.com/ko-kr/engine/6000.0/manual/render-pipelines/universal-render-pipeline/customizing-urp/render-graph/write-render-pass)

### Licence

Copyright (c) 2026 Daniel Weston

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.