using ArgonUI.Drawing;
using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[DebuggerDisplay("{GetType().Name,nq} ({Window.Title} - {name}, {Children.Count} children)")]
public partial class UIWindowElement : UIContainer
{
    private readonly List<UIElement> children;
    private readonly ReadOnlyCollection<UIElement> childrenRO;
    /// <summary>
    /// The background colour of this window.
    /// </summary>
    [Reactive("BGColour"), Dirty(DirtyFlags.Content), Stylable]
    private Vector4 bgColour;

    //public new UIWindow Window { get; init; }
    public override IReadOnlyList<UIElement> Children => childrenRO;

    //public override int DesiredWidth => Window.Size.x;
    //public override int DesiredHeight => Window.Size.y;

    internal UIWindowElement(UIWindow window)
    {
        this.window = window;
        children = [];
        childrenRO = new(children);
        treeDepth = 0;
        Width = window.Size.x;
        Height = window.Size.y;
        window.OnLoaded += () =>
        {
            Width = this.window!.Size.x;
            Height = this.window!.Size.y;
            Dirty(DirtyFlags.Content);
        };
        window.OnResize += () =>
        {
            Width = this.window!.Size.x;
            Height = this.window!.Size.y;
            Dirty(DirtyFlags.Content);
        };
    }

    public override void Dirty(DirtyFlags flags)
    {
        base.Dirty(flags);

        if (flags != DirtyFlags.None)
        {
            Window!.RequestRedraw();
        }
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

    protected internal override Bounds2D Layout()
    {
        var wndSize = window!.Size;
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

    /// <summary>
    /// Note that this method is not supported on this type. Please use <see cref="Clone(UIElement)"/> instead.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override UIElement Clone() => throw new InvalidOperationException("Can't create clone of UIWindowElement, instances of UIWindowElement are only created and managed by UIWindows.");

    public override UIElement Clone(UIElement target)
    {
        base.Clone(target);
        if (target is UIWindowElement t)
        {
            t.bgColour = bgColour;
        }
        return target;
    }
}
