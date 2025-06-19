using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace ArgonUI.Backends.Headless;

public class HeadlessDrawContext : IDrawContext
{
    public void Clear(Vector4 colour)
    {
        
    }

    public void Dispose()
    {
        
    }

    public void DrawChar(Bounds2D bounds, float size, char c, BMFont font, Vector4 colour)
    {
        
    }

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        
    }

    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding)
    {
        
    }

    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur)
    {
        
    }

    public void DrawText(Bounds2D bounds, float size, string s, BMFont font, Vector4 colour)
    {
        
    }

    public void DrawText(Bounds2D bounds, float size, string s, BMFont font, Vector4 colour, float wordSpacing = 0, float charSpacing = 0, float skew = 0, float weight = 0.5F, float width = 1)
    {
        
    }

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        
    }

    public void EndFrame()
    {
        
    }

    public void FlushBatch()
    {
        
    }

    public void InitRenderer(UIWindow window)
    {
        
    }

    public ITextureHandle LoadTexture(Stream data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        return new HeadlessTexture2D(name, new(16, 16, TextureFormat.Unknown, Memory<byte>.Empty));
    }

    public ITextureHandle LoadTexture(TextureData data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        return new HeadlessTexture2D(name, data);
    }

    public void StartFrame(Bounds2D region)
    {
        
    }
}
