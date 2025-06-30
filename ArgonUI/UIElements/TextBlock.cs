using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

/// <summary>
/// Similar to a <see cref="Label"/> but with additional features such as text wrapping.
/// </summary>
[UIClonable]
public partial class TextBlock : Label
{
    /// <summary>
    /// Specifies how the text in this text block is horizontally aligned.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected TextAlignment alignment;
    /// <summary>
    /// Justifies the contents of this text block to fill it's width.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected bool justify;
    //protected bool justifyLastLine;

    // TODO: Finish implementing
    /// <summary>
    /// 
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float wordSpacing;
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float charSpacing;
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float stretchX;

    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float lineSpacing;
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float firstLineIndent;
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float indent;

    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float skew;
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    protected float weight;

    protected internal override void Draw(IDrawContext ctx)
    {
        if (text == null)
            return;

        var fnt = font ?? Fonts.Default;
        fnt.FontTexture?.ExecuteDrawCommands(ctx);
        ctx.DrawText(RenderedBoundsAbsolute, size, text, fnt, colour, wordSpacing, charSpacing, skew, weight);
    }
}

public enum TextAlignment
{
    Left,
    Centre,
    Right
}
