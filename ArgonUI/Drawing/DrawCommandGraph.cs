using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.UIElements;

namespace ArgonUI.Drawing;

public class DrawCommandGraph(UIElement element)
{
    public UIElement UIElement { get; init; } = element;
    public Bounds2D Bounds { get; set; }
    public List<Action<IDrawContext>> DrawCommands { get; init; } = [];
    public Dictionary<UIElement, DrawCommandGraph> Children { get; init; } = [];
}
