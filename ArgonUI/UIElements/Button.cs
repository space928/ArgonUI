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
    private Label label;

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
        label.HorizontalAlignment = label.VerticalAlignment = Alignment.Centre;
        base.Content = label;
    }

    public override UIElement Clone() => Clone(new Button());

    public override UIElement Clone(UIElement target)
    {
        base.Clone(target);
        if (target is Button t)
        {
            t.label = (Label)label.Clone();
            ((ContentButton)t).Content = t.label;
        }
        return target;
    }
}
