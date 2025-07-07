using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// The base class for any <see cref="UIElement"/> which draws simple text. For more complex text drawing, see <seealso cref="TextBlockBase"/>.
/// </summary>
[UIClonable]
public abstract partial class TextBase : UIElement
{
    /// <summary>
    /// The text represented by this element.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Layout)]
    protected string? text;
    /// <summary>
    /// The font size of this element's text.
    /// </summary>
    [Reactive("FontSize"), Dirty(DirtyFlag.Layout), Stylable]
    protected float size;
    /// <summary>
    /// The text colour of this element's text.
    /// </summary>
    [Reactive("TextColour"), Dirty(DirtyFlag.Layout), Stylable]
    protected Vector4 textColour;
    /// <summary>
    /// The font used by this element's text.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Layout), Stylable]
    protected Font font;
    /// <summary>
    /// Specifies how the text in this element is horizontally aligned.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected TextAlignment textAlignment;

    // TODO: Finish implementing
    /// <summary>
    /// Adds or subtracts space between individual words, measured in pixels.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected float wordSpacing;
    /// <summary>
    /// Adds or subtracts space between individual characters, measured in pixels.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected float charSpacing;
    /// <summary>
    /// The horizontal scaling factor applied to all characters in the text. Values smaller than 1 
    /// will squeeze the text, and values greater than 1 will stretch it.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected float stretchX = 1;

    /// <summary>
    /// A horizontal skew to apply to the text, useful for creating faux-italic type. A value of 
    /// 0 represents no skew.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected float skew = 0;
    /// <summary>
    /// The weight of the font when using SDF-based fonts. This is not a true variable-weight 
    /// axis control, but rather an adjustment made to the rendered SDF font, it's useful for 
    /// faux-bold type. A value of 0.5 represents the font's native weight.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable]
    protected float weight = 0.5f;

    // Note that in the future this will store the shaped positions of each glyph to be rendered.
    /// <summary>
    /// This stores the bounds of the label as measured by the font engine. This is updated automatically 
    /// whenever the UI engine re-measures the element.
    /// </summary>
    protected Bounds2D measuredBounds;

    public TextBase()
    {
        text = string.Empty;
        size = 14;
        textColour = new(0, 0, 0, 1);
        font = Fonts.Default;
    }

    public TextBase(string? text) : this()
    {
        this.text = text;
    }

    protected internal override Vector2 Measure()
    {
        var res = Font.Measure(text, size, 1);
        measuredBounds = res;
        return res.Size;
    }

    protected override Bounds2D ComputeBounds(Bounds2D parent)
    {
        var bounds = base.ComputeBounds(parent);
        // Apply an adjustment to the bounds to correct the vertical centering.
        switch (VerticalAlignment)
        {
            case Alignment.Top:
                break;
            case Alignment.Bottom:
                break;
            case Alignment.Centre:
            case Alignment.Stretch:
                /*float emHeight;
                //if (font.CharsDict.TryGetValue('M', out var xChar))
                //    emHeight = xChar.size.Y;
                //else
                    emHeight = font.Size * 1.333f;
                emHeight *= 0.5f;
                emHeight = 0;
                float offset = (font.Base - emHeight) * (size / font.Size);
                bounds.topLeft.Y -= offset;
                bounds.bottomRight.Y -= offset;*/
                float offset = measuredBounds.topLeft.Y;
                bounds.topLeft.Y -= offset;
                bounds.bottomRight.Y -= offset;

                break;
        }

        return bounds;
    }

    /// <summary>
    /// Draws this text using the given drawing context.
    /// <para/>
    /// This method exists outside of <see cref="Draw(IDrawContext)"/> so that elements derived 
    /// from this class can call the text drawing method whenever they want within their own
    /// <see cref="Draw(IDrawContext)"/> method implementation.
    /// </summary>
    /// <param name="ctx"></param>
    protected void DrawText(IDrawContext ctx)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var fnt = font;
        var tex = fnt.FontTexture;
        if (tex == null)
            return;

        tex.ExecuteDrawCommands(ctx);
        if (!tex.IsLoaded)
        {
            // Can't render yet, the font texture isn't ready, try again next frame.
            Dirty(DirtyFlag.Content);
            return;
        }
        ctx.DrawText(RenderedBoundsAbsolute, text.AsSpan(), fnt, size, textColour, wordSpacing, charSpacing, skew, weight, stretchX);
    }

    protected internal override void Draw(IDrawContext ctx)
    {
        DrawText(ctx);
    }
}
