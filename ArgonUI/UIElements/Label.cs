using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public class Label : UIElement
{
    private string? text;
    private float size;
    private BMFont? font;
    private Vector4 colour;

    /// <summary>
    /// The label for this button.
    /// </summary>
    public string? Text
    {
        get => text; set
        {
            UpdateProperty(ref text, value);
            Dirty(DirtyFlags.Content);
        }
    }

    public Label()
    {
        text = "Label";
        size = 14;
        font = null;
        colour = Vector4.One;
    }

    public Label(string? text) : this()
    {
        this.text = text;
    }

    protected internal override void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands)
    {
        commands.Clear();
        if (text == null)
            return;
        commands.Add(ctx =>
        {
            ctx.DrawText(bounds, size, text, font ?? Fonts.Default, colour);
        });
    }
}
