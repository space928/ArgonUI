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
    private Shader? rectShader;
    private Shader? textShader;
    private Shader? textureShader;
    private VertexArrayObject<uint, uint>? rectVAO;
    private VertexArrayObject<uint, uint>? textVAO;

    private VertexArrayObject<uint, uint>? activeVao;
    private Shader? activeShader;
    private Texture2D? activeTexture;
    private int vertPos;
    private int vertCount;
    private readonly uint[] vertBuff;

    public bool IsInitialised => window != null;

    public OpenGLDrawContext(GL gl)
    {
        this.gl = gl;
        vertBuff = new uint[32*1024];
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
        //try
        //{
        rectShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_ROUNDING", "#define SUPPORT_ALPHA"]);
        textShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_TEXT", "#define SUPPORT_ALPHA"]);
        textureShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_TEXTURE", "#define SUPPORT_ROUNDING", "#define SUPPORT_ALPHA"]);

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

        InitRectVAO();
        InitTextVAO();
        //vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        /*}
        catch (Exception ex)
        {
            onErrorMessageCB("Error while initialising the renderer!", ex.ToString());
        }*/
    }

    public void Dispose()
    {
        rectVAO?.Dispose();
        textVAO?.Dispose();
        //vao?.Dispose();
        rectShader?.Dispose();
        textShader?.Dispose();
        textureShader?.Dispose();
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
    /// <typeparam name="T"></typeparam>
    /// <param name="elems"></param>
    /// <returns><see langword="true"/> if the batch needs to be flushed.</returns>
    private bool CheckSpace<T>(int elems)
    {
        return (vertBuff.Length * Unsafe.SizeOf<uint>()) - vertPos < Unsafe.SizeOf<RectVert>() * elems;
    }

    private void InitRectVAO()
    {
        rectVAO?.Dispose();

        VertexArrayObject<uint, uint> vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        vao.Bind();
        vao.VertexAttributePointer(0, 2, VertexAttribType.Float, 11, 0);
        vao.VertexAttributePointer(1, 4, VertexAttribType.Float, 11, 2);
        vao.VertexAttributePointer(3, 2, VertexAttribType.Float, 11, 6);
        vao.VertexAttributePointer(4, 3, VertexAttribType.Float, 11, 8);
        vao.Unbind();
        rectVAO = vao;
    }

    private void InitTextVAO()
    {
        textVAO?.Dispose();

        VertexArrayObject<uint, uint> vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        vao.Bind();
        vao.VertexAttributePointer(0, 2, VertexAttribType.Float, 9, 0);
        vao.VertexAttributePointer(1, 4, VertexAttribType.Float, 9, 2);
        vao.VertexAttributePointer(2, 3, VertexAttribType.Float, 9, 6);
        vao.Unbind();
        textVAO = vao;
    }

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        if (activeShader != rectShader || CheckSpace<RectVert>(6))
            FlushBatch();

        if (activeShader != rectShader)
        {
            activeShader = rectShader;
            activeVao = rectVAO;
            activeTexture = null;
        }
        Vector3 rectProps = new(bounds.Width, bounds.Height, rounding);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colourB, new(0, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colourD, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colourA, new(0, 0), rectProps));
        writer.Write(new RectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colourC, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding)
    {
        if (activeShader != rectShader || CheckSpace<RectVert>(6))
            FlushBatch();

        if (activeShader != rectShader)
        {
            activeShader = rectShader;
            activeVao = rectVAO;
            activeTexture = null;
        }

        Vector3 rectProps = new(bounds.Width, bounds.Height, rounding);

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void DrawShadow(Bounds2D bounds, Vector4 colour, float rounding, float blur)
    {
        throw new NotImplementedException();
    }

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

    public void DrawText(Bounds2D bounds, float size, string s, BMFont font, Vector4 colour, 
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

        if (activeShader != textShader || CheckSpace<CharVert>(6))
            FlushBatch();

        if (activeShader != textShader)
        {
            activeShader = textShader;
            textShader?.Use();
            textShader?.SetUniform("uFontTex", 0);
            activeVao = textVAO;
        }

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
        }

        float size_x = size * width;
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

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        if (texture is not Texture2D tex)
            throw new NotSupportedException("Attempted to call DrawTexture with an incompatible texture object!");

        if (activeShader != textShader || CheckSpace<CharVert>(6))
            FlushBatch();

        if (activeShader != textureShader)
        {
            activeShader = textureShader;
            textureShader?.Use();
            textureShader?.SetUniform("uMainTex", 0);
            activeVao = rectVAO;
        }

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
        }

        Vector3 rectProps = new(bounds.Width, bounds.Height, rounding);
        var colour = Vector4.One;

        var writer = vertBuff.GetBinaryWriter(vertPos);
        writer.Write(new RectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colour, new(0, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.bottomRight, colour, new(1, 1), rectProps));
        writer.Write(new RectVert(bounds.topLeft, colour, new(0, 0), rectProps));
        writer.Write(new RectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colour, new(1, 0), rectProps));
        vertCount += 6;
        vertPos = writer.Offset;
    }

    public void FlushBatch()
    {
        if (vertPos == 0)
            return;

        if (activeShader != null)
        {
            activeShader.Use();

            activeShader.SetUniform("uResolution", resolution);
            // Set all other program properties...
            // Bind textures and stuff
            activeTexture?.Bind(0);

            var vao = activeVao!;

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

    [StructLayout(LayoutKind.Explicit)]
    readonly struct RectVert(Vector2 pos, Vector4 col, Vector2 texcoord, Vector3 rounding)
    {
        [FieldOffset(0)] public readonly Vector2 pos = pos;
        [FieldOffset(0x8)] public readonly Vector4 col = col;
        [FieldOffset(0x18)] public readonly Vector2 texcoord = texcoord;
        [FieldOffset(0x20)] public readonly Vector3 rounding = rounding;
    }

    [StructLayout(LayoutKind.Explicit)]
    readonly struct CharVert(Vector2 pos, Vector4 col, Vector3 charData)
    {
        [FieldOffset(0)] public readonly Vector2 pos = pos;
        [FieldOffset(0x8)] public readonly Vector4 col = col;
        /// <summary>
        /// The uv coordinates into the font texture are stored in xy, and z stores the font weight.
        /// </summary>
        [FieldOffset(0x18)] public readonly Vector3 charData = charData;
    }
}
