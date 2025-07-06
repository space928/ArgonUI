using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// Represents the base class for all <see cref="UIContainer"/>s which draw a rectangle background.
/// <para/>
/// This should have feature parity with <see cref="RectangleBase"/>.
/// </summary>
[UIClonable]
public abstract partial class UIContainerRectangle : UIContainer, IRectangleProps
{
    /// <summary>
    /// The colour of this rectangle.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected Vector4 colour;
    /// <summary>
    /// The outline colour.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected Vector4 outlineColour;
    /// <summary>
    /// The thickness of the outline in pixels.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected float outlineThickness;
    /// <summary>
    /// The radius of the corners of this rectangle.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected float rounding;
    /// <summary>
    /// The texture to fill the rectangle with.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected ArgonTexture? texture;
    /// <summary>
    /// The gradient to fill the rectangle with.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] protected Gradient? gradientFill;


#if DEBUG_LATENCY
    public bool logLatencyNow;
#endif

    protected internal override void Draw(IDrawContext ctx)
    {
        if (texture != null)
        {
            texture.ExecuteDrawCommands(ctx);
            if (!texture.IsLoaded)
            {
                // Can't render yet, the font texture isn't ready, try again next frame.
                Dirty(DirtyFlag.Content);
                return;
            }
            ctx.DrawTexture(RenderedBoundsAbsolute, texture.TextureHandle!, Rounding);
        }
        else if (gradientFill != null)
        {
            ctx.DrawGradient(RenderedBoundsAbsolute, gradientFill.ColourTL, gradientFill.ColourTR, gradientFill.ColourBL, gradientFill.ColourBR, Rounding);
        }
        else if (Colour.W > 0)
        {
            ctx.DrawRect(RenderedBoundsAbsolute, Colour, Rounding);
        }

        if (outlineThickness > 0 && outlineColour.W > 0)
            ctx.DrawOutlineRect(RenderedBoundsAbsolute, outlineColour, outlineThickness, rounding);
#if DEBUG_LATENCY
        if (logLatencyNow)
            commands.Add(ctx => ctx.MarkLatencyTimerEnd($"{Colour.Y}"));
        logLatencyNow = false;
#endif
    }
}
