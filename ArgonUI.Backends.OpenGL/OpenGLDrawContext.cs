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

    private VertexArrayObject<uint, uint>? vao;
    private Shader? activeShader;
    private Texture2D? activeTexture;
    private int vertPos;
    private int vertCount;
    private readonly uint[] vertBuff;

    public bool IsInitialised => window != null;

    public OpenGLDrawContext(GL gl)
    {
        this.gl = gl;
        vertBuff = new uint[4096];
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

        vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        /*}
        catch (Exception ex)
        {
            onErrorMessageCB("Error while initialising the renderer!", ex.ToString());
        }*/
    }

    public void Dispose()
    {
        vao?.Dispose();
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

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        if (activeShader != rectShader || vertBuff.Length - vertPos < Unsafe.SizeOf<RectVert>() * 4)
            FlushBatch();

        if (activeShader != rectShader)
        {
            activeShader = rectShader;
            vao?.Bind();
            vao?.VertexAttributePointer(0, 2, VertexAttribType.Float, 11, 0);
            vao?.VertexAttributePointer(1, 4, VertexAttribType.Float, 11, 2);
            vao?.VertexAttributePointer(3, 2, VertexAttribType.Float, 11, 6);
            vao?.VertexAttributePointer(4, 3, VertexAttribType.Float, 11, 8);
            vao?.Unbind();
        }
        Vector2 rectSize = new(bounds.Width, bounds.Height);

        DrawRectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), colourB, new(0, 1));
        DrawRectVert(bounds.topLeft, colourA, new(0, 0));
        DrawRectVert(bounds.bottomRight, colourD, new(1, 1));
        DrawRectVert(bounds.bottomRight, colourD, new(1, 1));
        DrawRectVert(bounds.topLeft, colourA, new(0, 0));
        DrawRectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), colourC, new(1, 0));

        void DrawRectVert(in Vector2 pos, in Vector4 colour, in Vector2 texcoord)
        {
            var bytes = MemoryMarshal.AsBytes<uint>(vertBuff);
            Unsafe.WriteUnaligned(ref bytes[vertPos], pos);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], colour);
            vertPos += Unsafe.SizeOf<Vector4>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], texcoord);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], new Vector3(rectSize.X, rectSize.Y, rounding));
            vertPos += Unsafe.SizeOf<Vector3>();
            vertCount++;
        }
    }

    public void DrawRect(Bounds2D bounds, Vector4 colour, float rounding)
    {
        if (activeShader != rectShader || vertBuff.Length - vertPos < Unsafe.SizeOf<RectVert>() * 4)
            FlushBatch();

        if (activeShader != rectShader)
        {
            activeShader = rectShader;
            vao?.Bind();
            vao?.VertexAttributePointer(0, 2, VertexAttribType.Float, 11, 0);
            vao?.VertexAttributePointer(1, 4, VertexAttribType.Float, 11, 2);
            vao?.VertexAttributePointer(3, 2, VertexAttribType.Float, 11, 6);
            vao?.VertexAttributePointer(4, 3, VertexAttribType.Float, 11, 8);
            vao?.Unbind();
        }
        Vector2 rectSize = new(bounds.Width, bounds.Height);

        DrawRectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), new(0, 1));
        DrawRectVert(bounds.topLeft, new(0, 0));
        DrawRectVert(bounds.bottomRight, new(1, 1));
        DrawRectVert(bounds.bottomRight, new(1, 1));
        DrawRectVert(bounds.topLeft, new(0, 0));
        DrawRectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), new(1, 0));

        void DrawRectVert(Vector2 pos, Vector2 texcoord)
        {
            var bytes = MemoryMarshal.AsBytes<uint>(vertBuff);
            Unsafe.WriteUnaligned(ref bytes[vertPos], pos);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], colour);
            vertPos += Unsafe.SizeOf<Vector4>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], texcoord);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], new Vector3(rectSize.X, rectSize.Y, rounding));
            vertPos += Unsafe.SizeOf<Vector3>();
            vertCount++;
        }
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
        if (vertBuff.Length - vertPos < Unsafe.SizeOf<CharVert>() * 4)
            FlushBatch();

        if (texture is not Texture2D tex)
            throw new NotSupportedException("Attempted to call DrawTexture with an incompatible texture object!");

        if (tex.Width == 0 || tex.Height == 0)
            return 0f;

        if (activeShader != textShader)
        {
            FlushBatch();
            activeShader = textShader;
            textShader?.Use();
            textShader?.SetUniform("uFontTex", 0);
            vao?.Bind();
            vao?.VertexAttributePointer(0, 2, VertexAttribType.Float, 9, 0);
            vao?.VertexAttributePointer(1, 4, VertexAttribType.Float, 9, 2);
            vao?.VertexAttributePointer(2, 3, VertexAttribType.Float, 9, 6);
            vao?.Unbind();
        }

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
            //gl.ActiveTexture(TextureUnit.Texture0);
            tex.Bind(0);
        }

        float size_x = size * width;
        float x0 = pos.X + c.xOffset * size_x;
        float x1 = pos.X + (c.xOffset + c.width) * size_x;
        float y0 = pos.Y + c.yOffset * size;
        float y1 = pos.Y + (c.yOffset + c.height) * size;
        Vector2 texScale = new(1f / activeTexture.Width, 1f / activeTexture.Height);
        float uv_x0 = c.x * texScale.X;
        float uv_x1 = (c.x + c.width) * texScale.X;
        float uv_y0 = c.y * texScale.X;
        float uv_y1 = (c.y + c.height) * texScale.X;

        DrawRectVert(new(x0, y1), colour, new(uv_x0, uv_y1, weight));
        DrawRectVert(new(x0, y0), colour, new(uv_x0, uv_y0, weight));
        DrawRectVert(new(x1, y1), colour, new(uv_x1, uv_y1, weight));
        DrawRectVert(new(x1, y1), colour, new(uv_x1, uv_y1, weight));
        DrawRectVert(new(x0, y0), colour, new(uv_x0, uv_y0, weight));
        DrawRectVert(new(x1, y0), colour, new(uv_x1, uv_y0, weight));

        return c.xAdvance * size_x;

        void DrawRectVert(in Vector2 pos, in Vector4 colour, in Vector3 chData)
        {
            var bytes = MemoryMarshal.AsBytes<uint>(vertBuff);
            Unsafe.WriteUnaligned(ref bytes[vertPos], pos);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], colour);
            vertPos += Unsafe.SizeOf<Vector4>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], chData);
            vertPos += Unsafe.SizeOf<Vector3>();
            vertCount++;
        }
    }

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        if (vertBuff.Length - vertPos < Unsafe.SizeOf<RectVert>() * 4)
            FlushBatch();

        if (texture is not Texture2D tex)
            throw new NotSupportedException("Attempted to call DrawTexture with an incompatible texture object!");

        if (activeShader != textureShader)
        {
            FlushBatch();
            activeShader = textureShader;
            textureShader?.Use();
            textureShader?.SetUniform("uMainTex", 0);
            vao?.Bind();
            vao?.VertexAttributePointer(0, 2, VertexAttribType.Float, 11, 0);
            vao?.VertexAttributePointer(1, 4, VertexAttribType.Float, 11, 2);
            vao?.VertexAttributePointer(3, 2, VertexAttribType.Float, 11, 6);
            vao?.VertexAttributePointer(4, 3, VertexAttribType.Float, 11, 8);
            vao?.Unbind();
        }

        if (activeTexture != tex)
        {
            FlushBatch();
            activeTexture = tex;
            tex.Bind();
        }

        Vector2 rectSize = new(bounds.Width, bounds.Height);

        DrawRectVert(new(bounds.topLeft.X, bounds.bottomRight.Y), new(0, 1));
        DrawRectVert(bounds.topLeft, new(0, 0));
        DrawRectVert(bounds.bottomRight, new(1, 1));
        DrawRectVert(bounds.bottomRight, new(1, 1));
        DrawRectVert(bounds.topLeft, new(0, 0));
        DrawRectVert(new(bounds.bottomRight.X, bounds.topLeft.Y), new(1, 0));

        void DrawRectVert(Vector2 pos, Vector2 texcoord)
        {
            var bytes = MemoryMarshal.AsBytes<uint>(vertBuff);
            Unsafe.WriteUnaligned(ref bytes[vertPos], pos);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], Vector4.One);
            vertPos += Unsafe.SizeOf<Vector4>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], texcoord);
            vertPos += Unsafe.SizeOf<Vector2>();
            Unsafe.WriteUnaligned(ref bytes[vertPos], new Vector3(rectSize.X, rectSize.Y, rounding));
            vertPos += Unsafe.SizeOf<Vector3>();
            vertCount++;
        }
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

            vao?.VBO.Update(vertBuff.AsSpan(0, vertPos / Unsafe.SizeOf<uint>()));

            vao?.Bind();
            gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)vertCount);
            vao?.Unbind();
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
    struct RectVert
    {
        [FieldOffset(0)] public Vector2 pos;
        [FieldOffset(0x8)] public Vector4 col;
        [FieldOffset(0x18)] public Vector2 texcoord;
        [FieldOffset(0x1A)] public Vector3 rounding;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct CharVert
    {
        [FieldOffset(0)] public Vector2 pos;
        [FieldOffset(0x8)] public Vector4 col;
        /// <summary>
        /// The uv coordinates into the font texture are stored in xy, and z stores the font weight.
        /// </summary>
        [FieldOffset(0x18)] public Vector3 charData;
    }
}
