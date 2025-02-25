using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

/// <summary>
/// Represents a handle to a texture created by the rendering backend. For convenience, this 
/// is wrapped by the <see cref="ArgonTexture"/> class.
/// </summary>
public interface ITextureHandle : IDisposable
{
    /// <summary>
    /// The name of the texture.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The width of the texture in pixels.
    /// </summary>
    public uint Width { get; }
    /// <summary>
    /// The height of the texture in pixels.
    /// </summary>
    public uint Height { get; }
    /// <summary>
    /// The type of compression used by the texture.
    /// </summary>
    public TextureCompression Compression { get; }
    /// <summary>
    /// The pixel format of the texture.
    /// </summary>
    public TextureFormat Format { get; }
    /// <summary>
    /// Whether the texture has been loaded and is ready for drawing.
    /// Drawing with an unloaded texture will result in a fallback texture being displayed.
    /// </summary>
    public bool IsLoaded { get; }
}

public enum TextureCompression
{
    Unknown,
    Raw,
    DDS,
    PNG,
    JPEG,
    TGA,
    BMP
}

public enum TextureFormat
{
    Unknown,
    Unsupported,
    RGBA8,
    RGB8,
    RGB565,
    R8,
    R16,
    R32,
    RGBA32,

    RGBA = RGBA8,
    RGB = RGB8,
    Gray = R8
}