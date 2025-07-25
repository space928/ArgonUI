﻿using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which which matches all elements of any of the types specified.
/// </summary>
public class TypeSelector : IStyleSelector, IFlattenedStyleSelector, ICollection<Type>
{
    private readonly HashSet<Type> types;

    public event Action<IStyleSelector>? RequestReevaluation;

    public int Count => types.Count;
    public bool IsReadOnly => false;
    public bool SupportsFlattenedSelection => true;

    public TypeSelector(IEnumerable<Type> types)
    {
        this.types = new(types);
    }

    public TypeSelector(params Type[] types) : this((IEnumerable<Type>)types) { }

    public IEnumerable<UIElement> Filter(UIElement elementTree)
    {
        return Filter(AllSelector.SelectAll(elementTree));
    }

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        foreach (var element in elements)
        {
            if (types.Contains(element.GetType()))
                yield return element;
        }
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        // If an element was added to the tree, it might need to be re-styled. In any other case, do nothing.
        return treeChange == UIElementTreeChange.ElementAdded ? StyleSelectorUpdate.AddedElement : StyleSelectorUpdate.None;
    }

    public void Add(Type item)
    {
        if (types.Add(item))
            RequestReevaluation?.Invoke(this);
    }

    public void Clear()
    {
        types.Clear();
        RequestReevaluation?.Invoke(this);
    }

    public bool Remove(Type item)
    {
        var res = types.Remove(item);
        if (res)
            RequestReevaluation?.Invoke(this);
        return res;
    }

    public bool Contains(Type item) => types.Contains(item);
    public void CopyTo(Type[] array, int arrayIndex) => types.CopyTo(array, arrayIndex);

    public override string ToString()
    {
        var children = string.Join(" | ", types.Select(x=>x.Name));
        return $"[Types: ({children})]";
    }

    public IEnumerator<Type> GetEnumerator() => types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => types.GetEnumerator();
}
