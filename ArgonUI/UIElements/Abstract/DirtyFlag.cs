using System;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// Flags used to indicate if a <see cref="UIElement"/> needs 
/// to redrawing or re-laying out.
/// </summary>
[Flags]
public enum DirtyFlag
{
    None,
    /// <summary>
    /// The layout of this element is invalid, it's <see cref="UIElement.RenderedBoundsAbsolute"/> 
    /// may need to change. Dirtying the layout of an element implies that it's <see cref="Content"/>
    /// is also dirty.
    /// </summary>
    Layout = 1 << 0,
    /// <summary>
    /// The content of this element is invalid, it's bounds have not changed but a property 
    /// affecting it's inner content (such as it's colour) has changed.
    /// </summary>
    Content = 1 << 1,
    /// <summary>
    /// A child element of this <see cref="UIContainer"/> has a dirty <see cref="Layout"/>.
    /// </summary>
    ChildLayout = 1 << 2,
    /// <summary>
    /// A child element of this <see cref="UIContainer"/> has a dirty <see cref="Content"/>.
    /// </summary>
    ChildContent = 1 << 3,

    /// <summary>
    /// Both the <see cref="Content"/> and <see cref="Layout"/> are dirty.
    /// </summary>
    ContentAndLayout = Content | Layout,
    /// <summary>
    /// Both the <see cref="ChildContent"/> and <see cref="ChildLayout"/> are dirty.
    /// </summary>
    ChildContentAndLayout = ChildContent | ChildLayout,
    /// <summary>
    /// All <see cref="DirtyFlag"/>s are set.
    /// </summary>
    All = ContentAndLayout | ChildContentAndLayout,
}
