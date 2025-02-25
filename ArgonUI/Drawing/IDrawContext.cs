using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

/// <summary>
/// An interface implemented by the rendering backend which provides methods for drawing 
/// 2D shapes/text/textures on a window.
/// </summary>
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

    /// <summary>
    /// Initialises the renderer; must be called before any other draw commands can be called.
    /// </summary>
    /// <param name="window">The window instance this draw context is associated with.</param>
    public void InitRenderer(UIWindow window);
    /// <summary>
    /// Starts rendering a frame in the given region.
    /// No draw calls should be invoked until this method is called.
    /// </summary>
    /// <param name="region"></param>
    public void StartFrame(Bounds2D region);
    /// <summary>
    /// Ends frame rendering, no more draw calls should be invoked until the next <see cref="StartFrame(Bounds2D)"/>.
    /// </summary>
    public void EndFrame();

    /// <summary>
    /// Clears the entire draw region with a solid colour.
    /// </summary>
    /// <param name="colour">The colour to fill the region passed into <see cref="StartFrame(Bounds2D)"/> with.</param>
    public void Clear(Vector4 colour);
    /// <summary>
    /// Draws a solid rounded rectangle.
    /// </summary>
    /// <param name="bounds">The absolute window-space bounds of the rectangle.</param>
    /// <param name="colour">The colour of the rectangle.</param>
    /// <param name="rounding">The corner rounding radius in pixels.</param>
    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding);
    /// <summary>
    /// Draws a string of text within the specified bounds.
    /// </summary>
    /// <param name="bounds">The bounds in which this string should be drawn.
    /// Overflowing text will be truncated.</param>
    /// <param name="size">The font size to render the text with.</param>
    /// <param name="s">The string to draw.</param>
    /// <param name="font">The font to draw the string with.</param>
    /// <param name="colour">The colour to draw the text in.</param>
    public void DrawText(Bounds2D bounds, float size, string s, BMFont font, Vector4 colour);
    /// <summary>
    /// Draws a single character within the specified bounds.
    /// </summary>
    /// <param name="bounds">The bounds in which this char should be drawn.</param>
    /// <param name="size">The font size to render the char with.</param>
    /// <param name="c">The char to draw.</param>
    /// <param name="font">The font to draw the char with.</param>
    /// <param name="colour">The colour to draw the char in.</param>
    public void DrawChar(Bounds2D bounds, float size, char c, BMFont font, Vector4 colour);
    /// <summary>
    /// Draws a rounded rectangle filled with a texture.
    /// </summary>
    /// <param name="bounds">The absolute window-space bounds of the rectangle.</param>
    /// <param name="texture">The texture to fill this rectangle with.</param>
    /// <param name="rounding">The corner rounding radius in pixels.</param>
    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding);
    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding);
    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur);

    /// <summary>
    /// Draw calls are expected to be automatically batched by the implementor to improve performance.
    /// After all draw calls have been completed for the frame, the renderer should call this to
    /// flush any remaining draw call batches.
    /// </summary>
    public void FlushBatch();

    // Closes the stream when complete
    public ITextureHandle LoadTexture(Stream data, string? name = null, 
        TextureCompression compression = TextureCompression.Unknown);

    public ITextureHandle LoadTexture(TextureData data, string? name = null,
        TextureCompression compression = TextureCompression.Unknown);
}
