using ArgonUI.Drawing;
using ArgonUI.Input;
using System.Numerics;
using System;
using System.Collections.Generic;
using ArgonUI.Styling;
using ArgonUI.SourceGenerator;

namespace ArgonUI.UIElements;

/// <summary>
/// Represents the base class for all renderable UI elements in ArgonUI.
/// </summary>
public abstract partial class UIElement : ReactiveObject, IDisposable
{
    /// <summary>
    /// The name associated with this UIElement.
    /// </summary>
    [Reactive]
    private string name = nameof(UIElement);
    /// <summary>
    /// Whether this element can receive keyboard focus.
    /// </summary>
    [Reactive]
    private bool focusable = true;
    /// <summary>
    /// A set of stylable properties to be applied to this UIElement and it's decendants.
    /// </summary>
    [Reactive, CustomSet("this.style?.Unregister(this); value?.Register(this); this.style = value", true)]
    private StyleSet? style;
    //public KeyBindingCollection keybinds;
    /// <summary>
    /// Gets the parent UIElement.
    /// </summary>
    public UIElement? Parent { get; internal set; }
    /// <summary>
    /// A list of string tags which can be used to store metadata about this element. 
    /// These can also be used similar to HTML classes with a <see cref="Styling.Selectors.TagSelector"/>
    /// to style elements selectivly based on their tags.
    /// </summary>
    public ObservableStringSet Tags { get; } = [];

    /// <summary>
    /// The absolute width of this element. Set to 0 to use automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int width;
    /// <summary>
    /// The absolute height of this element. Set to 0 to use automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int height;
    //public Vector2 Pivot { get => pivot; set => UpdateProperty(ref pivot, value); }
    /// <summary>
    /// How this element should be aligned vertically relative to it's parent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private Alignment verticalAlignment;
    /// <summary>
    /// How this element should be aligned horizontally relative to it's parent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private Alignment horizontalAlignment;
    /// <summary>
    /// How much space (in pixels) to leave around each edge of the element relative to the parent. Specified as a vector of (Top, Right, Bottom, Left).
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private Vector4 margin;
    /// <summary>
    /// Controls which elements are shown on top of each other. Higher z-indexes will be shown on top.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int zIndex;
    /// <summary>
    /// The smallest width to shrink this element down to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int minWidth = -1;
    /// <summary>
    /// The smallest height to shrink this element down to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int minHeight = -1;
    /// <summary>
    /// The largest width to expand this element up to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int maxWidth = -1;
    /// <summary>
    /// The largest height to expand this element up to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout)]
    private int maxHeight = -1;

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
    /// Gets the dirty flags of this element which determine if the element needs to be re-rendered or re-laid out.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Dirty(DirtyFlags)"/> and <see cref="ClearDirtyFlag(DirtyFlags)"/> methods to set the dirty flags.
    /// </remarks>
    public DirtyFlags DirtyFlags { get => dirtyFlag; protected internal set => dirtyFlag = value; }
    /// <summary>
    /// Gets the width of the element when using automatic sizing.
    /// </summary>
    public virtual int DesiredWidth => Width;
    /// <summary>
    /// Gets the height of the element when using automatic sizing.
    /// </summary>
    public virtual int DesiredHeight => Height;

    // Read only
    private float renderedWidth;
    private float renderedHeight;
    private Bounds2D renderedBounds; // Relative to parent
    private Bounds2D renderedBoundsAbsolute;
    private DirtyFlags dirtyFlag = DirtyFlags.Layout | DirtyFlags.Content;

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
    /// <param name="flags">Which <see cref="ArgonUI.UIElements.DirtyFlags"/> to set.</param>
    public virtual void Dirty(DirtyFlags flags)
    {
        UpdateProperty(ref dirtyFlag, dirtyFlag | flags, nameof(DirtyFlags));

        // Propagate dirty flags up
        if ((flags & DirtyFlags.Layout) != 0) 
            Parent?.Dirty(DirtyFlags.ChildLayout);

        if ((flags & DirtyFlags.Content) != 0)
            Parent?.Dirty(DirtyFlags.ChildContent);
    }

    /// <summary>
    /// Clears the given dirty flags from the UI element.
    /// </summary>
    /// <param name="flags">Which <see cref="ArgonUI.UIElements.DirtyFlags"/> to clear.</param>
    public virtual void ClearDirtyFlag(DirtyFlags flags)
    {
        dirtyFlag &= ~flags;
    }

    /// <summary>
    /// Checks if the given point overlaps this element.
    /// </summary>
    /// <param name="pos">The point in screen space pixels to test.</param>
    /// <returns><see langword="true"/> if given point is on this element.</returns>
    public virtual bool HitTest(in Vector2 pos)
    {
        return renderedBoundsAbsolute.Contains(pos);
    }

    public void Dispose()
    {
        style?.Unregister(this);
    }

    // Internal
    /// <summary>
    /// Computes the bounds that this element occupies given the bounds of it's parent.
    /// </summary>
    /// <param name="parent">The bounds of the parent element.</param>
    /// <returns>The bounds occupied by this element in screen space.</returns>
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
    /// <param name="bounds">The bounds (in window-space) of this element.</param>
    /// <param name="commands">The drawing command list to populate.</param>
    internal protected abstract void Draw(Bounds2D bounds, List<Action<IDrawContext>> commands);

    /// <summary>
    /// Updates the layout of this element given it's parent.
    /// </summary>
    /// <returns>The computed window-space bounds of this element.</returns>
    internal protected virtual Bounds2D Layout()
    {
        if (Parent is not UIElement parent)
            return Bounds2D.Zero;

        var parentBounds = parent.renderedBounds;
        var bounds = ComputeBounds(parentBounds);
        RenderedBoundsAbsolute = bounds;
        RenderedBounds = new(bounds.topLeft - parentBounds.topLeft, bounds.bottomRight - parentBounds.topLeft);
        RenderedWidth = bounds.Width;
        RenderedHeight = bounds.Height;

        // Invalidating the layout implies invalidating the content
        Dirty(DirtyFlags.Content);

        return bounds;
    }

    internal void InvokeOnMouseDown(MouseButton button) => OnMouseDown?.Invoke();
    internal void InvokeOnMouseUp(MouseButton button) => OnMouseUp?.Invoke();
    internal void InvokeOnMouseEnter(VectorInt2 pos) => OnMouseEnter?.Invoke();
    internal void InvokeOnMouseLeave(VectorInt2 pos) => OnMouseLeave?.Invoke();
    internal void InvokeOnMouseOver(VectorInt2 pos) => OnMouseOver?.Invoke();
    internal void InvokeOnDoubleClick() => OnDoubleClick?.Invoke();
    internal void InvokeOnMouseWheel(Vector2 delta) => OnMouseWheel?.Invoke();
    internal void InvokeOnKeyDown(KeyCode key) => OnKeyDown?.Invoke();
    internal void InvokeOnKeyUp(KeyCode key) => OnKeyUp?.Invoke();
    internal void InvokeOnDragStart() => OnDragStart?.Invoke();
    internal void InvokeOnDrop() => OnDrop?.Invoke();
    internal void InvokeOnDragEnter() => OnDragEnter?.Invoke();
    internal void InvokeOnDragLeave() => OnDragLeave?.Invoke();
    internal void InvokeOnDragOver() => OnDragOver?.Invoke();
    internal void InvokeOnFocusGot() => OnFocusGot?.Invoke();
    internal void InvokeOnFocusLost() => OnFocusLost?.Invoke();
    internal void InvokeOnLoaded() => OnLoaded?.Invoke();
    internal void InvokeOnUnloaded() => OnUnloaded?.Invoke();
    internal void InvokeOnDraw() => OnDraw?.Invoke();
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
