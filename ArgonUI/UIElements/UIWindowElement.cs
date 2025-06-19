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

public partial class UIWindowElement : UIContainer
{
    private readonly List<UIElement> children;
    private readonly ReadOnlyCollection<UIElement> childrenRO;
    /// <summary>
    /// The background colour of this window.
    /// </summary>
    [Reactive("BGColour"), Dirty(DirtyFlags.Content), Stylable]
    private Vector4 bgColour;

    public UIWindow Window { get; init; }
    public override IReadOnlyList<UIElement> Children => childrenRO;

    //public override int DesiredWidth => Window.Size.x;
    //public override int DesiredHeight => Window.Size.y;

    internal UIWindowElement(UIWindow window)
    {
        this.Window = window;
        children = [];
        childrenRO = new(children);
        Width = Window.Size.x;
        Height = Window.Size.y;
        Window.OnLoaded += () =>
        {
            Width = Window.Size.x;
            Height = Window.Size.y;
            Dirty(DirtyFlags.Content);
        };
        Window.OnResize += () =>
        {
            Width = Window.Size.x;
            Height = Window.Size.y;
            Dirty(DirtyFlags.Content);
        };
    }

    public override void Dirty(DirtyFlags flags)
    {
        base.Dirty(flags);

        if (flags != DirtyFlags.None)
        {
            Window.RequestRedraw();
        }
    }

    public override void AddChild(UIElement child)
    {
        children.Add(child);
        child.Parent = this;
        OnChildElementChanged(child, Styling.UIElementTreeChange.ElementAdded);
    }

    public override void AddChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
            AddChild(child);
    }

    public override void InsertChild(UIElement child, int index)
    {
        children.Insert(index, child);
        child.Parent = this;
        OnChildElementChanged(child, Styling.UIElementTreeChange.ElementAdded);
    }

    public override bool RemoveChild(UIElement child)
    {
        if (children.Remove(child))
        {
            child.Parent = null;
            OnChildElementChanged(child, Styling.UIElementTreeChange.ElementRemoved);
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
            child.Parent = null;
            OnChildElementChanged(child, Styling.UIElementTreeChange.ElementRemoved);
        }
    }

    protected internal override Bounds2D Layout()
    {
        var wndSize = Window.Size;
        var parentBounds = new Bounds2D(Vector2.Zero, new(wndSize.x, wndSize.y));
        var bounds = ComputeBounds(parentBounds);
        RenderedBoundsAbsolute = bounds;
        RenderedBounds = new(bounds.topLeft - parentBounds.topLeft, bounds.bottomRight - parentBounds.topLeft);
        RenderedWidth = bounds.Width;
        RenderedHeight = bounds.Height;
        return bounds;
    }

    protected internal override void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands)
    {
        commands.Clear();
        commands.Add(ctx => ctx.Clear(bgColour));
    }
}
