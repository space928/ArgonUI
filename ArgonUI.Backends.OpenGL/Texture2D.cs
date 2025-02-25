using ArgonUI.Drawing;
using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace ArgonUI.Backends.OpenGL;

public class Texture2D : ITextureHandle, IDisposable
{
    private readonly string name;

    public string Name => name;
    public uint Width => width;
    public uint Height => height;
    public TextureCompression Compression => TextureCompression.Unknown;
    public bool IsLoaded => isValid;
    TextureFormat ITextureHandle.Format => Texture2DLoader.TextureFormatFromGlFormat(pixelFormat, pixelType);

    public bool IsValid { get => isValid; internal set => isValid = value; }
    public PixelFormat Format => pixelFormat;
    internal uint Handle => handle;

    protected readonly uint handle;
    protected readonly GL gl;
    protected PixelFormat pixelFormat;
    protected InternalFormat internalFormat;
    protected PixelType pixelType;
    protected GLTextureCompressionType compressionType;
    protected bool isValid = true;
    protected uint width;
    protected uint height;

    private static uint currentTexture = 0;
    private static Texture2D? missingTexture;

    public Texture2D(GL gl, string? name = null)
    {
        this.name = name ?? "ArgonTexture";
        this.gl = gl;
        handle = gl.CreateTexture(TextureTarget.Texture2D);
    }

    private static Texture2D CreateMissingTexture(GL gl)
    {
        var tex = new Texture2D(gl, "MissingTexture");
        tex.Bind();

        uint size = 32;
        uint halfSize = size / 2;
        uint[] data = new uint[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                data[y * size + x] = ((x >= halfSize) ^ (y >= halfSize)) ? 0xff000000 : 0xffff00ff;
            }
        }

        gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        gl.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, size, size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, MemoryMarshal.AsBytes<uint>(data));
        gl.GenerateMipmap(TextureTarget.Texture2D);
        gl.TextureParameter(tex.handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        gl.TextureParameter(tex.handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        gl.TextureParameter(tex.handle, TextureParameterName.TextureMaxAnisotropy, 8);
        tex.Unbind();

        tex.width = size;
        tex.height = size;

        return tex;
    }

    /// <summary>
    /// Updates the content of this texture from a byte buffer. The <paramref name="data"/> must 
    /// match the <paramref name="size"/> parameter.
    /// </summary>
    /// <param name="offset">The top-left coordinates of the rectangle to update.</param>
    /// <param name="size">The size of the rectangle to update.</param>
    /// <param name="data">The data to upload to the texture.</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Update(VectorInt2 offset, VectorInt2 size, TextureData data)
    {
        if (compressionType != GLTextureCompressionType.None)
            throw new NotImplementedException("Can't update texture data for a compressed texture!");

        if (data.width != size.x || data.height != size.y)
            throw new ArgumentOutOfRangeException(nameof(size));

        Bind();
        gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        gl.TexSubImage2D<byte>(TextureTarget.Texture2D, 0, offset.x, offset.y, (uint)size.x, (uint)size.y, pixelFormat, pixelType, data.data.Span);
        gl.GenerateMipmap(TextureTarget.Texture2D);
        Unbind();
    }

    public void Bind()
    {
        if (!isValid)
        {
            missingTexture ??= CreateMissingTexture(gl);
            missingTexture.Bind();
            return;
        }

        if (currentTexture != handle)
            gl.BindTexture(TextureTarget.Texture2D, handle);
        currentTexture = handle;
    }

    public void Bind(uint unit)
    {
        if (!isValid)
        {
            missingTexture ??= CreateMissingTexture(gl);
            missingTexture.Bind(unit);
            return;
        }

        gl.BindTextureUnit(unit, handle);
    }

    public static void BindTextures(uint startUnit, ReadOnlySpan<Texture2D> textures)
    {
        if (textures.Length < 1)
            return;

        Span<uint> handles = stackalloc uint[textures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            var tex = textures[i];
            var handle = tex.handle;
            if (!tex.isValid)
                handle = (missingTexture ??= CreateMissingTexture(tex.gl)).handle;
            handles[i] = handle;
        }

        var gl = textures[0].gl;
        gl.BindTextures(startUnit, handles);
    }

    public void Unbind()
    {
        if (missingTexture != null && currentTexture == missingTexture.handle && this != missingTexture)
        {
            missingTexture.Unbind();
            return;
        }
        if (currentTexture != handle)
            throw new Exception($"Can't unbind texture {handle} as it isn't bound! (current = {currentTexture})");
        gl.BindTexture(TextureTarget.Texture2D, 0);
        currentTexture = 0;
    }

    public void Dispose()
    {
        gl.DeleteTexture(handle);
        isValid = false;
    }

    internal void UpdateMetadata(uint width, uint height, PixelFormat pixFmt, PixelType pixType, InternalFormat intFmt, GLTextureCompressionType compressionType)
    {
        this.width = width;
        this.height = height;
        this.pixelFormat = pixFmt;
        this.pixelType = pixType;
        this.internalFormat = intFmt;
        this.compressionType = compressionType;
    }
}
