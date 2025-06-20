﻿using ArgonUI.UIElements;
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

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange)
    {
        return treeChange switch
        {
            UIElementTreeChange.ElementAdded => StyleSelectorUpdate.ChangedElement,
            UIElementTreeChange.ElementRemoved => StyleSelectorUpdate.None,
            _ => propertyName == nameof(UIElement.Tags) ? StyleSelectorUpdate.ChangedElement : StyleSelectorUpdate.None,
        };
    }

    public void Add(string item) => tags.Add(item);
    public void Clear() => tags.Clear();
    public bool Contains(string item) => tags.Contains(item);
    public void CopyTo(string[] array, int arrayIndex) => tags.CopyTo(array, arrayIndex);
    public IEnumerator<string> GetEnumerator() => tags.GetEnumerator();
    public bool Remove(string item) => tags.Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => tags.GetEnumerator();

    public override string ToString()
    {
        var children = string.Join(" & ", tags);
        return $"[Tags: ({children})]";
    }
}
