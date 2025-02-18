using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

public interface ITextureHandle : IDisposable
{
    public string Name { get; }
    public uint Width { get; }
    public uint Height { get; }
    public TextureCompression Compression { get; }
    public TextureFormat Format { get; }
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