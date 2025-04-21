using ArgonUI.Styling.Selectors;
using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgonUI.Styling;

/// <summary>
/// A collection of styled properties controlled by a single selector.
/// </summary>
public class Style : IList<IStylableProperty>, INotifyStyleChanged
{
    private readonly List<IStylableProperty> props = [];
    private IStyleSelector? selector;

    // TODO: Selectors need to implement property change notifications
    public IStyleSelector? Selector
    {
        get => selector;
        set
        {
            selector = value;
            NotifyStyleChanged();
        }
    }
    public IStylableProperty this[int index] { get => props[index]; set => props[index] = value; }

    public int Count => props.Count;
    public bool IsReadOnly => false;
    public event Action<Style, IStylableProperty>? OnStyleChanged;
    public event Action<IEnumerable<int>>? OnReapplyParentStyles;

    public Style()
    {

    }

    public Style(IEnumerable<IStylableProperty> props)
    {
        AddRange(props);
    }

    public Style(params IStylableProperty[] props)
    {
        AddRange(props);
    }

    public Style(IStyleSelector? styleSelector, IEnumerable<IStylableProperty> props) : this(props)
    {
        this.Selector = styleSelector;
    }

    public Style(IStyleSelector? styleSelector, params IStylableProperty[] props) : this(props)
    {
        this.Selector = styleSelector;
    }

    public Style(IEnumerable<string> tagSelectors, IEnumerable<IStylableProperty> props) : this(props)
    {
        this.Selector = new TagSelector(tagSelectors);
    }

    public Style(IEnumerable<Type> typeSelectors, IEnumerable<IStylableProperty> props) : this(props)
    {
        this.Selector = new TypeSelector(typeSelectors);
    }

    public Style(IEnumerable<Type> typeSelectors, IEnumerable<string> tagSelectors, IEnumerable<IStylableProperty> props) : this(props)
    {
        this.Selector = new CompoundSelector(
            new TypeSelector(typeSelectors),
            new TagSelector(tagSelectors));
    }

    /// <summary>
    /// Manually apply all of the properties in this style to the given <see cref="UIElement"/>.
    /// Most of the time you shouldn't need to call this method.
    /// </summary>
    /// <param name="element">The element to apply styles to.</param>
    public void ApplyStyle(UIElement element)
    {
        if (selector?.Filter(new OneEnumerable<UIElement>(element)).Any() ?? true)
        {
            foreach (var prop in props)
                prop.Apply(element);
        }
    }

    private void NotifyStyleChanged(IStylableProperty prop)
    {
        OnStyleChanged?.Invoke(this, prop);
    }

    private void NotifyStyleChanged()
    {
        foreach (var prop in props)
            OnStyleChanged?.Invoke(this, prop);
    }

    public void Add(IStylableProperty item)
    {
        props.Add(item);
        item.OnStylablePropChanged += NotifyStyleChanged;
        NotifyStyleChanged(item);
    }

    public void AddRange(IEnumerable<IStylableProperty> items)
    {
        props.AddRange(items);
        foreach (var prop in items)
        {
            prop.OnStylablePropChanged += NotifyStyleChanged;
            NotifyStyleChanged(prop);
        }
    }

    public void Clear()
    {
        foreach (var prop in props)
            prop.OnStylablePropChanged -= NotifyStyleChanged;
        OnReapplyParentStyles?.Invoke(props.Select(x=>x.NameHash));
        props.Clear();
    }

    public void Insert(int index, IStylableProperty item)
    {
        props.Insert(index, item);
        item.OnStylablePropChanged += NotifyStyleChanged;
        NotifyStyleChanged(item); // TODO: Also apply all subsequant rules with matching names to ensure ordering
        var target = item.NameHash;
        for (int i = index + 1; i < props.Count; i++)
        {
            var p = props[i];
            if (p.NameHash == target)
                NotifyStyleChanged(p);
        }
    }

    public bool Remove(IStylableProperty item)
    {
        bool res = props.Remove(item);
        if (res)
        {
            item.OnStylablePropChanged -= NotifyStyleChanged;
            OnReapplyParentStyles?.Invoke(new OneEnumerable<int>(item.NameHash));
        }
        return res;
    }

    public void RemoveAt(int index)
    {
        var item = props[index];
        props.RemoveAt(index);
        item.OnStylablePropChanged -= NotifyStyleChanged;
        OnReapplyParentStyles?.Invoke(new OneEnumerable<int>(item.NameHash));
    }

    public bool Contains(IStylableProperty item) => props.Contains(item);
    public void CopyTo(IStylableProperty[] array, int arrayIndex) => props.CopyTo(array, arrayIndex);
    public int IndexOf(IStylableProperty item) => props.IndexOf(item);

    IEnumerator IEnumerable.GetEnumerator() => props.GetEnumerator();
    public IEnumerator<IStylableProperty> GetEnumerator() => props.GetEnumerator();
}
