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
    private int vertPos;
    private int vertCount;
    private uint[] vertBuff;

    public bool IsInitialised => window != null;

    public OpenGLDrawContext(GL gl)
    {
        this.gl = gl;
        vertBuff = new uint[4096];
    }

    public void InitRenderer(UIWindow window)
    {
        this.window = window;
        //try
        //{
        rectShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_ROUNDING", "#define SUPPORT_ALPHA"]);
        textShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_TEXT", "#define SUPPORT_ALPHA"]);
        textureShader = new(gl, "ui_vert.glsl", "ui_frag.glsl", ["#define SUPPORT_TEXTURE", "#define SUPPORT_ALPHA"]);

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

    }

    public void Clear(Vector4 colour)
    {
        gl.ClearColor(colour.X, colour.Y, colour.Z, colour.W);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    public void DrawGradient(Bounds2D bounds, Vector4 colourA, Vector4 colourB, Vector4 colourC, Vector4 colourD, float rounding)
    {
        throw new NotImplementedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    struct RectVert
    {
        [FieldOffset(0)] public Vector2 pos;
        [FieldOffset(0x8)] public Vector4 col;
        [FieldOffset(0x18)] public Vector2 texcoord;
        [FieldOffset(0x1A)] public Vector3 rounding;
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
        throw new NotImplementedException();
    }

    public void DrawText(Bounds2D bounds, float size, string c, BMFont font, Vector4 colour)
    {
        throw new NotImplementedException();
    }

    public void DrawTexture(Bounds2D bounds, ITextureHandle texture, float rounding)
    {
        throw new NotImplementedException();
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
            gl.DrawArrays(PrimitiveType.TriangleStrip, 0, (uint)vertCount);
            vao?.Unbind();
        }

        vertPos = 0;
        vertCount = 0;
    }

    public ITextureHandle LoadTexture(Stream data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        throw new NotImplementedException();
    }

    public ITextureHandle LoadTexture(ReadOnlyMemory<byte> data, string? name = null, TextureCompression compression = TextureCompression.Unknown)
    {
        throw new NotImplementedException();
    }
}
