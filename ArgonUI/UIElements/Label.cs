using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using ArgonUI.UIElements.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class Label : TextBase
{
    public Label()
    {
        text = "Label";
        size = 14;
        textColour = new(0, 0, 0, 1);
        font = Fonts.Default;
    }

    public Label(string? text) : this()
    {
        this.text = text;
    }
}
