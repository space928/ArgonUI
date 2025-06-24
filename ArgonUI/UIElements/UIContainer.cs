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
    [Reactive, CustomAccessibility("public virtual"), Stylable, Dirty(DirtyFlags.ChildLayout)] 
    private Vector4 innerPadding;
    [Reactive, CustomAccessibility("public virtual"), Stylable, Dirty(DirtyFlags.ChildLayout)]
    private bool clipContents;

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
}
