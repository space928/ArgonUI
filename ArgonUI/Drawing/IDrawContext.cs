using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

public interface IDrawContext : IDisposable
{
    // UIElements are represented by a vertex buffer and a material instance
    // When dirty, a UI element may need to update it's vertex buffer, but it might only need to update it's material.
    // We need methods such that a UIElement can create a new material and vertex buffer with a given number of elements
    // and update either the vertex buffer or the material later as needed, and release them when destroyed.
    // Ideally, we would batch vertex buffers together before drawing to save making many tiny draw calls.
    // Maybe we could take all the vertex buffers which haven't changed in the last 10 frames and attempt to batch them into one.
    // We could also consider using gpu instancing or geometry shaders. We usually are just drawing quads of various sizes and
    // positions with a couple of different shader variants.

    public void InitRenderer(UIWindow window);
    public void StartFrame(Bounds2D region);
    public void EndFrame();

    public void Clear(Vector4 colour);
    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding);
    public void DrawText(Bounds2D bounds, float size, string c, BMFont font, Vector4 colour);
    public void DrawChar(Bounds2D bounds, float size, char c, BMFont font, Vector4 colour);
    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding);
    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding);
    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur);

    public void FlushBatch();

    // Closes the stream when complete
    public ITextureHandle LoadTexture(Stream data, string? name = null, 
        TextureCompression compression = TextureCompression.Unknown);

    public ITextureHandle LoadTexture(ReadOnlyMemory<byte> data, string? name = null,
        TextureCompression compression = TextureCompression.Unknown);
}
