using ArgonUI.Drawing;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Backends.OpenGL;

public class Texture2DArgon : Texture2D, ITextureHandle
{
    private string name;

    public string Name => name;
    public uint Width => Width;
    public uint Height => Height;
    public TextureCompression Compression => throw new NotImplementedException();
    public bool IsLoaded => isValid;
    TextureFormat ITextureHandle.Format => throw new NotImplementedException();

    public Texture2DArgon(GL gl, Stream data, string? name = null, TextureCompression compression = TextureCompression.Unknown) : base(gl)
    {
        this.name = name ?? "ArgonTexture";

    }

    #region Async Texture Loading
    internal static void LoadTextureResults()
    {
        // This method gets called every frame by Program in the main thread to synchornise any loaded TextureResults.
        int loaded = 0;
        int maxTexturesLoadedPerFrame = 16;
        while (!textureResults.IsEmpty && loaded < maxTexturesLoadedPerFrame)
        {
            if (textureResults.TryDequeue(out var res))
            {
                res.tex.LoadTextureResult(res.data);
                loaded++;
            }
        }
    }

    private void LoadTextureResult(in TextureResult t)
    {
        var img = t;
        isValid = true;
        Bind();
        if (img.compression != TextureCompressionType.None)
        {
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            for (int m = 0; m < img.data.Length; m++)
            {
                var mip = img.data[m];
                gl.CompressedTexImage2D<byte>(TextureTarget.Texture2D, m, img.internalFormat, mip.width, mip.height, 0, (uint)mip.data.Length, mip.data.AsSpan());
            }
        }
        else
        {
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            for (int m = 0; m < img.data.Length; m++)
            {
                var mip = img.data[m];
                gl.TexImage2D<byte>(TextureTarget.Texture2D, m, img.internalFormat, mip.width, mip.height, 0, img.pixelFormat, img.pixelType, mip.data.AsSpan());
            }
        }
        gl.GenerateMipmap(TextureTarget.Texture2D);
        gl.TextureParameter(handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        gl.TextureParameter(handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        gl.TextureParameter(handle, TextureParameterName.TextureMaxAnisotropy, 8);
        Unbind();

        //isValid = false;
    }
    #endregion

    private static TextureResult ReadTexture(string path, TextureCompression compression)
    {
        // This function is thread-safe so it can be run in a task to load textures asynchronously.
        switch (compression)
        {
            case TextureCompression.DDS:
                try
                {
                    using var f = File.OpenRead(path);
                    var img = DDSReader.ReadFromStream(f);

                    if (img.textureType != TextureType.Texture2D)
                        throw new NotImplementedException("Can't load non-2D texture!");

                    return new TextureResult()
                    {
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
            case TextureCompression.PNG:
            case TextureCompression.JPEG:
            case TextureCompression.TGA:
            case TextureCompression.BMP:
                try
                {
                    using var f = File.OpenRead(path);
                    var img = ImageResult.FromStream(f);
                    var (pixelFormat, internalFormat, pixelType) = img.Comp.ToOpenGLFormat();
                    return new TextureResult()
                    {
                        pixelFormat = pixelFormat,
                        internalFormat = internalFormat,
                        pixelType = pixelType,
                        compression = TextureCompressionType.None,
                        data = [new() { width = (uint)img.Width, height = (uint)img.Height, data = img.Data }]
                    };
                }
                catch (Exception)
                {
                    throw;
                }
            default:
                throw new NotImplementedException();
        }
    }

    internal struct TextureResult
    {
        public PixelFormat pixelFormat;
        public InternalFormat internalFormat;
        public PixelType pixelType;
        public TextureCompressionType compression;
        public MipMap[] data;

        public struct MipMap
        {
            public uint width, height;
            public byte[] data;
        }
    }
}
