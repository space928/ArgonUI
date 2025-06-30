using ArgonUI.Drawing;
using ArgonUI.Helpers;
using ArgonUI.Input;
using ArgonUI.SourceGenerator;
using ArgonUI.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    /// Whether this element is enabled. Disabled UI elements will still be drawn, but will not receive input events.
    /// </summary>
    [Reactive, Stylable]
    private bool enabled = true;
    /// <summary>
    /// Whether this element and it's children should be drawn. Elements which are not visible will not receive input events, nor will they participate in layout or drawing.
    /// </summary>
    [Reactive, Stylable, Dirty(DirtyFlags.Layout)]
    private Visibility visible = Visibility.Visible;
    /// <summary>
    /// A set of stylable properties to be applied to this UIElement and it's decendants.
    /// </summary>
    [Reactive, CustomSet("this.style?.Unregister(this); value?.Register(this); this.style = value", true)]
    private StyleSet? style;
    //public KeyBindingCollection keybinds;
    /// <summary>
    /// Gets the parent UIElement.
    /// </summary>
    public UIContainer? Parent { get; internal set; }
    /// <summary>
    /// A list of string tags which can be used to store metadata about this element. 
    /// These can also be used similar to HTML classes with a <see cref="Styling.Selectors.TagSelector"/>
    /// to style elements selectivly based on their tags.
    /// </summary>
    public ObservableStringSet Tags { get; } = [];

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
    private Thickness margin;
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
    public float RenderedWidth { get => renderedWidth; internal set => renderedWidth = value; }
    /// <summary>
    /// The rendered height of the element after it was last drawn. (Read-only)
    /// </summary>
    public float RenderedHeight { get => renderedHeight; internal set => renderedHeight = value; }
    /// <summary>
    /// The computed bounds of the element after it was last drawn relative to it's parent. (Read-only)
    /// </summary>
    public Bounds2D RenderedBounds { get => renderedBounds; internal set => renderedBounds = value; }
    /// <summary>
    /// The computed bounds of the element after it was last drawn. (Read-only)
    /// </summary>
    public Bounds2D RenderedBoundsAbsolute { get => renderedBoundsAbsolute; internal set => renderedBoundsAbsolute = value; }
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
    public int DesiredWidth => desiredSize.x;
    /// <summary>
    /// Gets the height of the element when using automatic sizing.
    /// </summary>
    public int DesiredHeight => desiredSize.y;
    /// <summary>
    /// Gets whether the mouse is currently hovering over this element.
    /// </summary>
    public bool IsHovered => isHovered;
    /// <summary>
    /// Gets whether the mouse is currently clicking on this element.
    /// </summary>
    public bool IsPressed => isPressed;
    /// <summary>
    /// Gets whether this element currently has input focus.
    /// </summary>
    public bool IsFocused => isFocused;
    /// <summary>
    /// Gets the window this element is current a member of. Returns <see langword="null"/> if the element is not attached to a window.
    /// </summary>
    public UIWindow? Window => window;
    /// <summary>
    /// Gets the depth of this element in the tree of UI Elements attached to the window. This is 
    /// effectively a count of how many <see cref="Parent"/>s this element has.
    /// <para/>
    /// Returns <c>-1</c> if this element isn't attached to a window.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int TreeDepth => treeDepth;
    #endregion

    // Read only
    private float renderedWidth;
    private float renderedHeight;
    private Bounds2D renderedBounds; // Relative to parent
    private Bounds2D renderedBoundsAbsolute;
    /// <summary>
    /// This is used internally by the rendering engine. It's defined as the union of the current 
    /// <see cref="RenderedBoundsAbsolute"/> and the previous rendered bounds.
    /// </summary>
    internal Bounds2D invalidatedRenderBounds;
    internal VectorInt2 desiredSize;
    private DirtyFlags dirtyFlag = DirtyFlags.ContentAndLayout;
    private bool isHovered;
    private bool isPressed;
    private bool isFocused;
    internal UIWindow? window;
    internal protected int treeDepth = -1;

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
    protected internal event Action<UIElement, UIElementInputChange>? OnStylableInputEvent;

    /// <summary>
    /// Represents the method that handles <see cref="ChildPropertyChanged"/> events.
    /// </summary>
    /// <param name="target">The child <see cref="UIElement"/> which raised the event.</param>
    /// <param name="propertyName">The name of the property which was changed.</param>
    public delegate void ChildPropertyChangedHandler(UIElement target, string? propertyName);
    /// <summary>
    /// Represents the method that handles <see cref="ChildElementChanged"/> events.
    /// </summary>
    /// <param name="target">The <see cref="UIElement"/> which is involved in the tree operation.</param>
    /// <param name="treeChange">What change occurred to the UI tree that triggered this event.</param>
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
        var prev = dirtyFlag;
        var newFlags = prev | flags;
        if (newFlags == prev)
            return;

        UpdateProperty(ref dirtyFlag, newFlags, nameof(DirtyFlags));

#if DEBUG && DEBUG_PROP_UPDATES
        var trace = string.Join(", ", new StackTrace().GetFrames().Take(5).Select(x => x.GetMethod().Name));
        Debug.WriteLine($"[UIElement] Dirty: {new string(' ', Math.Max(treeDepth, 0))}{this} -> {dirtyFlag}   {trace}");
#endif

        // Propagate dirty flags up
        var toPropagate = flags & (DirtyFlags.ChildContent | DirtyFlags.ChildLayout);
        if ((flags & DirtyFlags.Layout) != 0)
            toPropagate |= DirtyFlags.ChildLayout;
        if ((flags & DirtyFlags.Content) != 0)
            toPropagate |= DirtyFlags.ChildContent;

        Parent?.Dirty(toPropagate);
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

    /// <summary>
    /// Creates a deep copy of this <see cref="UIElement"/>.
    /// </summary>
    /// <remarks>
    /// Implementors of new <see cref="UIElement"/> subclasses can implement this method as follows:<para/>
    /// <c>
    /// public override UIElement Clone() => Clone(new MyUIElement());
    /// </c>
    /// </remarks>
    /// <returns>A new <see cref="UIElement"/> instance with the same properties as this instance.</returns>
    public abstract UIElement Clone();

    /// <summary>
    /// Copies this <see cref="UIElement"/>'s properties into those of a target <see cref="UIElement"/>.
    /// </summary>
    /// <remarks>
    /// This method is used internally by <see cref="Clone()"/>. Implementors of new <see cref="UIElement"/> 
    /// subclasses should override this method to clone properties defined in their subclass.
    /// </remarks>
    /// <param name="target">The <see cref="UIElement"/> to clone our properties into.</param>
    /// <returns>The <see cref="UIElement"/> passed into <paramref name="target"/>.</returns>
    public virtual UIElement Clone(UIElement target)
    {
        // TODO: Having an analyser check we didn't miss any fields for cloning might be good.
        //       I'm hesitant to fully automate parameter cloning (through reflection or source
        //       generation), as there is some logic to apply to it which could be complex.
        // Not sure if we should invalidate dirty flags here or not...
        target.dirtyFlag = DirtyFlags.Layout;
        target.focusable = focusable;
        target.height = height;
        target.width = width;
        target.Style = style; // Don't deep-copy the style
        target.horizontalAlignment = horizontalAlignment;
        target.verticalAlignment = verticalAlignment;
        target.margin = margin;
        target.maxHeight = maxHeight;
        target.maxWidth = maxWidth;
        target.minHeight = minHeight;
        target.minWidth = minWidth;
        target.zIndex = zIndex;
        target.name = name;

        return target;
    }

    /// <summary>
    /// Measure the desired size of this UI Element.
    /// </summary>
    /// <remarks>
    /// When drawing the UI, events happen in the following order: Measure() -> Layout() -> Draw()
    /// <para/>
    /// Layout() and Draw() occur from root node to leaves, whereas measure is invoked on the leaves first working it's way up to the root.
    /// </remarks>
    /// <returns>The desired width and height of this element.</returns>
    internal protected virtual VectorInt2 Measure()
    {
        return new VectorInt2(width, height);
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
        var parentSize = parent.Size;
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
                left = parent.topLeft.X + margin.left;
                right = left + desiredWidth;
                break;
            case Alignment.Right:
                right = parent.bottomRight.X - margin.right;
                left = right - desiredWidth;
                break;
            case Alignment.Stretch:
                left = parent.topLeft.X + margin.left;
                var finalWidth = parentSize.X - margin.right - margin.left;
                if (minWidth >= 0)
                    finalWidth = Math.Max(finalWidth, minWidth);
                right = left + finalWidth;
                break;
            case Alignment.Centre:
                left = parent.topLeft.X + (parentSize.X - desiredWidth) * 0.5f;
                right = parent.topLeft.X + (parentSize.X + desiredWidth) * 0.5f;
                break;
        }

        // Compute the vertical bounds
        switch (verticalAlignment)
        {
            case Alignment.Top:
                top = parent.topLeft.Y + margin.top;
                bottom = top + desiredHeight;
                break;
            case Alignment.Bottom:
                bottom = parent.bottomRight.Y - margin.bottom;
                top = bottom - desiredHeight;
                break;
            case Alignment.Stretch:
                top = parent.topLeft.Y + margin.top;
                var finalHeight = parentSize.Y - margin.top - margin.bottom;
                if (minHeight >= 0)
                    finalHeight = Math.Max(finalHeight, minHeight);
                bottom = top + finalHeight;
                break;
            case Alignment.Centre:
                top = parent.topLeft.Y + (parentSize.Y - desiredHeight) * 0.5f;
                bottom = parent.topLeft.Y + (parentSize.Y + desiredHeight) * 0.5f;
                break;
        }

        return new(left, right, top, bottom);
    }

    /// <summary>
    /// Uses the draw context provided to draw this element to the screen using the provided bounds.
    /// </summary>
    /// <param name="context">The drawing context to execute draw commands on.</param>
    internal protected abstract void Draw(IDrawContext context);//(Bounds2D bounds, List<Action<IDrawContext>> commands);

    /// <summary>
    /// Updates the layout of this element given it's parent.
    /// </summary>
    /// <returns>The computed window-space bounds of this element.</returns>
    /// <param name="childIndex">The index of this element into the parent container's list of children.</param>
    internal protected virtual Bounds2D Layout(int childIndex)
    {
        var parent = Parent;
        if (parent == null)
            return Bounds2D.Zero;

        var parentBounds = parent.RequestChildBounds(this, childIndex);
        var bounds = ComputeBounds(parentBounds);
        RenderedBounds = new(bounds.topLeft - parentBounds.topLeft, bounds.bottomRight - parentBounds.topLeft);

        // Invalidating the layout implies invalidating the content
        Dirty(DirtyFlags.Content);

        return bounds;
    }

    internal void InvokeOnMouseDown(InputManager inputManager, MouseButton button)
    {
        isPressed = true;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.MousePress);
        OnMouseDown?.Invoke(inputManager, button);
    }
    internal void InvokeOnMouseUp(InputManager inputManager, MouseButton button)
    {
        isPressed = false;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.MousePress);
        OnMouseUp?.Invoke(inputManager, button);
    }
    internal void InvokeOnMouseEnter(InputManager inputManager, VectorInt2 pos)
    {
        isHovered = true;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.MouseHover);
        OnMouseEnter?.Invoke(inputManager, pos);

        Parent?.InvokeOnMouseEnter(inputManager, pos);
    }
    internal void InvokeOnMouseLeave(InputManager inputManager, VectorInt2 pos)
    {
        isHovered = false;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.MouseHover);
        OnMouseLeave?.Invoke(inputManager, pos);

        Parent?.InvokeOnMouseLeave(inputManager, pos);
    }
    internal void InvokeOnMouseOver(InputManager inputManager, VectorInt2 pos)
    {
        isHovered = true;
        //OnStylableInputEvent?.Invoke(UIElementInputChange.MouseHover);
        OnMouseOver?.Invoke(inputManager, pos);

        // TODO: Bubble events up to parents
        // We might consider doing the bubbling in the input manager, and passing a shared
        // reference to the event args object so that subscribers can stop the bubbling
        // of the event.
        Parent?.InvokeOnMouseOver(inputManager, pos);
    }
    internal void InvokeOnDoubleClick(InputManager inputManager, MouseButton button) => OnDoubleClick?.Invoke(inputManager, button);
    internal void InvokeOnMouseWheel(InputManager inputManager, Vector2 delta) => OnMouseWheel?.Invoke(inputManager, delta);
    internal void InvokeOnKeyDown(InputManager inputManager, KeyCode key)
    {
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.KeyPress);
        OnKeyDown?.Invoke(inputManager, key);
    }
    internal void InvokeOnKeyUp(InputManager inputManager, KeyCode key)
    {
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.KeyPress);
        OnKeyUp?.Invoke(inputManager, key);
    }
    internal void InvokeOnDragStart(InputManager inputManager) => OnDragStart?.Invoke(inputManager);
    internal void InvokeOnDrop(InputManager inputManager) => OnDrop?.Invoke(inputManager);
    internal void InvokeOnDragEnter(InputManager inputManager) => OnDragEnter?.Invoke(inputManager);
    internal void InvokeOnDragLeave(InputManager inputManager) => OnDragLeave?.Invoke(inputManager);
    internal void InvokeOnDragOver(InputManager inputManager) => OnDragOver?.Invoke(inputManager);
    internal void InvokeOnFocusGot(InputManager inputManager)
    {
        isFocused = true;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.Focus);
        OnFocusGot?.Invoke(inputManager);
    }
    internal void InvokeOnFocusLost(InputManager inputManager)
    {
        isFocused = false;
        OnStylableInputEvent?.Invoke(this, UIElementInputChange.Focus);
        OnFocusLost?.Invoke(inputManager);
    }
    internal void InvokeOnLoaded()
    {
        OnLoaded?.Invoke();
    }
    internal void InvokeOnUnloaded()
    {
        OnUnloaded?.Invoke();
    }
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

    public override string? ToString() => $"{GetType().Name} ({name})";
}

/// <summary>
/// Describes the horizontal/vertical alignment of a <see cref="UIElement"/> within it's bounding rectangle.
/// </summary>
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

/// <summary>
/// Describes whether or not a <see cref="UIElement"/> should be drawn, and whether it should participate in layout.
/// </summary>
public enum Visibility
{
    /// <summary>
    /// This element participates in layout and drawing as normal.
    /// </summary>
    Visible,
    /// <summary>
    /// This element does not participate in layout or drawing, no layout space is reserved for this element. Hidden elements do not receive input events.
    /// </summary>
    Hidden,
    /// <summary>
    /// This element does not participate in drawing, but still participates in layout events, layout space is reserved for this element. Hidden elements do not receive input events.
    /// </summary>
    SkipDrawing
}
