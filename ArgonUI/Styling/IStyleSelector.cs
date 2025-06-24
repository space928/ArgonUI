using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

/// <summary>
/// Provides a method which allows a tree of UIElements to be filtered based on some 
/// selection criteria.
/// <para/>
/// This can be combined with <seealso cref="IFlattenedStyleSelector"/>.
/// </summary>
public interface IStyleSelector
{
    /// <summary>
    /// Filters a subtree of <see cref="UIElement"/>, returning the ones which match the selector.
    /// </summary>
    /// <param name="elementTree">The element tree to filter.</param>
    /// <returns>An enumerable of filtered elements.</returns>
    public IEnumerable<UIElement> Filter(UIElement elementTree);
    /// <summary>
    /// Determines if an update to any of the styled elements should result in the re-evaluation of 
    /// this style selector.
    /// </summary>
    /// <param name="target">The element which was affected.</param>
    /// <param name="propertyName">The name of the property which was changed; <see langword="null"/> if a tree change occurred.</param>
    /// <param name="treeChange">Whether a child element was added or removed from the UI tree.</param>
    /// <param name="inputChange">Whether an input event which might affect this style has occurred.</param>
    /// <returns>Which, if any, elements need to be re-evaluated and potentially re-styled.</returns>
    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange);
    /// <summary>
    /// This event is invoked by the selector when it's properties have been changed, such that it 
    /// must re-select the elements it controls.
    /// </summary>
    public event Action<IStyleSelector> RequestReevaluation;
}

/// <summary>
/// Provides a method which allows a sequence of UIElements to be filtered based on 
/// some selection criteria. For selectors which don't rely on the element hierarchy, 
/// then implementing this interface can allow for some performance benefits.
/// <para/>
/// This must be combined with <seealso cref="IStyleSelector"/>.
/// </summary>
public interface IFlattenedStyleSelector
{
    /// <summary>
    /// Filters an enumerable of <see cref="UIElement"/>, returning the ones which match the selector.
    /// <para/>
    /// This can be used in conjunction with <see cref="IStyleSelector"/> to take advantage of certain
    /// performance optimizations if the selector doesn't require hierarchy information.
    /// </summary>
    /// <param name="elements">The elements to filter/</param>
    /// <returns>An enumerable of filtered elements.</returns>
    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements);
    /// <summary>
    /// Whether the <see cref="Filter(IEnumerable{UIElement})"/> method can be used with this selector.
    /// </summary>
    /// <remarks>
    /// This property is needed by selectors which only know if they support flattened selection at 
    /// runtime, such as <see cref="Selectors.CompoundSelector"/>.
    /// </remarks>
    public bool SupportsFlattenedSelection { get; }
}

/// <summary>
/// The result of a <see cref="IStyleSelector.NeedsReevaluation(UIElement, string?, UIElementTreeChange, UIElementInputChange)"/>
/// check. Allows the style selector to tell the styling engine which (if any) elements might
/// need reevaluating for styling.
/// </summary>
public enum StyleSelectorUpdate
{
    /// <summary>
    /// No elements need to be re-styled.
    /// </summary>
    None,
    /// <summary>
    /// This is functionally very similar to <see cref="ChangedElement"/>, but it guarantees that no elements were UN-selected by the selector.
    /// </summary>
    AddedElement,
    /// <summary>
    /// The element which resulted in the <see cref="IStyleSelector.NeedsReevaluation(UIElement, string?, UIElementTreeChange, UIElementInputChange)"/>
    /// call being raised might require re-styling.
    /// </summary>
    ChangedElement,
    /// <summary>
    /// All elements controlled by this style might require re-styling.
    /// </summary>
    AllElements
}

/// <summary>
/// Represents a change in the tree of <see cref="UIElement"/>.
/// </summary>
public enum UIElementTreeChange
{
    /// <summary>
    /// No change occured to the hierarchy of <see cref="UIElement"/>.
    /// </summary>
    None,
    /// <summary>
    /// An element (and it's subtree) were added as a leaf to the current tree of <see cref="UIElement"/>.
    /// </summary>
    ElementAdded,
    /// <summary>
    /// An element (and it's subtree) were removed from the current tree of <see cref="UIElement"/>. The entire branch was pruned.
    /// </summary>
    ElementRemoved,
}

/// <summary>
/// Represents an abstracted input event received by a <see cref="UIElement"/>.
/// This is used by the styling system to determine when a selector need re-evaluating.
/// </summary>
public enum UIElementInputChange
{
    None,
    MouseHover,
    MousePress,
    KeyPress,
    Focus
}
