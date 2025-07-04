using ArgonUI.Helpers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace ArgonUI.Backends.OpenGL;

internal class ShaderManager : IDisposable
{
    private readonly Dictionary<ShaderFeature, Shader> shaders = [];
    private readonly Dictionary<ShaderVertexLayout, VertexArrayObject<uint, uint>> vaos = [];
    private readonly FrozenDictionary<ShaderFeature, string> shaderFeatureDefines;

    public ShaderFeature ActiveFeatures { get; private set; }
    public Shader? ActiveShader { get; private set; }
    public VertexArrayObject<uint, uint>? ActiveVAO { get; private set; }

    public readonly ShaderFeature[] PRECOMPILED_SHADERS = [
        ShaderFeature.Text | ShaderFeature.Alpha, 
        ShaderFeature.Rounding | ShaderFeature.Alpha, 
        ShaderFeature.Outline | ShaderFeature.Rounding | ShaderFeature.Alpha,
        ShaderFeature.Texture | ShaderFeature.Rounding | ShaderFeature.Alpha,
        ShaderFeature.Blur | ShaderFeature.Rounding | ShaderFeature.Alpha,
    ];

    public ShaderManager()
    {
        Dictionary<ShaderFeature, string> shaderFeatureStrings = [];
        shaderFeatureStrings.Add(ShaderFeature.Text, "#define SUPPORT_TEXT");
        shaderFeatureStrings.Add(ShaderFeature.Texture, "#define SUPPORT_TEXTURE");
        shaderFeatureStrings.Add(ShaderFeature.Outline, "#define SUPPORT_OUTLINE");
        shaderFeatureStrings.Add(ShaderFeature.TextShadow, "#define SUPPORT_TEXT_SHADOW");
        shaderFeatureStrings.Add(ShaderFeature.Alpha, "#define SUPPORT_ALPHA");
        shaderFeatureStrings.Add(ShaderFeature.Rounding, "#define SUPPORT_ROUNDING");
        shaderFeatureStrings.Add(ShaderFeature.Blur, "#define SUPPORT_BLUR");
        shaderFeatureDefines = shaderFeatureStrings.ToFrozenDictionary();
    }

    public void Init(GL gl)
    {
        foreach (var shader in shaders.Values)
            shader.Dispose();
        foreach (var vao in vaos.Values)
            vao.Dispose();
        shaders.Clear();
        vaos.Clear();

        foreach (var precompile in PRECOMPILED_SHADERS)
            SetActiveShader(gl, precompile);
    }

    public bool IsShaderActive(ShaderFeature features) => features == ActiveFeatures;

    /// <summary>
    /// Sets the active shader to one which supports the given features. Compiles and caches a new shader if needed.
    /// <para/>
    /// Also updates the active VAO to match.
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="features">The required shader features.</param>
    /// <returns><see langword="true"/> if the shader was changed.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool SetActiveShader(GL gl, ShaderFeature features)
    {
        if (features == ActiveFeatures)
            return false;

        VertexArrayObject<uint, uint>? vao;
        var vertLayout = GetVertexLayout(features);
        if (shaders.TryGetValue(features, out var shader))
        {
            vao = vaos[vertLayout];
            goto ReturnActiveShader;
        }

        shader = InitShader(gl, features);

        if (!vaos.TryGetValue(vertLayout, out vao))
        {
            vao = vertLayout switch
            {
                ShaderVertexLayout.Rect => InitRectVAO(gl),
                ShaderVertexLayout.RectRound => InitRectRoundVAO(gl),
                ShaderVertexLayout.Char => InitTextVAO(gl),
                _ => throw new NotImplementedException(),
            };
            vaos.Add(vertLayout, vao);
        }

        shaders.Add(features, shader);

    ReturnActiveShader:
        shader.Use();
        ActiveFeatures = features;
        ActiveShader = shader;
        ActiveVAO = vao;

        if ((features & ShaderFeature.Text) != 0)
            shader?.SetUniform("uFontTex", 0);
        if ((features & ShaderFeature.Texture) != 0)
            shader?.SetUniform("uMainTex", 0);

        return true;
    }

    private static ShaderVertexLayout GetVertexLayout(ShaderFeature features)
    {
        // TODO: This is likely to cause bugs, many features share vertex layouts
        if ((features & ShaderFeature.Text) != 0)
            return ShaderVertexLayout.Char;
        if ((features & ShaderFeature.Rounding) != 0)
            return ShaderVertexLayout.RectRound;
        return ShaderVertexLayout.Rect;
    }

    private Shader InitShader(GL gl, ShaderFeature features)
    {
        // Build a list of defines to add to the shader
        using TemporaryList<string> featuresList = [];
        for (int i = 1; i < (int)ShaderFeature.MAX_VALUE; i <<= 1)
            if (((int)features & i) != 0)
                featuresList.Add(shaderFeatureDefines[(ShaderFeature)i]);

        return new(gl, "ui_vert.glsl", "ui_frag.glsl", featuresList);
    }

    private static VertexArrayObject<uint, uint> InitRectVAO(GL gl)
    {
        VertexArrayObject<uint, uint> vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        vao.Bind();
        vao.VertexAttributePointer(0, 2, VertexAttribType.Float, 8, 0);
        vao.VertexAttributePointer(1, 4, VertexAttribType.Float, 8, 2);
        vao.VertexAttributePointer(3, 2, VertexAttribType.Float, 8, 6);
        vao.Unbind();
        return vao;
    }

    private static VertexArrayObject<uint, uint> InitRectRoundVAO(GL gl)
    {
        VertexArrayObject<uint, uint> vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        vao.Bind();
        vao.VertexAttributePointer(0, 2, VertexAttribType.Float, 12, 0);
        vao.VertexAttributePointer(1, 4, VertexAttribType.Float, 12, 2);
        vao.VertexAttributePointer(3, 2, VertexAttribType.Float, 12, 6);
        vao.VertexAttributePointer(4, 4, VertexAttribType.Float, 12, 8);
        vao.Unbind();
        return vao;
    }

    private static VertexArrayObject<uint, uint> InitTextVAO(GL gl)
    {
        VertexArrayObject<uint, uint> vao = new(gl, new BufferObject<uint>(gl, [], BufferTargetARB.ArrayBuffer), null);
        vao.Bind();
        vao.VertexAttributePointer(0, 2, VertexAttribType.Float, 9, 0);
        vao.VertexAttributePointer(1, 4, VertexAttribType.Float, 9, 2);
        vao.VertexAttributePointer(2, 3, VertexAttribType.Float, 9, 6);
        vao.Unbind();
        return vao;
    }

    public void Dispose()
    {
        foreach (var shader in shaders.Values)
            shader.Dispose();
        foreach (var vao in vaos.Values)
            vao.Dispose();
        shaders.Clear();
        vaos.Clear();
    }
}

[Flags]
internal enum ShaderFeature : int
{
    None = 0,
    Text = 1 << 0,
    Texture = 1 << 1,
    Outline = 1 << 2,
    TextShadow = 1 << 3,
    Alpha = 1 << 4,
    Rounding = 1 << 5,
    Blur = 1 << 6,

    /// <summary>
    /// Used internally to count the number of possible features, do not move.
    /// </summary>
    MAX_VALUE
}

internal enum ShaderVertexLayout
{
    Rect,
    RectRound,
    Char
}

[StructLayout(LayoutKind.Explicit)]
readonly struct RectVert(Vector2 pos, Vector4 col, Vector2 texcoord)
{
    [FieldOffset(0)] public readonly Vector2 pos = pos;
    [FieldOffset(0x8)] public readonly Vector4 col = col;
    [FieldOffset(0x18)] public readonly Vector2 texcoord = texcoord;
}

[StructLayout(LayoutKind.Explicit)]
readonly struct RectRoundVert(Vector2 pos, Vector4 col, Vector2 texcoord, Vector4 rounding)
{
    [FieldOffset(0)] public readonly Vector2 pos = pos;
    [FieldOffset(0x8)] public readonly Vector4 col = col;
    [FieldOffset(0x18)] public readonly Vector2 texcoord = texcoord;
    /// <summary>
    /// (XY: The size of the rectangle in pixels, Z: The rounding radius in pixels, W: The blur radius in pixels)
    /// </summary>
    [FieldOffset(0x20)] public readonly Vector4 rounding = rounding;
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
