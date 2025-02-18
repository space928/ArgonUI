using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public class Button : ContentButton
{
    private readonly Label label;

    /// <summary>
    /// The label for this button.
    /// </summary>
    public string? Text
    {
        get => label.Text; set => label.Text = value;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Hides an unneeded member from the public API.")]
    private new UIElement Content => label;

    public Button()
    {
        label = new Label();
        base.Content = label;
    }
}
