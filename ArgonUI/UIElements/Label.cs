using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public partial class Label : UIElement
{
    /// <summary>
    /// The text represented by this label.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)] protected string? text;
    /// <summary>
    /// The font size of this label.
    /// </summary>
    [Reactive("FontSize"), Dirty(DirtyFlags.Layout)] protected float size;
    /// <summary>
    /// The text colour of this label.
    /// </summary>
    [Reactive("TextColour"), Dirty(DirtyFlags.Layout)] protected Vector4 colour;

    protected BMFont? font;

    public override int DesiredWidth => (int)Font.Measure(text, size).X;
    public override int DesiredHeight => (int)Font.Measure(text, size).Y;

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
