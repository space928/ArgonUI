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

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        return TestReevaluation(target, propertyName, treeChange, inputChange);
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        return elements;
    }


    public static IEnumerable<UIElement> SelectAll(UIElement elementTree)
    {
        if (elementTree == null)
            yield break;
        yield return elementTree;
        // Depth first selection of all children. The order here is important so that flattened selectors work correctly.
        if (elementTree is UIContainer container)
        {
            foreach (UIElement element in container.Children)
            {
                foreach (var child in SelectAll(element))
                    yield return child;
            }
        }
    }

    /// <summary>
    /// Checks if a selector which selectors all elements would need to be re-evaluated given the parameters passed in.
    /// <para/>
    /// If an element was added to the tree, it needs to be re-styled. In any other case, do nothing.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="propertyName"></param>
    /// <param name="treeChange"></param>
    /// <param name="inputChange"></param>
    /// <returns></returns>
    public static StyleSelectorUpdate TestReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        // If an element was added to the tree, it needs to be re-styled. In any other case, do nothing.
        return treeChange == UIElementTreeChange.ElementAdded ? StyleSelectorUpdate.AddedElement : StyleSelectorUpdate.None;
    }
}
