using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Backends.Headless;

public class HeadlessTexture2D : ITextureHandle
{
    private readonly string name;
    private uint width;
    private uint height;
    private TextureCompression compression;
    private TextureFormat format;

    public string Name => name;
    public uint Width => width;
    public uint Height => height;
    public TextureCompression Compression => compression;
    public TextureFormat Format => format;
    public bool IsLoaded => true;

    public HeadlessTexture2D(string? name, TextureData? textureData, TextureCompression compression = TextureCompression.Unknown)
    {
        this.name = name ?? "ArgonTexture";
        width = 16;
        height = 16;
        this.compression = compression;
        format = TextureFormat.Unknown;

        if (textureData is TextureData data)
        {
            width = data.width;
            height = data.height;
            format = data.format;
        }
    }

    public void Dispose()
    {

    }
}
