using ArgonUI.Drawing;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Backends.OpenGL;

internal class OpenGLDrawContext : IDrawContext
{
    private readonly GL gl;
    private UIWindow? window;
    private Vector2 resolution;
    private ShaderManager shaderManager;

    private Texture2D? activeTexture;
    private int vertPos;
    private int vertCount;
    private readonly uint[] vertBuff;

    public bool IsInitialised => window != null;

    public OpenGLDrawContext(GL gl)
    {
        this.gl = gl;
        vertBuff = new uint[32*1024];
        shaderManager = new();
    }

#if DEBUG_LATENCY || DEBUG
    internal bool dbg_latencyTimerEnd;
    internal string? dbg_latencyMsg;

    public void MarkLatencyTimerEnd(string? msg = null)
    {
        dbg_latencyTimerEnd = true;
        dbg_latencyMsg = msg;
    }
#endif

    public void InitRenderer(UIWindow window)
    {
        this.window = window;

        gl.FrontFace(FrontFaceDirection.CW);
        //gl.Enable(EnableCap.DepthTest);
        //gl.DepthFunc(DepthFunction.Lequal);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        gl.Enable(EnableCap.Multisample);
        gl.Enable(EnableCap.Blend);
        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);
        gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        gl.Enable(EnableCap.PolygonOffsetFill);
        gl.Enable(EnableCap.PolygonOffsetLine);
        gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        gl.Enable(EnableCap.ScissorTest);

        shaderManager.Init(gl);
        //vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        /*}
        catch (Exception ex)
        {
            onErrorMessageCB("Error while initialising the renderer!", ex.ToString());
        }*/
    }

    public void Dispose()
    {
        shaderManager.Dispose();
        gl.Dispose();
    }

    public void StartFrame(Bounds2D region)
    {
        //VertexArrayObject<float, uint>.UnbindAny(gl);
        //gl.Clear(ClearBufferMask.DepthBufferBit);
        var size = window?.Size ?? VectorInt2.Zero;
        resolution = new(size.x, size.y);

        gl.Viewport(0, 0, (uint)size.x, (uint)size.y);

        int height = (int)region.Height;
        if (height > 0)
        {
            //gl.Enable(EnableCap.ScissorTest);
            gl.Scissor((int)region.topLeft.X, size.y - (int)region.bottomRight.Y, (uint)region.Width, (uint)height);
        }
        else
        {
            gl.Scissor(0, 0, (uint)resolution.X, (uint)resolution.Y);
            //gl.Disable(EnableCap.ScissorTest);
        }
    }

    public void EndFrame()
    {
        Texture2DLoader.LoadTextureResults(gl);

#if DEBUG_LATENCY
        if (dbg_latencyTimerEnd)
        {
            dbg_latencyTimerEnd = false;
            var timeEnd = DateTime.UtcNow.Ticks;

            ((OpenGLWindow)window!).LogLatency(timeEnd, $"end of frame ({dbg_latencyMsg})");
            dbg_latencyMsg = null;
        }
#endif
    }

    public void Clear(Vector4 colour)
    {
        gl.ClearColor(colour.X, colour.Y, colour.Z, colour.W);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    /// <summary>
    /// Checks if the vertex buffer has enough space for this draw.
    /// </summary>
    /// <typeparam name="TVert">The type of vertex to check space for.</typeparam>
    /// <param name="elems">The number of vertices to check for space for.</param>
    /// <returns><see langword="true"/> if the batch needs to be flushed.</returns>
    private bool CheckSpace<TVert>(int elems)
    {
        return (vertBuff.Length * Unsafe.SizeOf<uint>()) - vertPos < Unsafe.SizeOf<TVert>() * elems;
    }

    #region Rectangles
    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Rounding | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        Vector4 rectProps = new(bounds.Size, rounding, 0);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Rounding | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        Vector4 rectProps = new(bounds.Size, rounding, 0);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colourB, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colourC, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Rounding | ShaderFeature.Blur | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        Vector4 rectProps = new(bounds.Size, rounding, blur);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawOutlineRect(Bounds2D bounds, Vector4 colour, float outlineThickness, float rounding)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Outline | ShaderFeature.Rounding | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        Vector4 rectProps = new(bounds.Size, rounding, outlineThickness);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawOutlineGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float outlineThickness, float rounding)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Outline | ShaderFeature.Rounding | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        Vector4 rectProps = new(bounds.Size, rounding, 0);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colourB, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colourC, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        if (texture is not Texture2D tex)
            throw new NotSupportedException("Attempted to call DrawTexture with an incompatible texture object!");

        ShaderFeature shaderFeatures = ShaderFeature.Rounding | ShaderFeature.Alpha | ShaderFeature.Texture;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectRoundVert>(6))
            FlushBatch();

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
        }

        Vector4 rectProps = new(bounds.Size, rounding, 0);
        var colour = Vector4.One;

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectRoundVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectRoundVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectRoundVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }
    #endregion

    #region Text
    public void DrawChar(Bounds2D bounds, float size, char c, BMFont font, Vector4 colour)
    {
        float fontSize = size / font.Size;
        DrawCharInternal(bounds.topLeft, fontSize, 0, 0.5f, 1, font.FontTexture?.TextureHandle, font.CharsDict[c], colour);
    }

    public void DrawText(Bounds2D bounds, float size, string s, BMFont font, Vector4 colour)
    {
        var tex = font.FontTexture?.TextureHandle;
        if (tex == null || !tex.IsLoaded)
            return;
        var dict = font.CharsDict;
        //float initialX = bounds.topLeft.X;
        float fontSize = size / font.Size;
        foreach (var c in s)
        {
            var cDef = dict[c];
            if (bounds.topLeft.X /*+ cDef.xAdvance * fontSize*/ > bounds.bottomRight.X)
                break;
            float adv = DrawCharInternal(in bounds.topLeft, fontSize, 0, 0.5f, 1, tex, in cDef, in colour);
            bounds.topLeft.X += adv;
        }
    }

    public void DrawText(Bounds2D bounds, ReadOnlySpan<char> s, BMFont font, float size, Vector4 colour,
        float wordSpacing = 0, float charSpacing = 0, float skew = 0, float weight = 0.5F, float width = 1)
    {
        var tex = font.FontTexture?.TextureHandle;
        if (tex == null || !tex.IsLoaded)
            return;
        var dict = font.CharsDict;
        //float initialX = bounds.topLeft.X;
        float fontSize = size / font.Size;
        foreach (var c in s)
        {
            //if (char.IsWhiteSpace(c))
            if (wordSpacing != 0 && c == ' ')
            {
                // TODO: Support different sized spaces
                bounds.topLeft.X += wordSpacing;
                continue;
            }

            var cDef = dict[c];
            if (bounds.topLeft.X > bounds.bottomRight.X)
                break;
            float adv = DrawCharInternal(in bounds.topLeft, fontSize, skew, weight, width, tex, in cDef, in colour);
            bounds.topLeft.X += adv + charSpacing;
        }
    }

    private float DrawCharInternal(in Vector2 pos, float size, float skew, float weight, float width, ITextureHandle? texture, in BMFontChar c, in Vector4 colour)
    {
        // TODO: Implement text rendering skew
        if (texture is not Texture2D tex)
            throw new NotSupportedException("Attempted to call DrawTexture with an incompatible texture object!");

        if (tex.Width == 0 || tex.Height == 0)
            return 0f;

        ShaderFeature shaderFeatures = ShaderFeature.Text | ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<CharVert>(6))
            FlushBatch();

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
        }

        Vector2 fontSizeVec = new(size * width, size);
        Vector2 tl = c.offset * fontSizeVec;
        Vector2 br = (c.offset + c.size) * fontSizeVec;
        tl += pos;
        br += pos;
        Vector2 texScale = new(1f / activeTexture.Width, 1f / activeTexture.Height);
        Vector2 uv_tl = c.pos * texScale;
        Vector2 uv_br = (c.pos + c.size) * texScale;

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new CharVert(new(tl.X, br.Y), colour, new(uv_tl.X, uv_br.Y, weight)));
        writer.Write(new CharVert(tl, colour, new(uv_tl, weight)));
        writer.Write(new CharVert(br, colour, new(uv_br, weight)));
        writer.Write(new CharVert(br, colour, new(uv_br, weight)));
        writer.Write(new CharVert(tl, colour, new(uv_tl, weight)));
        writer.Write(new CharVert(new(br.X, tl.Y), colour, new(uv_br.X, uv_tl.Y, weight)));
        vertCount += 6;
        vertPos = writer.Offset;

        return c.xAdvance * fontSizeVec.X;
    }
    #endregion

    #region Lines & Polys
    public void DrawLine(Vector2 start, Vector2 end, Vector4 colourStart, Vector4 colourEnd, float thickness)
    {
        ShaderFeature shaderFeatures = ShaderFeature.Alpha;
        if (!shaderManager.IsShaderActive(shaderFeatures))
        {
            FlushBatch();
            shaderManager.SetActiveShader(gl, shaderFeatures);
        }

        if (CheckSpace<RectVert>(6))
            FlushBatch();

        Vector2 dir = end - start;
        dir = Vector2.Normalize(dir);
        Vector2 perp = new Vector2(-dir.Y, dir.X) * thickness;
        Vector2 a_0 = start + perp;
        Vector2 a_1 = start - perp;
        Vector2 b_0 = end + perp;
        Vector2 b_1 = end - perp;

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectVert(a_0, colourStart, new(0, 1)));
        writer.Write(new RectVert(a_1, colourStart, new(0, 0)));
        writer.Write(new RectVert(b_0, colourEnd, new(1, 1)));
        writer.Write(new RectVert(b_0, colourEnd, new(1, 1)));
        writer.Write(new RectVert(a_1, colourStart, new(0, 0)));
        writer.Write(new RectVert(b_1, colourEnd, new(1, 0)));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawPolyFill(IEnumerable<IDrawContext.PolyVert> points)
    {
        throw new NotImplementedException();
    }

    public void DrawPolyLine(IEnumerable<IDrawContext.PolyVert> points, float thickness)
    {
        throw new NotImplementedException();
    }
    #endregion

    public void FlushBatch()
    {
        if (vertPos == 0)
            return;

        if (shaderManager.ActiveShader != null)
        {
            //shaderManager.ActiveShader.Use();

            shaderManager.ActiveShader.SetUniform("uResolution", resolution);
            // Set all other program properties...
            // Bind textures and stuff
            activeTexture?.Bind(0);

            var vao = shaderManager.ActiveVAO!;

            vao.VBO.Update(vertBuff.AsSpan(0, vertPos >> 2));

            vao.Bind();
            gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)vertCount);
            vao.Unbind();
        }

        vertPos = 0;
        vertCount = 0;
    }

    public ITextureHandle LoadTexture(Stream data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        return Texture2DLoader.LoadTexture(gl, name, data, compression);
    }

    public ITextureHandle LoadTexture(TextureData data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        return Texture2DLoader.LoadTexture(gl, name, data, compression);
    }
}
