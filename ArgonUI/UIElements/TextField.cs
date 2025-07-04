using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using ArgonUI.UIElements.Abstract;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class TextField : TextBlockBase
{
    protected VectorInt2 textSelection;

    public TextField()
    {
        text = string.Empty;
        size = 14;
        textColour = new(0, 0, 0, 1);
        font = Fonts.Default;

        //OnKeyDown
    }

    public TextField(string? text) : this()
    {
        this.text = text;
    }

    /// <summary>
    /// Gets the span of characters in this text field which are currently selected.
    /// Returns an empty span if the text is <see langword="null"/>, empty, or no
    /// text is selected.
    /// </summary>
    public ReadOnlySpan<char> SelectedText
    {
        get
        {
            if (string.IsNullOrEmpty(text))
                return default;
            string t = text!;
            int len = t.Length;
            return text.AsSpan(Math.Min(textSelection.x, len), Math.Min(textSelection.y, len));
        }
    }

    protected internal override void Draw(IDrawContext ctx)
    {
        // Draw rectangle
        DrawRectangle(ctx);

        // Draw selection

        // Draw text
        DrawText(ctx);

        // Draw caret

    }
}
