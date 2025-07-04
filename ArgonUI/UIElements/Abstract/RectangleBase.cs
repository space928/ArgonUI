using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// Represents the base class for all <see cref="UIElement"/>s which draw a rectangle background.
/// </summary>
[UIClonable]
public abstract partial class RectangleBase : UIElement, IRectangleProps
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

    /// <summary>
    /// Draws this rectangle using the given drawing context.
    /// <para/>
    /// This method exists outside of <see cref="Draw(IDrawContext)"/> so that elements derived 
    /// from this class can call the rectangle drawing method whenever they want within their own
    /// <see cref="Draw(IDrawContext)"/> method implementation.
    /// </summary>
    /// <param name="ctx"></param>
    protected void DrawRectangle(IDrawContext ctx)
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

    protected internal override void Draw(IDrawContext ctx)
    {
        DrawRectangle(ctx);
    }
}
