using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class Label : UIElement
{
    /// <summary>
    /// The text represented by this label.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)] protected string? text;
    /// <summary>
    /// The font size of this label.
    /// </summary>
    [Reactive("FontSize"), Dirty(DirtyFlags.Layout), Stylable] protected float size;
    /// <summary>
    /// The text colour of this label.
    /// </summary>
    [Reactive("TextColour"), Dirty(DirtyFlags.Layout), Stylable] protected Vector4 colour;

    //[UICloneableField]
    /// <summary>
    /// The font used by this label.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    protected BMFont font;

    /// <summary>
    /// This stores the bounds of the label as measured by the font engine. This is updated automatically 
    /// whenever the UI engine re-measures the element.
    /// </summary>
    protected Bounds2D measuredBounds;

    public Label()
    {
        text = "Label";
        size = 14;
        colour = new(0, 0, 0, 1);
        font = Fonts.Default;
    }

    public Label(string? text) : this()
    {
        this.text = text;
    }

    protected internal override VectorInt2 Measure()
    {
        var res = Font.Measure(text, size, 1);
        measuredBounds = res;
        return new(res.Size);
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

    protected internal override void Draw(IDrawContext ctx)
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
            Dirty(DirtyFlags.Content);
            return;
        }
        ctx.DrawText(RenderedBoundsAbsolute, size, text!, fnt, colour);
    }
}
