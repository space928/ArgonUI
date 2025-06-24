using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class Panel : UIContainer
{
    public override IReadOnlyList<UIElement> Children => throw new NotImplementedException();

    public override void AddChild(UIElement child)
    {
        throw new NotImplementedException();
    }

    public override void AddChildren(IEnumerable<UIElement> children)
    {
        throw new NotImplementedException();
    }

    public override void InsertChild(UIElement child, int index)
    {
        throw new NotImplementedException();
    }

    public override bool RemoveChild(UIElement child)
    {
        throw new NotImplementedException();
    }

    public override void RemoveChildren(IEnumerable<UIElement> children)
    {
        throw new NotImplementedException();
    }

    public override void ClearChildren()
    {
        throw new NotImplementedException();
    }

    protected override Bounds2D ComputeBounds(Bounds2D parent)
    {
        throw new NotImplementedException();
    }

    protected internal override void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands)
    {
        throw new NotImplementedException();
    }
}
