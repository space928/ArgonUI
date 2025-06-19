using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A very simple selector that simply returns the intersection of the selection of two other selectors.
/// <para/>
/// For more complex chaining see the <see cref="CompoundSelector"/>.
/// </summary>
/// <param name="a"></param>
/// <param name="b"></param>
public readonly struct AndSelector(IStyleSelector a, IStyleSelector b) : IStyleSelector, IFlattenedStyleSelector
{
    private readonly IStyleSelector a = a;
    private readonly IStyleSelector b = b;
    private readonly IFlattenedStyleSelector? af = a as IFlattenedStyleSelector;
    private readonly IFlattenedStyleSelector? bf = b as IFlattenedStyleSelector;

    public readonly bool SupportsFlattenedSelection => af != null && bf != null;

    public readonly IEnumerable<UIElement> Filter(UIElement elementTree)
    {
        // Try to save walking the element tree twice if possible
        // This can be done if at least one of the selectors implements IFlattenedStyleSelector
        if (af != null)
        {
            //if (bf != null)
            //    return Filter(AllSelector.SelectAll(elementTree));
            return af.Filter(b.Filter(elementTree));
        } 
        else if (bf != null)
        {
            return bf.Filter(a.Filter(elementTree));
        }
        return a.Filter(elementTree).Intersect(b.Filter(elementTree));
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        if (af == null || bf == null)
            throw new InvalidOperationException("Flattened selection is only possible if both source selectors support flattened selection!");
        return bf.Filter(af.Filter(elements));
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange)
    {
        // Higher values mean more needs updating, so simply return the maximum of the two selectors.
        return (StyleSelectorUpdate)Math.Max((int)a.NeedsReevaluation(target, propertyName, treeChange), (int)b.NeedsReevaluation(target, propertyName, treeChange));
    }
}
