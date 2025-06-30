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
public partial class Slider : UIElement
{
    /// <summary>
    /// The colour of this slider.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    private Vector4 colour;
    /// <summary>
    /// The rounding radius of the slider's handle.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content), Stylable]
    private float handleRounding;
    /// <summary>
    /// The radius of the handle.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private Vector2 handleSize;
    /// <summary>
    /// The thickness of the slider track.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private float trackThickness;
    /// <summary>
    /// Whether the slider should slide horizontally, or vertically.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private bool vertical;
    /// <summary>
    /// The current value of the slider.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content)]
    private float value;
    /// <summary>
    /// The minimum value the slider can represent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content)] 
    private float min;
    /// <summary>
    /// The maximum value the slider can represent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content)] 
    private float max;
    /// <summary>
    /// The size of steps between values of the slider.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content)] 
    private float step;
    /// <summary>
    /// An exponent to raise the value of the slider to, useful for creating non-linear sliders.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Content)] 
    private float power;

    protected internal override void Draw(IDrawContext ctx)
    {
        throw new NotImplementedException();
    }
}
