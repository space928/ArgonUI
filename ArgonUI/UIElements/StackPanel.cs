using ArgonUI.SourceGenerator;
using ArgonUI.UIElements.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class StackPanel : Panel
{
    [Reactive, Dirty(DirtyFlag.Layout)]
    private Direction direction;

    public StackPanel()
    {
        direction = Direction.Vertical;
        VerticalAlignment = Alignment.Stretch;
        HorizontalAlignment = Alignment.Left;
    }

    public StackPanel(Direction direction)
    {
        this.direction = direction;
        if (direction == Direction.Vertical)
        {
            VerticalAlignment = Alignment.Stretch;
            HorizontalAlignment = Alignment.Left;
        }
        else
        {
            VerticalAlignment = Alignment.Top;
            HorizontalAlignment = Alignment.Stretch;
        }
    }

    protected internal override void BeforeLayoutChildren()
    {
        Dirty(DirtyFlag.Layout);
    }

    protected internal override Vector2 Measure()
    {
        var children = Children;
        if (children.Count == 0)
            return base.Measure();

        var res = Vector2.Zero;
        Vector2 pad = InnerPadding.Size;
        if (direction == Direction.Vertical)
        {
            foreach (var child in children)
            {
                res.X = Math.Max(res.X, child.desiredSize.X);
                res.Y += child.desiredSize.Y + pad.Y;
            }
            res.X += pad.X;
        } 
        else
        {
            foreach (var child in children)
            {
                res.X += child.desiredSize.X + pad.X;
                res.Y = Math.Max(res.Y, child.desiredSize.Y);
            }
            res.Y += pad.Y;
        }

        return res;
    }

    /*protected internal override void LayoutChildren()
    {
        
    }*/

    protected internal override Bounds2D RequestChildBounds(UIElement element, int index)
    {
        if (index == 0)
            return RenderedBoundsAbsolute.SubtractMargin(InnerPadding).WithSizeNonZero(element.desiredSize);

        if (direction == Direction.Vertical)
        {
            var bounds = RenderedBoundsAbsolute;
            bounds.topLeft.Y = Children[index - 1].RenderedBoundsAbsolute.bottomRight.Y;
            return bounds.SubtractMargin(InnerPadding).WithSizeNonZero(element.desiredSize);
        }
        else
        {
            var bounds = RenderedBoundsAbsolute;
            bounds.topLeft.X = Children[index - 1].RenderedBoundsAbsolute.bottomRight.X;
            return bounds.SubtractMargin(InnerPadding).WithSizeNonZero(element.desiredSize);
        }
    }
}

public enum Direction
{
    Vertical,
    Horizontal
}
