using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using ArgonUI.UIElements.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// A very basic type of <see cref="UIContainer"/> which draws each of it's children in order, on top of each other.
/// </summary>
[UIClonable]
public partial class Panel : UIContainerRectangle
{
    private readonly List<UIElement> children;
    private readonly ReadOnlyCollection<UIElement> childrenRO;

    public override IReadOnlyList<UIElement> Children => childrenRO;

    public Panel() : base()
    {
        children = [];
        childrenRO = new(children);
        HorizontalAlignment = Alignment.Stretch;
        VerticalAlignment = Alignment.Stretch;
    }

    public override void AddChild(UIElement child)
    {
        children.Add(child);
        RegisterChild(child);
    }

    public override void AddChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
            AddChild(child);
    }

    public override void InsertChild(UIElement child, int index)
    {
        children.Insert(index, child);
        RegisterChild(child);
    }

    public override bool RemoveChild(UIElement child)
    {
        if (children.Remove(child))
        {
            UnregisterChild(child);
            return true;
        }
        return false;
    }

    public override void RemoveChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
            RemoveChild(child);
    }

    public override void ClearChildren()
    {
        for (int i = children.Count - 1; i >= 0; i--)
        {
            var child = children[i];
            children.RemoveAt(i);
            UnregisterChild(child);
        }
    }

    protected internal override Vector2 Measure()
    {
        if (children.Count == 0)
            return base.Measure();

        var res = Vector2.Zero;
        foreach (var child in children)
            res = Vector2.Max(res, child.desiredSize);
        res += InnerPadding.Size;

        return res;
    }
}
