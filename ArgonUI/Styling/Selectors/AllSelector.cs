using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which selects all elements in a tree.
/// </summary>
public class AllSelector : IStyleSelector, IFlattenedStyleSelector
{
    public static readonly AllSelector Shared = new();

    public event Action<IStyleSelector>? RequestReevaluation;
    public bool SupportsFlattenedSelection => true;

    public IEnumerable<UIElement> Filter(UIElement elementTree)
    {
        return SelectAll(elementTree);
    }

    public static IEnumerable<UIElement> SelectAll(UIElement elementTree)
    {
        if (elementTree == null)
            yield break;
        yield return elementTree;
        if (elementTree is UIContainer container)
        {
            foreach (UIElement element in container.Children)
            { 
                foreach (var child in SelectAll(element))
                    yield return child;
            }
        }
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange)
    {
        // If an element was added to the tree, it needs to be re-styled. In any other case, do nothing.
        return treeChange == UIElementTreeChange.ElementAdded ? StyleSelectorUpdate.AddedElement : StyleSelectorUpdate.None;
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        return elements;
    }
}
