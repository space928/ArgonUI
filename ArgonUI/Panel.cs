using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

public class Panel : UIElement, IContainer
{
    public ReadOnlyCollection<UIElement> Children => throw new NotImplementedException();

    public Vector4 InnerPadding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool ClipContents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void AddChild(UIElement child)
    {
        throw new NotImplementedException();
    }

    public void AddChildren(UIElement[] children)
    {
        throw new NotImplementedException();
    }

    public void InsertChild(UIElement child, int index)
    {
        throw new NotImplementedException();
    }

    public void RemoveChild(UIElement child)
    {
        throw new NotImplementedException();
    }

    public void RemoveChildren(UIElement[] children)
    {
        throw new NotImplementedException();
    }

    protected override Bounds2D ComputeBounds(Bounds2D parent)
    {
        throw new NotImplementedException();
    }

    protected override void Draw(Bounds2D bounds, IDrawContext context)
    {
        throw new NotImplementedException();
    }
}
