using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public class ContentButton : ElementPresenterBase
{
    private Vector4 colour;
    private float rounding;

    /// <summary>
    /// The colour of this button.
    /// </summary>
    public Vector4 Colour
    {
        get => colour; set
        {
            UpdateProperty(ref colour, value);
            Dirty(DirtyFlags.Content);
        }
    }

    /// <summary>
    /// The radius of the corners of this button.
    /// </summary>
    public float Rounding
    {
        get => rounding; set
        {
            UpdateProperty(ref rounding, value);
            Dirty(DirtyFlags.Content);
        }
    }

    protected internal override void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands)
    {
        commands.Clear();
        commands.Add(ctx => ctx.DrawRect(bounds, Colour, Rounding));
    }
}
