using ArgonUI.Helpers;
using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which matches elements which have all of the specified tags.
/// </summary>
public class TagSelector : IStyleSelector, IFlattenedStyleSelector, ICollection<string>
{
    private readonly ObservableStringSet tags;

    public event Action<IStyleSelector>? RequestReevaluation;

    public int Count => tags.Count;
    public bool IsReadOnly => false;
    public bool SupportsFlattenedSelection => true;

    public TagSelector(params string[] tags) : this((IEnumerable<string>)tags) { }

    public TagSelector(IEnumerable<string> tags)
    {
        this.tags = new(tags);
    }

    public IEnumerable<UIElement> Filter(UIElement elementTree)
    {
        return Filter(AllSelector.SelectAll(elementTree));
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        foreach (var element in elements)
        {
            if (element.Tags.IsSupersetOf(tags))
                yield return element;
        }
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        return treeChange switch
        {
            UIElementTreeChange.ElementAdded => StyleSelectorUpdate.AddedElement,
            UIElementTreeChange.ElementRemoved => StyleSelectorUpdate.None,
            _ => propertyName == nameof(UIElement.Tags) ? StyleSelectorUpdate.ChangedElement : StyleSelectorUpdate.None,
        };
    }

    public void Add(string item)
    {
        if (tags.Add(item))
            RequestReevaluation?.Invoke(this);
    }

    public void Clear()
    {
        tags.Clear();
        RequestReevaluation?.Invoke(this);
    }

    public bool Remove(string item)
    {
        var res = tags.Remove(item);
        if (res)
            RequestReevaluation?.Invoke(this);
        return res;
    }

    public bool Contains(string item) => tags.Contains(item);
    public void CopyTo(string[] array, int arrayIndex) => tags.CopyTo(array, arrayIndex);
    public IEnumerator<string> GetEnumerator() => tags.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => tags.GetEnumerator();

    public override string ToString()
    {
        var children = string.Join(" & ", tags);
        return $"[Tags: ({children})]";
    }
}
