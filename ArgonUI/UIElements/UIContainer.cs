using ArgonUI.SourceGenerator;
using ArgonUI.Styling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

[DebuggerDisplay("{GetType().Name,nq} ({name}, {Children.Count} children)")]
public abstract partial class UIContainer : UIElement
{
    // Must propagate PropertyChanged/OnDirty events from children
    // OnDraw invocations are propagated back down to children
    public abstract IReadOnlyList<UIElement> Children { get; }

    /// <summary>
    /// How much space (in pixels) to leave around each edge of each child element.
    /// </summary>
    [Reactive, CustomAccessibility("public virtual"), Stylable, Dirty(DirtyFlags.Layout)] 
    private Thickness innerPadding;

    /// <summary>
    /// Whether child elements which overflow the bounds of this container should be drawn.
    /// </summary>
    [Reactive, CustomAccessibility("public virtual"), Stylable, Dirty(DirtyFlags.Layout)]
    private bool clipContents;

    public UIContainer() : base() 
    {
        DirtyFlags |= DirtyFlags.AllChild;
    }

    /// <summary>
    /// Adds a <see cref="UIElement"/> as a child to this element.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="RegisterChild(UIElement)"/>.
    /// </remarks>
    /// <param name="child">The element to add as a child.</param>
    public abstract void AddChild(UIElement child);
    /// <summary>
    /// Adds a collection of <see cref="UIElement"/> as children to this element.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="RegisterChild(UIElement)"/> 
    /// for each child added.
    /// </remarks>
    /// <param name="children">The elements to add as children.</param>
    public abstract void AddChildren(IEnumerable<UIElement> children);
    /// <summary>
    /// Inserts a <see cref="UIElement"/> as a child to this element at a specific index.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="RegisterChild(UIElement)"/>.
    /// </remarks>
    /// <param name="child">The element to insert as a child.</param>
    /// <param name="index">The index in the list of children to insert the new element at.</param>
    public abstract void InsertChild(UIElement child, int index);

    /// <summary>
    /// Removes a <see cref="UIElement"/> from this element's list of children.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="UnregisterChild(UIElement)"/>.
    /// </remarks>
    /// <param name="child">The child element to remove.</param>
    /// <returns><see langword="true"/> if the element was a direct child of this element.</returns>
    public abstract bool RemoveChild(UIElement child);
    /// <summary>
    /// Removes a collection of <see cref="UIElement"/> from this element's list of children.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="UnregisterChild(UIElement)"/>.
    /// </remarks>
    /// <param name="children">The collection of children to remove.</param>
    public abstract void RemoveChildren(IEnumerable<UIElement> children);

    /// <summary>
    /// Removes all children from this element.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to invoke <see cref="UnregisterChild(UIElement)"/>.
    /// </remarks>
    public abstract void ClearChildren();

    //internal void Layout();
    // Draw() must layout and draw all children

    /// <summary>
    /// This method is invoked after this element's children have been measured. The intent is 
    /// to give the UIContainer a chance decide if it needs to dirty it's layout and hence 
    /// re-layout the rest of it's children.
    /// <para/>
    /// Example: If a button in a stack panel changes in height, then the stack panel needs to 
    /// recompute it's layout to move all the subsequant buttons up/down.
    /// </summary>
    /// <remarks>
    /// This method is only invoked if a measured child's size changed.
    /// </remarks>
    protected internal virtual void BeforeLayoutChildren()
    {
        //Dirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// This method is invoked whenever the layout of the children needs to be recomputed.
    /// Note that if the container doesn't need to precompute the children's bounds before they 
    /// request them (<see cref="RequestChildBounds(UIElement, int)"/>, then it's best to 
    /// compute the child bounds on demand.
    /// </summary>
    protected internal virtual void LayoutChildren()
    {

    }

    /// <summary>
    /// Gets the bounds a child element is allowed to use. 
    /// Note that this will always be called AFTER <see cref="LayoutChildren"/>.
    /// </summary>
    /// <param name="element">The instance of the child element requesting bounds.</param>
    /// <param name="index">The index of the child element requesting bounds.</param>
    /// <returns></returns>
    protected internal virtual Bounds2D RequestChildBounds(UIElement element, int index)
    {
        return RenderedBoundsAbsolute.SubtractMargin(innerPadding);
    }

    protected internal override Bounds2D Layout(int childIndex)
    {
        var bounds = base.Layout(childIndex);

        // When a container's layout changes, it's children need to be re-evaluated
        if (bounds != RenderedBoundsAbsolute)
        {
            foreach (var child in Children)
                child.Dirty(DirtyFlags.Layout);
        }

        return bounds;
    }

    /// <summary>
    /// Registration method which must be called by implementors after adding a new child element.
    /// </summary>
    /// <param name="element"></param>
    protected void RegisterChild(UIElement element)
    {
        if (element.Parent == this)
            return;
        if (element.Parent != null)
            throw new InvalidOperationException($"The element '{element}' cannot be a child of more than one container. Make sure to remove the element from it's current parent first!");
        element.Parent = this;
        if (window != null)
        {
            // We need to recursively update the depth of the entire subtree being added.
            element.treeDepth = treeDepth + 1;
            if (element is UIContainer container)
                container.UpdateTreeDepth();

            element.window = window;
            element.InvokeOnLoaded();
        }
        OnChildElementChanged(element, UIElementTreeChange.ElementAdded);
    }

    /// <summary>
    /// Registration method which must be called by implementors after removing a child element.
    /// </summary>
    /// <param name="element"></param>
    protected void UnregisterChild(UIElement element)
    {
        if (element.Parent == null)
            return; // Element has already been unregistered, shouldn't happen normally...

        element.Parent = null;
        element.window = null;
        element.treeDepth = -1;
        element.InvokeOnUnloaded();
        OnChildElementChanged(element, UIElementTreeChange.ElementRemoved);
    }

    /// <summary>
    /// Recursively computes the tree depth of all child elements.
    /// </summary>
    private void UpdateTreeDepth()
    {
        foreach (var child in Children)
        {
            child.treeDepth = treeDepth + 1;
            if (child is UIContainer container)
                container.UpdateTreeDepth();
        }
    }

    public override UIElement Clone(UIElement target)
    {
        base.Clone(target);
        if (target is UIContainer t)
        {
            t.innerPadding = innerPadding;
            t.clipContents = clipContents;

            foreach (UIElement child in Children)
                t.AddChild(child);
        }
        return target;
    }

    public override string? ToString() => $"{GetType().Name} ({Name}, {Children.Count} children)";
}
