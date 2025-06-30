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
public partial class Rectangle : UIElement
{
    /// <summary>
    /// The colour of this rectangle.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable] private Vector4 colour;
    /// <summary>
    /// The radius of the corners of this rectangle.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable] private float rounding;

#if DEBUG_LATENCY
    public bool logLatencyNow;
#endif

    protected internal override void Draw(IDrawContext ctx)
    {
        if (Colour.W > 0)
            ctx.DrawRect(RenderedBoundsAbsolute, Colour, Rounding);
#if DEBUG_LATENCY
        if (logLatencyNow)
            commands.Add(ctx => ctx.MarkLatencyTimerEnd($"{Colour.Y}"));
        logLatencyNow = false;
#endif
    }
}
