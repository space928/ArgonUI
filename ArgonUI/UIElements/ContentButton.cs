using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using ArgonUI.UIElements.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class ContentButton : ElementPresenterBase
{
    /// <summary>
    /// The colour of this button.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] private Vector4 colour;
    /// <summary>
    /// The radius of the corners of this button.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Content), Stylable] private float rounding;

    protected internal override void Draw(IDrawContext ctx)
    {
        ctx.DrawRect(RenderedBoundsAbsolute, Colour, Rounding);
    }
}
