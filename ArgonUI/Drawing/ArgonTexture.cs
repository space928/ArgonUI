using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

public class ArgonTexture
{
    private readonly ConcurrentQueue<Action<IDrawContext>> drawCommands = [];

    public ITextureHandle? TextureHandle { get; private set; }
    public string? Name => TextureHandle?.Name;
    public uint Width => TextureHandle?.Width ?? 0;
    public uint Height => TextureHandle?.Height ?? 0;
    public TextureCompression Compression => TextureHandle?.Compression ?? TextureCompression.Unknown;
    public TextureFormat Format => TextureHandle?.Format ?? TextureFormat.Unknown;
    public bool IsLoaded => TextureHandle?.IsLoaded ?? false;

    private ArgonTexture() { }

    /// <summary>
    /// Creates a new texture from a file and loads it asynchronously.
    /// <para/>
    /// Supports: BMP, DDS, TGA, PNG, JPG
    /// </summary>
    /// <param name="path">The path to the texture file to load.</param>
    /// <param name="containingAssembly">Searches the specified assembly for the embedded file if it couldn't be found via File.Exists()</param>
    /// <returns></returns>
    public static ArgonTexture CreateFromFile(string path, Assembly? containingAssembly = null)
    {
        var fs = ArgonManager.LoadResourceFile(path, containingAssembly);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        var name = path[..^ext.Length];
        TextureCompression compression = ext switch
        {
            ".bmp" => TextureCompression.BMP,
            ".dds" => TextureCompression.DDS,
            ".tga" => TextureCompression.TGA,
            ".png" => TextureCompression.PNG,
            ".jpg" => TextureCompression.JPEG,
            ".jpeg" => TextureCompression.JPEG,
            ".raw" => TextureCompression.Raw,
            ".bin" => TextureCompression.Raw,
            _ => TextureCompression.Unknown,
        };
        return CreateFromStream(name, fs, compression);
    }

    /// <summary>
    /// Creates a new texture and loads it from a stream asynchronously.
    /// <para/>
    /// Supports: BMP, DDS, TGA, PNG, JPG
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="stream">The stream containing the compressed texture file.</param>
    /// <param name="compression">The compression used by the texture.</param>
    /// <returns></returns>
    public static ArgonTexture CreateFromStream(string name, Stream stream, TextureCompression compression)
    {
        ArgonTexture tex = new();
        tex.drawCommands.Enqueue(ctx =>
        {
            var handle = ctx.LoadTexture(stream, name, compression);
            tex.TextureHandle = handle;
        });
        return tex;
    }

    /// <summary>
    /// Creates a new texture and loads it from memory asynchronously.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="data">The struct containing the raw texture data to load.</param>
    /// <param name="compression">The compression used by the texture. (Should be None)</param>
    /// <returns></returns>
    public static ArgonTexture CreateFromData(string name, TextureData data, TextureCompression compression)
    {
        ArgonTexture tex = new();
        tex.drawCommands.Enqueue(ctx =>
        {
            var handle = ctx.LoadTexture(data, name, compression);
            tex.TextureHandle = handle;
        });
        return tex;
    }

    /*public void UpdateFromData(TextureData data)
    {
        drawCommands.Enqueue(ctx =>
        {
            ctx.UpdateTexture(TextureHandle, data);
        });
    }*/

    /// <summary>
    /// Should be called by consumming <see cref="UIElements.UIElement"/> of this texture in their 
    /// <see cref="UIElements.UIElement.Draw(Bounds2D, List{Action{IDrawContext}})"/> method.
    /// </summary>
    /// <param name="context"></param>
    public void ExecuteDrawCommands(IDrawContext context)
    {
        while (!drawCommands.IsEmpty)
        {
            if (drawCommands.TryDequeue(out var cmd))
            {
                cmd(context);
            }
        }
    }
}

public readonly struct TextureData(uint width, uint height, TextureFormat format, Memory<byte> data)
{
    public readonly uint width = width;
    public readonly uint height = height;
    public readonly TextureFormat format = format;
    public readonly int bytesPerPixel = BytesPerPixel(format);
    public readonly ReadOnlyMemory<byte> data = data;

    //public byte this[int x] => data.Span[x];
    //public byte this[int x, int y] => data.Span[(int)((uint)x + (uint)y * width)];

    private static int BytesPerPixel(TextureFormat format)
    {
        return format switch
        {
            TextureFormat.R8 => 1,
            //TextureFormat.Gray => 1,
            TextureFormat.R16 => 2,
            TextureFormat.R32 => 4,
            TextureFormat.RGB8 => 3,
            TextureFormat.RGBA8 => 4,
            TextureFormat.RGB565 => 2,
            TextureFormat.RGBA32 => 16,
            _ => 1
        };
    }
}
