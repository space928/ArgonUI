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
public class CompoundSelector : IStyleSelector, ICollection<IStyleSelector>
{
    private readonly List<IStyleSelector> selectors = [];

    public int Count => selectors.Count;
    public bool IsReadOnly => true;

    public CompoundSelector() { }

    public CompoundSelector(IEnumerable<IStyleSelector> selectors)
    {
        this.selectors.AddRange(selectors);
    }

    public CompoundSelector(params IStyleSelector[] selectors)
    {
        this.selectors.AddRange(selectors);
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        var ret = elements;
        // Apply each of the selectors successively to the sequence
        foreach (var selector in selectors)
            ret = selector.Filter(ret);
        return ret;
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
