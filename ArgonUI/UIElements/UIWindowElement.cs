using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public class UIWindowElement : UIElement, IContainer
{
    private readonly List<UIElement> children;
    private Vector4 colour;

    public UIWindow Window { get; init; }
    public ReadOnlyCollection<UIElement> Children => new(children);

    public Vector4 InnerPadding { get; set; }
    public bool ClipContents { get; set; }
    //public override int DesiredWidth => Window.Size.x;
    //public override int DesiredHeight => Window.Size.y;

    /// <summary>
    /// The background colour of this window.
    /// </summary>
    public Vector4 BGColour
    {
        get => colour; set
        {
            UpdateProperty(ref colour, value);
            Dirty(DirtyFlags.Content);
        }
    }

    internal UIWindowElement(UIWindow window)
    {
        this.Window = window;
        children = [];
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

    public void AddChild(UIElement child)
    {
        children.Add(child);
        child.Parent = this;
    }

    public void AddChildren(UIElement[] children)
    {
        this.children.AddRange(children);
        foreach (var child in children)
            child.Parent = this;
    }

    public void InsertChild(UIElement child, int index)
    {
        children.Insert(index, child);
        child.Parent = this;
    }

    public void RemoveChild(UIElement child)
    {
        children.Remove(child);
        child.Parent = null;
    }

    public void RemoveChildren(UIElement[] children)
    {
        foreach (UIElement child in children)
            RemoveChild(child);
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
        commands.Add(ctx => ctx.Clear(colour));
    }
}
