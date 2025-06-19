using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which filters elements out unless they match all of the contained selectors.
/// </summary>
public class CompoundSelector : IStyleSelector, IFlattenedStyleSelector, ICollection<IStyleSelector>
{
    private readonly List<IStyleSelector> selectors;
    private readonly bool canUseFlattened;

    public int Count => selectors.Count;
    public bool IsReadOnly => true;

    public bool SupportsFlattenedSelection => canUseFlattened;

    public CompoundSelector() 
    {
        selectors = [];
    }

    public CompoundSelector(IEnumerable<IStyleSelector> selectors)
    {
        this.selectors = new(selectors);
        canUseFlattened = this.selectors.All(x => x is IFlattenedStyleSelector);
    }

    public CompoundSelector(params IStyleSelector[] selectors)
    {
        this.selectors = new(selectors);
        canUseFlattened = this.selectors.All(x => x is IFlattenedStyleSelector);
    }

    public IEnumerable<UIElement> Filter(UIElement elementTree)
    {
        if (selectors.Count == 0)
            return AllSelector.SelectAll(elementTree);
        if (canUseFlattened)
            return Filter(AllSelector.SelectAll(elementTree));

        // Apply each of the selectors successively to the sequence
        var selected = selectors[0].Filter(elementTree);
        for (int i = 1; i < selectors.Count; i++)
            selected = selected.Intersect(selectors[i].Filter(elementTree));
        
        return selected;
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        var ret = elements;
        // Apply each of the selectors successively to the sequence
        foreach (var selector in selectors)
            ret = ((IFlattenedStyleSelector)selector).Filter(ret);
        return ret;
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange)
    {
        // Higher values mean more needs updating, so simply return the maximum of the selectors.
        return (StyleSelectorUpdate)selectors.Max(x=>(int)x.NeedsReevaluation(target, propertyName, treeChange));
    }

    public bool Contains(IStyleSelector item) => selectors.Contains(item);
    public void CopyTo(IStyleSelector[] array, int arrayIndex) => selectors.CopyTo(array, arrayIndex);
    public IEnumerator<IStyleSelector> GetEnumerator() => selectors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => selectors.GetEnumerator();

    public bool Remove(IStyleSelector item) => throw new NotSupportedException();
    public void Add(IStyleSelector item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();

    public override string ToString()
    {
        var children = string.Join("; ", selectors.Select(x => x.ToString()));
        return $"[Compound: ({children})]";
    }
}
