using ArgonUI.Drawing;
using ArgonUI.Input;
using System.Numerics;
using System;
using System.Collections.Generic;
using ArgonUI.Styling;
using ArgonUI.SourceGenerator;
using System.Runtime.CompilerServices;
using ArgonUI.Helpers;
using System.Diagnostics;

namespace ArgonUI.UIElements;

/// <summary>
/// Represents the base class for all renderable UI elements in ArgonUI.
/// </summary>
[DebuggerDisplay("{GetType().Name,nq} ({name})")]
#pragma warning disable AR1004 // "Fields annotated with [Reactive] must be in a class which derives from UIElement"; this is the UIElement class so this doesn't apply
public abstract partial class UIElement : ReactiveObject, IDisposable
#pragma warning restore AR1004
{
    #region Public Properties
    /// <summary>
    /// The name associated with this UIElement.
    /// </summary>
    [Reactive]
    private string name;
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
    public ObservableStringBag Tags { get; } = [];

    /// <summary>
    /// The absolute width of this element. Set to 0 to use automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int width;
    /// <summary>
    /// The absolute height of this element. Set to 0 to use automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int height;
    //public Vector2 Pivot { get => pivot; set => UpdateProperty(ref pivot, value); }
    /// <summary>
    /// How this element should be aligned vertically relative to it's parent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private Alignment verticalAlignment;
    /// <summary>
    /// How this element should be aligned horizontally relative to it's parent.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private Alignment horizontalAlignment;
    /// <summary>
    /// How much space (in pixels) to leave around each edge of the element relative to the parent. Specified as a vector of (Top, Right, Bottom, Left).
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private Vector4 margin;
    /// <summary>
    /// Controls which elements are shown on top of each other. Higher z-indexes will be shown on top.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int zIndex;
    /// <summary>
    /// The smallest width to shrink this element down to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int minWidth = -1;
    /// <summary>
    /// The smallest height to shrink this element down to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int minHeight = -1;
    /// <summary>
    /// The largest width to expand this element up to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
    private int maxWidth = -1;
    /// <summary>
    /// The largest height to expand this element up to when using automatic sizing.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), Stylable]
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
    #endregion

    // Read only
    private float renderedWidth;
    private float renderedHeight;
    private Bounds2D renderedBounds; // Relative to parent
    private Bounds2D renderedBoundsAbsolute;
    private DirtyFlags dirtyFlag = DirtyFlags.Layout | DirtyFlags.Content;

    #region Events
    /// <summary>
    /// This event is invoked in the instant that a mouse button is pressed.
    /// </summary>
    public event MouseButtonEventHandler? OnMouseDown;
    /// <summary>
    /// This event is invoked the instant that a mouse button is released.
    /// </summary>
    public event MouseButtonEventHandler? OnMouseUp;
    /// <summary>
    /// This event is invoked when the cursor enters the collision rectangle of this element.
    /// This occurs when <see cref="HitTest(in Vector2)"/> returns <see langword="true"/>.
    /// </summary>
    public event MousePosEventHandler? OnMouseEnter;
    /// <summary>
    /// This event is invoked when the cursor leaves the collision rectangle of this element.
    /// This occurs when <see cref="HitTest(in Vector2)"/> returns <see langword="false"/>.
    /// </summary>
    public event MousePosEventHandler? OnMouseLeave;
    /// <summary>
    /// This event is invoked every frame that the mouse moves while it is hovering over this element.
    /// This occurs while <see cref="HitTest(in Vector2)"/> returns <see langword="true"/>.
    /// </summary>
    public event MousePosEventHandler? OnMouseOver;
    /// <summary>
    /// This event is invoked when this element is double clicked.
    /// </summary>
    public event MouseButtonEventHandler? OnDoubleClick;
    /// <summary>
    /// This event is invoked when the scroll wheel is scrolled, while the mouse is over this element.
    /// </summary>
    public event MouseScrollEventHandler? OnMouseWheel;
    /// <summary>
    /// This event is invoked when a key is pressed, while this element has keyboard focus <see cref="InputManager.FocussedElement"/>.
    /// </summary>
    public event KeyEventHandler? OnKeyDown;
    /// <summary>
    /// This event is invoked when a key is released, while this element has keyboard focus <see cref="InputManager.FocussedElement"/>.
    /// </summary>
    public event KeyEventHandler? OnKeyUp;
    /// <summary>
    /// This event is invoked when a drag-drop operation is started.
    /// </summary>
    public event InputEventHandler? OnDragStart;
    /// <summary>
    /// This event is invoked when a drag-drop operation is completed (the mouse button is released).
    /// </summary>
    public event InputEventHandler? OnDrop;
    /// <summary>
    /// This event is invoked when the cursor enters the collision rectangle of this element during a drag-drop operation.
    /// This occurs when <see cref="HitTest(in Vector2)"/> returns <see langword="true"/>.
    /// </summary>
    public event InputEventHandler? OnDragEnter;
    /// <summary>
    /// This event is invoked when the cursor leaves the collision rectangle of this element during a drag-drop operation.
    /// This occurs when <see cref="HitTest(in Vector2)"/> returns <see langword="false"/>.
    /// </summary>
    public event InputEventHandler? OnDragLeave;
    /// <summary>
    /// This event is invoked when the cursor moves when hovering over this element during a drag-drop operation.
    /// This occurs while <see cref="HitTest(in Vector2)"/> returns <see langword="false"/>.
    /// </summary>
    public event InputEventHandler? OnDragOver;
    /// <summary>
    /// This event is invoked when this element gains keyboard focus.
    /// See: <seealso cref="InputManager.FocussedElement"/>
    /// </summary>
    public event InputEventHandler? OnFocusGot;
    /// <summary>
    /// This event is invoked when this element looses keyboard focus.
    /// See: <seealso cref="InputManager.FocussedElement"/>
    /// </summary>
    public event InputEventHandler? OnFocusLost;

    /// <summary>
    /// This event is invoked when this element is added to a window (or one of its child elements).
    /// </summary>
    public event Action? OnLoaded; // Called when the UIElement is added to the ui graph
    /// <summary>
    /// This event is invoked when this element is removed from a window (or one of its child elements).
    /// </summary>
    public event Action? OnUnloaded;
    /// <summary>
    /// This event is invoked just before this element is drawn. Note that this event will only be invoked if this element has been 
    /// dirtied for redrawing, otherwise the element will be redrawn using the cached draw commands and this event will not be
    /// invoked.
    /// </summary>
    public event Action? OnDraw; // Often as a result of a bounds change, useful if the user wants to do anything on draw; tbh should probs just be invoked from Draw()
    /// <summary>
    /// This event is invoked when a property in a child element is changed.
    /// This event bubbles up to parent elements.
    /// </summary>
    public event ChildPropertyChangedHandler? ChildPropertyChanged;
    /// <summary>
    /// This event is invoked when a child element is added or removed. 
    /// This event bubbles up to parent elements.
    /// </summary>
    public event ChildElementChangedHandler? ChildElementChanged;

    protected internal event Action? OnDirty; // Mostly for internal use

    /// <summary>
    /// Represents the method that handles <see cref="ChildPropertyChanged"/> events.
    /// </summary>
    /// <param name="target">The child <see cref="UIElement"/> which raised the event.</param>
    /// <param name="propertyName">The name of the property which was changed.</param>
    public delegate void ChildPropertyChangedHandler(UIElement target, string? propertyName);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="treeChange"></param>
    public delegate void ChildElementChangedHandler(UIElement target, UIElementTreeChange treeChange);
    /// <summary>
    /// Represents the method that handles mouse down/up events.
    /// </summary>
    /// <param name="inputManager"></param>
    /// <param name="button">The mouse button which was pressed/released.</param>
    public delegate void MouseButtonEventHandler(InputManager inputManager, MouseButton button);
    /// <summary>
    /// Represents the method that handles mouse movement events.
    /// </summary>
    /// <param name="inputManager"></param>
    /// <param name="pos">The new position of the mouse.</param>
    public delegate void MousePosEventHandler(InputManager inputManager, VectorInt2 pos);
    /// <summary>
    /// Represents the method that handles mouse scroll events.
    /// </summary>
    /// <param name="inputManager"></param>
    /// <param name="delta">How much the mouse has scrolled since this event was last invoked.</param>
    public delegate void MouseScrollEventHandler(InputManager inputManager, Vector2 delta);
    /// <summary>
    /// Represents the method that handles keyboard events.
    /// </summary>
    /// <param name="inputManager"></param>
    /// <param name="key">The key that was pressed/released.</param>
    public delegate void KeyEventHandler(InputManager inputManager, KeyCode key);
    /// <summary>
    /// Represents the method that handles events from the input manager.
    /// </summary>
    /// <param name="inputManager"></param>
    public delegate void InputEventHandler(InputManager inputManager);
    #endregion

    public UIElement()
    {
        name = GetType().Name;
        // Hook up property changed listeners for this object to propagate to styles and parents.
        PropertyChanged += (o, e) => OnChildPropertyChanged(e.PropertyName);
        Tags.CollectionChanged += (o, e) => OnChildPropertyChanged(nameof(Tags));
    }

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

    internal void InvokeOnMouseDown(InputManager inputManager, MouseButton button) => OnMouseDown?.Invoke(inputManager, button);
    internal void InvokeOnMouseUp(InputManager inputManager, MouseButton button) => OnMouseUp?.Invoke(inputManager, button);
    internal void InvokeOnMouseEnter(InputManager inputManager, VectorInt2 pos) => OnMouseEnter?.Invoke(inputManager, pos);
    internal void InvokeOnMouseLeave(InputManager inputManager, VectorInt2 pos) => OnMouseLeave?.Invoke(inputManager, pos);
    internal void InvokeOnMouseOver(InputManager inputManager, VectorInt2 pos) => OnMouseOver?.Invoke(inputManager, pos);
    internal void InvokeOnDoubleClick(InputManager inputManager, MouseButton button) => OnDoubleClick?.Invoke(inputManager, button);
    internal void InvokeOnMouseWheel(InputManager inputManager, Vector2 delta) => OnMouseWheel?.Invoke(inputManager, delta);
    internal void InvokeOnKeyDown(InputManager inputManager, KeyCode key) => OnKeyDown?.Invoke(inputManager, key);
    internal void InvokeOnKeyUp(InputManager inputManager, KeyCode key) => OnKeyUp?.Invoke(inputManager, key);
    internal void InvokeOnDragStart(InputManager inputManager) => OnDragStart?.Invoke(inputManager);
    internal void InvokeOnDrop(InputManager inputManager) => OnDrop?.Invoke(inputManager);
    internal void InvokeOnDragEnter(InputManager inputManager) => OnDragEnter?.Invoke(inputManager);
    internal void InvokeOnDragLeave(InputManager inputManager) => OnDragLeave?.Invoke(inputManager);
    internal void InvokeOnDragOver(InputManager inputManager) => OnDragOver?.Invoke(inputManager);
    internal void InvokeOnFocusGot(InputManager inputManager) => OnFocusGot?.Invoke(inputManager);
    internal void InvokeOnFocusLost(InputManager inputManager) => OnFocusLost?.Invoke(inputManager);
    internal void InvokeOnLoaded() => OnLoaded?.Invoke();
    internal void InvokeOnUnloaded() => OnUnloaded?.Invoke();
    internal void InvokeOnDraw() => OnDraw?.Invoke();

    /// <summary>
    /// Invokes a <see cref="ChildPropertyChanged"/> event on all parent elements. 
    /// </summary>
    /// <remarks>
    /// When implementing custom UIElements, if you're having problems with styles not responding to property 
    /// changes, it's worth checking that this method is being invoked.
    /// </remarks>
    /// <param name="propertyName">The property which was changed.</param>
    protected void OnChildPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // Notify all parents one at a time. The event should bubble up, so all event
        // handlers of one element are invoked before the next parent's ones are invoked.
        var parent = this;
        while ((parent = parent!.Parent) != null)
            parent.ChildPropertyChanged?.Invoke(this, propertyName);
    }

    /// <summary>
    /// Invokes a <see cref="ChildElementChanged"/> event on all parent elements.
    /// </summary>
    /// <remarks>
    /// This method is intended to be invoked from classes deriving from 
    /// <see cref="UIContainer"/>. It must be called whenever a child element is 
    /// added, removed, or moved from the <see cref="UIContainer"/> for the
    /// styling system to work correctly.
    /// </remarks>
    /// <param name="target">The <see cref="UIElement"/> which was added/removed.</param>
    /// <param name="treeChange">The tree operation affecting this element.</param>
    protected static void OnChildElementChanged(UIElement target, UIElementTreeChange treeChange)
    {
        // Notify all parents one at a time. The event should bubble up, so all event
        // handlers of one element are invoked before the next parent's ones are invoked.
        var parent = target;
        while ((parent = parent!.Parent) != null)
            parent.ChildElementChanged?.Invoke(target, treeChange);
    }
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
