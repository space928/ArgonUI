using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Backends.OpenGL;

internal class OpenGLDrawContext : IDrawContext
{
    private readonly GL gl;

    public OpenGLDrawContext(GL gl)
    {
        this.gl = gl;
    }

    public void Clear(Vector4 colour)
    {
        gl.ClearColor(colour.X, colour.Y, colour.Z, colour.W);
    }

    public void DrawChar(Bounds2D bounds, float size, char c, IFontHandle font, Vector4 colour)
    {
        throw new NotImplementedException();
    }

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        throw new NotImplementedException();
    }

    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding)
    {
        //throw new NotImplementedException();
    }

    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur)
    {
        throw new NotImplementedException();
    }

    public void DrawText(Bounds2D bounds, float size, string c, IFontHandle font, Vector4 colour)
    {
        throw new NotImplementedException();
    }

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        throw new NotImplementedException();
    }

    public void FlushBatch()
    {
        throw new NotImplementedException();
    }
}
