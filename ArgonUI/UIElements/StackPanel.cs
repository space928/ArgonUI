using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements;

[UIClonable]
public partial class StackPanel : Panel
{
    [Reactive, Dirty(DirtyFlags.Layout)]
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
        Dirty(DirtyFlags.Layout);
    }

    protected internal override VectorInt2 Measure()
    {
        var children = Children;
        if (children.Count == 0)
            return base.Measure();

        VectorInt2 res = VectorInt2.Zero;
        VectorInt2 pad = new((int)(InnerPadding.left + InnerPadding.right), (int)(InnerPadding.top + InnerPadding.bottom));
        if (direction == Direction.Vertical)
        {
            foreach (var child in children)
            {
                res.x = Math.Max(res.x, child.desiredSize.x);
                res.y += child.desiredSize.y + pad.y;
            }
            res.x += pad.x;
        } 
        else
        {
            foreach (var child in children)
            {
                res.x += child.desiredSize.x + pad.x;
                res.y = Math.Max(res.y, child.desiredSize.y);
            }
            res.y += pad.y;
        }

        return res;
    }

    /*protected internal override void LayoutChildren()
    {
        
    }*/

    protected internal override Bounds2D RequestChildBounds(UIElement element, int index)
    {
        if (index == 0)
            return RenderedBoundsAbsolute.SubtractMargin(InnerPadding);

        if (direction == Direction.Vertical)
        {
            var bounds = RenderedBoundsAbsolute;
            bounds.topLeft.Y = Children[index - 1].RenderedBoundsAbsolute.bottomRight.Y;
            return bounds.SubtractMargin(InnerPadding);
        }
        else
        {
            var bounds = RenderedBoundsAbsolute;
            bounds.topLeft.X = Children[index - 1].RenderedBoundsAbsolute.bottomRight.X;
            return bounds.SubtractMargin(InnerPadding);
        }
    }
}

public enum Direction
{
    Vertical,
    Horizontal
}
