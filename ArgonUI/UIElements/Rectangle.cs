using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public class Rectangle : UIElement
{
    private Vector4 colour;
    private float rounding;

    /// <summary>
    /// The colour of this rectangle.
    /// </summary>
    public Vector4 Colour { get => colour; set => UpdateProperty(ref colour, value); }

    /// <summary>
    /// The radius of the corners of this rectangle.
    /// </summary>
    public float Rounding { get => rounding; set => UpdateProperty(ref rounding, value); }

    protected internal override void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands)
    {
        commands.Clear();
        commands.Add(ctx => ctx.DrawRect(bounds, Colour, Rounding));
    }
}
