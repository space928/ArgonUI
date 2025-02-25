using ArgonUI.Drawing;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Backends.OpenGL;

public static class Texture2DLoader
{
    private static readonly ConcurrentQueue<TextureResult> textureResults = [];

    /// <summary>
    /// Creates an OpenGL texture and loads it asynchornously.
    /// </summary>
    /// <param name="gl">The OpenGL context.</param>
    /// <param name="name">The name of the texture to create.</param>
    /// <param name="data">The data to initalise the texture with.</param>
    /// <param name="compression">The compression format of the texture.</param>
    /// <returns>A new texture object.</returns>
    public static Texture2D LoadTexture(GL gl, string? name, TextureData data, TextureCompression compression)
    {
        var tex = new Texture2D(gl, name);
        tex.IsValid = false;
        Task.Run(() =>
        {
            textureResults.Enqueue(ReadTexture(tex, data, compression));
        });
        return tex;
    }

    /// <inheritdoc cref="LoadTexture(GL, string?, TextureData, TextureCompression)"/>
    public static Texture2D LoadTexture(GL gl, string? name, Stream data, TextureCompression compression)
    {
        var tex = new Texture2D(gl, name);
        tex.IsValid = false;
        Task.Run(() =>
        {
            textureResults.Enqueue(ReadTexture(tex, data, compression));
        });
        return tex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gl"></param>
    internal static void LoadTextureResults(GL gl)
    {
        // This method gets called every frame by Program in the main thread to synchronise any loaded TextureResults.
        int loaded = 0;
        int maxTexturesLoadedPerFrame = 8;
        while (!textureResults.IsEmpty && loaded < maxTexturesLoadedPerFrame)
        {
            if (textureResults.TryDequeue(out var res))
            {
                LoadTextureResult(gl, res);
                loaded++;
            }
        }
    }

    private static void LoadTextureResult(GL gl, TextureResult t)
    {
        var img = t;
        t.targetTex.Bind();
        if (img.compression != GLTextureCompressionType.None)
        {
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            for (int m = 0; m < img.data.Length; m++)
            {
                var mip = img.data[m];
                gl.CompressedTexImage2D<byte>(TextureTarget.Texture2D, m, img.internalFormat, mip.width, mip.height, 0, (uint)mip.data.Length, mip.data.Span);
            }
        }
        else
        {
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            for (int m = 0; m < img.data.Length; m++)
            {
                var mip = img.data[m];
                gl.TexImage2D<byte>(TextureTarget.Texture2D, m, img.internalFormat, mip.width, mip.height, 0, img.pixelFormat, img.pixelType, mip.data.Span);
            }
        }
        gl.GenerateMipmap(TextureTarget.Texture2D);
        gl.TextureParameter(t.targetTex.Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        gl.TextureParameter(t.targetTex.Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        gl.TextureParameter(t.targetTex.Handle, TextureParameterName.TextureMaxAnisotropy, 8);
        t.targetTex.Unbind();
        if (img.data.Length > 0)
        {
            t.targetTex.UpdateMetadata(img.data[0].width, img.data[0].height, t.pixelFormat, t.pixelType, t.internalFormat, t.compression);
        }
        //t.targetTex.IsValid = true;

        //isValid = false;
    }

    private static TextureResult ReadTexture(Texture2D target, TextureData data, TextureCompression compression)
    {
        if (compression == TextureCompression.Raw)
        {
            var (pixFmt, pixType, intFmt) = GlFormatFromTextureFormat(data.format);
            return new TextureResult()
            {
                targetTex = target,
                pixelFormat = pixFmt,
                internalFormat = intFmt,
                pixelType = pixType,
                compression = GLTextureCompressionType.None,
                data = [new() { width = data.width, height = data.height, data = data.data }]
            };
        }
        else
        {
            return ReadTexture(target, new MemoryBufferStream<byte>(data.data), compression);
        }
    }

    private static TextureResult ReadTexture(Texture2D target, Stream stream, TextureCompression compression)
    {
        // This function is thread-safe so it can be run in a task to load textures asynchronously.
        switch (compression)
        {
            case TextureCompression.DDS:
                try
                {
                    var img = DDSReader.ReadFromStream(stream);

                    if (img.textureType != TextureType.Texture2D)
                        throw new NotImplementedException("Can't load non-2D texture!");

                    return new TextureResult()
                    {
                        targetTex = target,
                        pixelFormat = img.format,
                        internalFormat = img.internalFormat,
                        pixelType = img.pixelType,
                        compression = img.compression,
                        data = img.data[0].mipMaps.Select(x => new TextureResult.MipMap()
                        {
                            width = x.width,
                            height = x.height,
                            data = x.data
                        }).ToArray()
                    };
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    stream.Dispose();
                }
            case TextureCompression.PNG:
            case TextureCompression.JPEG:
            case TextureCompression.TGA:
            case TextureCompression.BMP:
                try
                {
                    var img = ImageResult.FromStream(stream);
                    var (pixelFormat, internalFormat, pixelType) = img.Comp.ToOpenGLFormat();
                    return new TextureResult()
                    {
                        targetTex = target,
                        pixelFormat = pixelFormat,
                        internalFormat = internalFormat,
                        pixelType = pixelType,
                        compression = GLTextureCompressionType.None,
                        data = [new() { width = (uint)img.Width, height = (uint)img.Height, data = img.Data }]
                    };
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    stream.Dispose();
                }
            default:
                stream.Dispose();
                throw new NotImplementedException();
        }
    }

    internal static TextureFormat TextureFormatFromGlFormat(PixelFormat pixelFormat, PixelType pixelType)
    {
        return (pixelFormat, pixelType) switch
        {
            (PixelFormat.Red, PixelType.UnsignedByte) => TextureFormat.R8,
            (PixelFormat.Rgb, PixelType.UnsignedByte) => TextureFormat.RGB8,
            (PixelFormat.Rgba, PixelType.UnsignedByte) => TextureFormat.RGBA8,
            (PixelFormat.Alpha, PixelType.UnsignedByte) => TextureFormat.Gray,
            (PixelFormat.Green, PixelType.UnsignedByte) => TextureFormat.Gray,
            (PixelFormat.Blue, PixelType.UnsignedByte) => TextureFormat.Gray,
            (PixelFormat.Red, PixelType.Byte) => TextureFormat.Gray,
            (PixelFormat.Red, PixelType.HalfFloat) => TextureFormat.R16,
            (PixelFormat.Red, PixelType.Short) => TextureFormat.R16,
            (PixelFormat.Red, PixelType.UnsignedShort) => TextureFormat.R16,
            (PixelFormat.Red, PixelType.Float) => TextureFormat.R32,
            (PixelFormat.Red, PixelType.Int) => TextureFormat.R32,
            (PixelFormat.Red, PixelType.UnsignedInt) => TextureFormat.R32,
            (PixelFormat.Rgba, PixelType.Float) => TextureFormat.RGBA32,
            (PixelFormat.Rgba, PixelType.UnsignedInt) => TextureFormat.RGBA32,
            (PixelFormat.Rgba, PixelType.Int) => TextureFormat.RGBA32,
            (PixelFormat.Rgb, PixelType.UnsignedShort565) => TextureFormat.RGB565,
            _ => TextureFormat.Unsupported
        };
    }

    internal static (PixelFormat pixelFormat, PixelType pixelType, InternalFormat internalFormat) GlFormatFromTextureFormat(TextureFormat format)
    {
        return format switch
        {
            TextureFormat.R8 => (PixelFormat.Red, PixelType.Byte, InternalFormat.R8),
            TextureFormat.RGB8 => (PixelFormat.Rgb, PixelType.Byte, InternalFormat.Rgb8),
            TextureFormat.RGBA8 => (PixelFormat.Rgba, PixelType.Byte, InternalFormat.Rgba8),
            TextureFormat.R16 => (PixelFormat.Red, PixelType.Short, InternalFormat.R16ui),
            TextureFormat.R32 => (PixelFormat.Red, PixelType.Float, InternalFormat.R32f),
            TextureFormat.RGBA32 => (PixelFormat.Rgba, PixelType.Float, InternalFormat.Rgba32f),
            TextureFormat.RGB565 => (PixelFormat.Rgb, PixelType.UnsignedShort565, InternalFormat.Rgb565),
            _ => throw new NotSupportedException("Couldn't convert texture format to compatible OpenGL texture.")
        };
    }

    internal class TextureResult
    {
        public required Texture2D targetTex;
        public PixelFormat pixelFormat;
        public InternalFormat internalFormat;
        public PixelType pixelType;
        public GLTextureCompressionType compression;
        public required MipMap[] data;

        public struct MipMap
        {
            public uint width, height;
            public ReadOnlyMemory<byte> data;
        }
    }

    private class MemoryBufferStream<T> : Stream
        where T : struct
    {
        private long position;

        public readonly ReadOnlyMemory<T> data;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => data.Length;
        public override long Position { get => position; set => position = value; }

        public MemoryBufferStream(ReadOnlyMemory<T> data)
        {
            this.data = data;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int toCopy = Math.Min(Math.Min(count, data.Length - (int)position), buffer.Length - offset);
            if (toCopy > 0)
            {
                var dst = MemoryMarshal.Cast<byte, T>(buffer.AsSpan().Slice(offset, toCopy));
                var src = data.Span.Slice((int)position, toCopy);
                src.CopyTo(dst);
            }
            return toCopy;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.Current:
                    position += offset;
                    break;
                case SeekOrigin.End:
                    position = Length + offset;
                    break;
            }
            return position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
