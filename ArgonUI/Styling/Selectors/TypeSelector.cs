using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which which matches all elements of any of the types specified.
/// </summary>
public class TypeSelector : IStyleSelector, ICollection<Type>
{
    private readonly HashSet<Type> types;

    public TypeSelector(IEnumerable<Type> types)
    {
        this.types = new(types);
    }

    public TypeSelector(params Type[] types) : this((IEnumerable<Type>)types) { }

    public int Count => types.Count;
    public bool IsReadOnly => false;

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        throw new NotImplementedException();
    }

    public void Add(Type item) => types.Add(item);
    public void Clear() => types.Clear();
    public bool Contains(Type item) => types.Contains(item);
    public void CopyTo(Type[] array, int arrayIndex) => types.CopyTo(array, arrayIndex);
    public bool Remove(Type item) => types.Remove(item);

    public override string ToString()
    {
        var children = string.Join(" | ", types.Select(x=>x.Name));
        return $"[Types: ({children})]";
    }

    public IEnumerator<Type> GetEnumerator() => types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => types.GetEnumerator();
}
