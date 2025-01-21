using System.ComponentModel;
using System.Numerics;
using System;
using System.Runtime.CompilerServices;

namespace ArgonUI;

/// <summary>
/// Represents the base class for all renderable UI elements in ArgonUI.
/// </summary>
public abstract class UIElement : ReactiveObject
{
    /// <summary>
    /// The name associated with this UIElement.
    /// </summary>
    public string Name { get => name; set => UpdateProperty(ref name, value); }
    /// <summary>
    /// Whether this element can receive keyboard focus.
    /// </summary>
    public bool Focusable { get => focusable; set => UpdateProperty(ref focusable, value); }
    //public Style style;
    //public KeyBindingCollection keybinds;

    /// <summary>
    /// The absolute width of this element. Set to 0 to use automatic sizing.
    /// </summary>
    public int Width { get => width; set => UpdateProperty(ref width, value); }
    /// <summary>
    /// The absolute height of this element. Set to 0 to use automatic sizing.
    /// </summary>
    public int Height { get => height; set => UpdateProperty(ref height, value); }
    //public Vector2 Pivot { get => pivot; set => UpdateProperty(ref pivot, value); }
    /// <summary>
    /// How this element should be aligned vertically relative to it's parent.
    /// </summary>
    public Alignment VerticalAlignment { get => verticalAlignment; set => UpdateProperty(ref verticalAlignment, value); }
    /// <summary>
    /// How this element should be aligned horizontally relative to it's parent.
    /// </summary>
    public Alignment HorizontalAlignment { get => horizontalAlignment; set => UpdateProperty(ref horizontalAlignment, value); }
    /// <summary>
    /// How much space (in pixels) to leave around each edge of the element relative to the parent. Specified as a vector of (Top, Right, Bottom, Left).
    /// </summary>
    public Vector4 Margin { get => margin; set => UpdateProperty(ref margin, value); }
    /// <summary>
    /// Controls which elements are shown on top of each other. Higher z-indexes will be shown on top.
    /// </summary>
    public int ZIndex { get => zIndex; set => UpdateProperty(ref zIndex, value); }
    /// <summary>
    /// The smallest width to shrink this element down to when using automatic sizing.
    /// </summary>
    public int MinWidth { get => minWidth; set => UpdateProperty(ref minWidth, value); }
    /// <summary>
    /// The smallest height to shrink this element down to when using automatic sizing.
    /// </summary>
    public int MinHeight { get => minHeight; set => UpdateProperty(ref minHeight, value); }
    /// <summary>
    /// The largest width to expand this element up to when using automatic sizing.
    /// </summary>
    public int MaxWidth { get => maxWidth; set =>  UpdateProperty(ref maxWidth, value); }
    /// <summary>
    /// The largest height to expand this element up to when using automatic sizing.
    /// </summary>
    public int MaxHeight { get => maxHeight; set => UpdateProperty(ref maxHeight, value); }

    /// <summary>
    /// The rendered width of the element after it was last drawn. (Read-only)
    /// </summary>
    public float RenderedWidth { get => renderedWidth; protected set => renderedWidth = value; }
    /// <summary>
    /// The rendered height of the element after it was last drawn. (Read-only)
    /// </summary>
    public float RenderedHeight { get => renderedHeight; protected set => renderedHeight = value; }
    /// <summary>
    /// The computed bounds of the element after it was last drawn relative to it's parent. (Read-only)
    /// </summary>
    public Bounds2D RenderedBounds { get => renderedBounds; protected set => renderedBounds = value; }
    /// <summary>
    /// The computed bounds of the element after it was last drawn. (Read-only)
    /// </summary>
    public Bounds2D RenderedBoundsAbsolute { get => renderedBoundsAbsolute; protected set => renderedBoundsAbsolute = value; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDirty { get => isDirty; protected internal set => isDirty = value; }
    /// <summary>
    /// Gets the width of the element when using automatic sizing.
    /// </summary>
    public virtual int DesiredWidth => Width;
    /// <summary>
    /// Gets the height of the element when using automatic sizing.
    /// </summary>
    public virtual int DesiredHeight => Height;

    private string name = nameof(UIElement);
    private bool focusable = true;
    //public Style style;
    //public KeyBindingCollection keybinds;

    private int width;
    private int height;
    //private Vector2 pivot;
    private Alignment verticalAlignment;
    private Alignment horizontalAlignment;
    private Vector4 margin;
    private int zIndex;

    private int minWidth = -1;
    private int minHeight = -1;
    private int maxWidth = -1;
    private int maxHeight = -1;

    // Read only
    private float renderedWidth;
    private float renderedHeight;
    private Bounds2D renderedBounds; // Relative to parent
    private Bounds2D renderedBoundsAbsolute;
    private bool isDirty;

    // Events
    public event Action? OnMouseDown;
    public event Action? OnMouseUp;
    public event Action? OnMouseEnter;
    public event Action? OnMouseLeave;
    public event Action? OnMouseOver;
    public event Action? OnDoubleClick;
    public event Action? OnMouseWheel;
    public event Action? OnKeyDown;
    public event Action? OnKeyUp;
    public event Action? OnDragStart;
    public event Action? OnDrop;
    public event Action? OnDragEnter;
    public event Action? OnDragLeave;
    public event Action? OnDragOver;
    public event Action? OnFocusGot;
    public event Action? OnFocusLost;

    public event Action? OnLoaded; // Called when the UIElement is added to the ui graph
    public event Action? OnUnloaded;
    public event Action? OnDraw; // Often as a result of a bounds change, useful if the user wants to do anything on draw; tbh should probs just be invoked from Draw()

    protected internal event Action? OnDirty; // Mostly for internal use

    /// <summary>
    /// Marks this element as dirty, forcing the UI engine to redraw this element and it's children when it's next dispatched.
    /// </summary>
    public void Dirty()
    {
        UpdateProperty(ref isDirty, true, nameof(IsDirty));
    }

    // Internal
    /// <summary>
    /// Computes the bounds that this element occupies given the bounds of it's parent.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    protected virtual Bounds2D ComputeBounds(Bounds2D parent)
    { 
        // Apply limits to the width and height
        var parentWidth = parent.Width;
        var parentHeight = parent.Height;
        var desiredWidth = width > 0 ? width : DesiredWidth;
        var desiredHeight = height > 0 ? height : DesiredHeight;

        if (minWidth >= 0)
            desiredWidth = Math.Max(desiredWidth, minWidth);
        if (maxWidth >= 0)
            desiredWidth = Math.Min(desiredWidth, maxWidth);
        if (minHeight >= 0)
            desiredHeight = Math.Max(desiredHeight, minHeight);
        if (maxHeight >= 0)
            desiredHeight = Math.Min(desiredHeight, maxHeight);

        float left = parent.topLeft.X, right = parent.bottomRight.X, top = parent.topLeft.Y, bottom = parent.bottomRight.Y;

        // Compute the horizontal bounds
        switch (horizontalAlignment)
        {
            case Alignment.Left:
                left = parent.topLeft.X + margin.W;
                right = left + desiredWidth;
                break;
            case Alignment.Right:
                right = parent.bottomRight.X - margin.Y;
                left = right - desiredWidth;
                break;
            case Alignment.Stretch:
                left = parent.topLeft.X + margin.W;
                var finalWidth = parentWidth - margin.Y - margin.W;
                if (minWidth >= 0)
                    finalWidth = Math.Max(finalWidth, minWidth);
                right = left + finalWidth;
                break;
            case Alignment.Centre:
                left = parent.topLeft.X + (parentWidth - desiredWidth) * 0.5f;
                right = parent.topLeft.X + (parentWidth + desiredWidth) * 0.5f;
                break;
        }

        // Compute the vertical bounds
        switch (verticalAlignment)
        {
            case Alignment.Top:
                top = parent.topLeft.Y + margin.X;
                bottom = top + desiredHeight;
                break;
            case Alignment.Bottom:
                bottom = parent.bottomRight.Y - margin.Z;
                top = bottom - desiredHeight;
                break;
            case Alignment.Stretch:
                top = parent.topLeft.Y + margin.X;
                var finalHeight = parentHeight - margin.X - margin.Z;
                if (minHeight >= 0)
                    finalHeight = Math.Max(finalHeight, minHeight);
                bottom = top + finalHeight;
                break;
            case Alignment.Centre:
                top = parent.topLeft.Y + (parentHeight - desiredHeight) * 0.5f;
                bottom = parent.topLeft.Y + (parentHeight + desiredHeight) * 0.5f;
                break;
        }

        return new(left, right, top, bottom);
    }

    /// <summary>
    /// Uses the draw context provided to draw this element to the screen using the provided bounds.
    /// </summary>
    /// <remarks>
    /// This method should set <see cref="isDirty"/> to <see langword="false"/> when complete.
    /// </remarks>
    /// <param name="bounds"></param>
    /// <param name="context"></param>
    protected abstract void Draw(Bounds2D bounds, IDrawContext context);
}

public enum Alignment
{
    Left,
    Right,
    Centre,
    Stretch,

    Center = Centre,
    Top = Left,
    Bottom = Right,
    Start = Left,
    End = Right
}
