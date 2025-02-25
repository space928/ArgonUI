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

    public override int DesiredWidth => (int)Font.Measure(text, size).X;
    public override int DesiredHeight => (int)Font.Measure(text, size).Y;

    /// <summary>
    /// The text represented by this label.
    /// </summary>
    public string? Text
    {
        get => text; set
        {
            UpdateProperty(ref text, value);
            Dirty(DirtyFlags.Content | DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// The font used by this label.
    /// </summary>
    public BMFont Font
    {
        get => font ?? Fonts.Default; set
        {
            UpdateProperty(ref font, value);
            Dirty(DirtyFlags.Content | DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// The font size of this label.
    /// </summary>
    public float FontSize
    {
        get => size; set
        {
            UpdateProperty(ref size, value);
            Dirty(DirtyFlags.Content | DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// The text colour of this label.
    /// </summary>
    public Vector4 TextColour
    {
        get => colour; set
        {
            UpdateProperty(ref colour, value);
            Dirty(DirtyFlags.Content | DirtyFlags.Layout);
        }
    }

    public Label()
    {
        text = "Label";
        size = 14;
        font = null;
        colour = new(0, 0, 0, 1);
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
            var fnt = font ?? Fonts.Default;
            fnt.FontTexture?.ExecuteDrawCommands(ctx);
            ctx.DrawText(bounds, size, text, fnt, colour);
        });
    }
}
