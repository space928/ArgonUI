using ArgonUI.Helpers;
using ArgonUI.Styling.Selectors;
using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ArgonUI.Styling;

/// <summary>
/// A collection of styled properties controlled by a single selector.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(StyleDebugView))]
public class Style : ICollection<IStylableProperty>, IReadOnlyDictionary<string, IStylableProperty>, INotifyStyleChanged
{
    //private readonly List<IStylableProperty> props = [];
    private readonly StringDict<IStylableProperty> props = [];
    private IStyleSelector? selector;

    public IStyleSelector? Selector
    {
        get => selector;
        set
        {
            if (selector != null)
                selector.RequestReevaluation -= NotifyStyleSelectorChanged;
            if (value != null)
                value.RequestReevaluation += NotifyStyleSelectorChanged;
            selector = value;
            NotifyStyleSelectorChanged();
        }
    }
    public IStylableProperty this[string propName] { get => props[propName]; set => props[propName] = value; }
    public IStylableProperty this[ReadOnlySpan<char> propName] { get => props[propName]; set => props[propName] = value; }

    public int Count => props.Count;
    public bool IsReadOnly => false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<string> Keys => props.Keys;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<IStylableProperty> Values => props.Values;

    public event Action<Style, IStylableProperty>? OnStyleChanged;
    public event Action<Style, IStylableProperty?>? OnStylePropRemoved;
    public event Action<Style>? OnStyleSelectorChanged;

    /// <summary>
    /// A pretty printed string representation of this style.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string DebuggerDisplay => ToString(true);

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
    /// <param name="reapplyChildStyles">Whether StyleSets belonging to 
    /// child elements should be reapplied after this style. Important note: 
    /// only styles belonging to children of <paramref name="element"/> will 
    /// be re-applied. This is noteworthy because the <paramref name="element"/> 
    /// being styled by this method may be a child of the element to which this 
    /// style actually belongs. Additionally, only the properties of the child 
    /// style in common with this style willl be applied.</param>
    /// <param name="matchProp">Optionally, only apply style properties 
    /// which match the names of the given properties.</param>
    public void ApplyStyle(UIElement element, bool reapplyChildStyles = true, IEnumerable<string>? matchProp = null)
    {
        /*
         Root --> Style[Colour(red)]
         |-> Rect1
         |-> Rect2
             |-> Rect3
             |-> Rect4 --> Style[Colour(green)]
                 |-> Rect5
                 |-> Rect6 --> Style[Colour(blue)]
                     |-> Rect7

         If (this==Root.Style), and (element==Rect4):
         then we should apply red to Rect 4-7,
         then, we should re-apply green to 4-7,
         then, we should re-apply blue to 6-7

         This still needs to work in the case that, for instance, Rect6 isn't selected by the red style.
         But, if Rect2 has a style, it would NOT get re-applied by this method, to handle this case, use
         StyleSet.ReapplyParentStyles(), which calls this method internally for each parent style of 
         element.
         */
        // Memoize the selected props, as these get iterated multiple times, and passed to recursive calls.
        TemporaryList<IStylableProperty> selectedProps;
        if (matchProp == null)
        {
            selectedProps = new(props.Values);
        }
        else
        {
            // With interned strings, this should be fairly fast.
            selectedProps = new(props.GetValues(matchProp));
        }
        // Fast exit if no properties need to be applied.
        if (selectedProps.Count == 0)
        {
            selectedProps.Dispose();
            return;
        }

        element.window?.renderer.StartUITreeOperation();

        var elements = selector?.Filter(element) ?? AllSelector.SelectAll(element);
        // Apply all the properties in this style to all selected elements.
        foreach (var selected in elements)
        {
            foreach (var prop in selectedProps)
            {
#if DEBUG && DEBUG_PROP_UPDATES
                Debug.WriteLine($"[Style] Apply prop: {prop.Name}({prop.Value}) -> {selected} [From style: {this}]");
#endif
                prop.Apply(selected);
            }
        }

        // This may have overwritten the properties set by child styles, re-apply them now.
        if (reapplyChildStyles)
        {
            using TemporaryList<UIElement> stylesToReapply = [];
            // This selector might not select all elements with styles, so we need to be sure we select them anyway.
            foreach (var child in AllSelector.SelectAll(element))
            {
                if (child != element && child.Style != null)
                    stylesToReapply.Add(child);
            }
            // TODO: For efficiency, we should be able to prune to only the elements selected by our selector.
            foreach (var styledElem in stylesToReapply)
                styledElem.Style!.ApplyStyles(styledElem, true, SelectNames(selectedProps));
        }

        selectedProps.Dispose();
        element.window?.renderer.EndUITreeOperation();

        static IEnumerable<string> SelectNames(IEnumerable<IStylableProperty> props)
        {
            foreach (var prop in props)
                yield return prop.Name;
        }
    }

    protected void NotifyStyleChanged(IStylableProperty prop)
    {
        OnStyleChanged?.Invoke(this, prop);
    }

    protected void NotifyStylePropRemoved(IStylableProperty? prop)
    {
        OnStylePropRemoved?.Invoke(this, prop);
    }

    protected void NotifyStyleSelectorChanged(IStyleSelector? selector = null)
    {
        OnStyleSelectorChanged?.Invoke(this);
    }

    /// <inheritdoc cref="IStyleSelector.NeedsReevaluation(UIElement, string?, UIElementTreeChange, UIElementInputChange)"/>
    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        return Selector?.NeedsReevaluation(target, propertyName, treeChange, inputChange)
            ?? AllSelector.Shared.NeedsReevaluation(target, propertyName, treeChange, inputChange);
    }

    public void Add(IStylableProperty item)
    {
        if (!props.TryAdd(item.Name, item))
            throw new ArgumentException("Style already contains a property with the same name!");
        item.OnStylablePropChanged += NotifyStyleChanged;
        NotifyStyleChanged(item);
    }

    /// <summary>
    /// Adds a number of stylable properties to this style at once.
    /// </summary>
    /// <param name="items">The properties to add to the style.</param>
    public void AddRange(IEnumerable<IStylableProperty> items)
    {
        if (items is ICollection itemsCollection)
            props.EnsureCapacity(props.Capacity + itemsCollection.Count);
        foreach (var prop in items)
        {
            if (!props.TryAdd(prop.Name, prop))
                throw new ArgumentException($"Style already contains a property with the same name. (property name: '{prop.Name}')");
            prop.OnStylablePropChanged += NotifyStyleChanged;
            NotifyStyleChanged(prop);
        }
    }

    public void Clear()
    {
        foreach (var prop in props.Values)
            prop.OnStylablePropChanged -= NotifyStyleChanged;
        NotifyStylePropRemoved(null);
        props.Clear();
    }

    public bool Remove(IStylableProperty item)
    {
        bool res = props.Remove(item.Name);
        if (res)
        {
            item.OnStylablePropChanged -= NotifyStyleChanged;
            NotifyStylePropRemoved(item);
        }
        return res;
    }

    /// <summary>
    /// Removes the property with the given name from this style.
    /// </summary>
    /// <param name="propName">The name of the property to remove.</param>
    /// <returns><see langword="true"/> if the property was successfully removed.</returns>
    public bool Remove(string propName)
    {
        bool res = props.Remove(propName, out var item);
        if (res)
        {
            item.OnStylablePropChanged -= NotifyStyleChanged;
            NotifyStylePropRemoved(item);
        }
        return res;
    }

    public bool ContainsKey(string key) => props.ContainsKey(key);
    public bool Contains(IStylableProperty item) => props.ContainsKey(item.Name);
    public void CopyTo(IStylableProperty[] array, int arrayIndex) => props.Values.CopyTo(array, arrayIndex);

#if !NETSTANDARD
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out IStylableProperty value)
#else
    public bool TryGetValue(string key, out IStylableProperty value)
#endif
    {
        return ((IDictionary<string, IStylableProperty>)props).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => props.GetEnumerator();
    public IEnumerator<IStylableProperty> GetEnumerator() => props.Values.GetEnumerator();
    IEnumerator<KeyValuePair<string, IStylableProperty>> IEnumerable<KeyValuePair<string, IStylableProperty>>.GetEnumerator() => props.GetEnumerator();

    /// <summary>
    /// Returns a pretty-printed string representation of this style and it's properties.
    /// </summary>
    /// <param name="compact">Whether a more compact, single line representation should be created.</param>
    /// <returns></returns>
    public string ToString(bool compact)
    {
        StringBuilder sb = new("Style [");
        if (compact)
        {
            int i = 0;
            int last = props.Count - 1;
            foreach (var prop in props.Values)
            {
                sb.Append(prop.Name).Append('(').Append(prop.Value);
                if (i != last)
                    sb.Append("), ");
                else
                    sb.Append(')');
                i++;
            }
        }
        else
        {
            sb.Append("  Selector: ").Append(selector?.ToString() ?? "All");
            int i = 0;
            int last = props.Count - 1;
            foreach (var prop in props.Values)
            {
                sb.Append("  ").Append(prop.Name).Append('(').Append(prop.Value);
                if (i != last)
                    sb.Append("), ");
                else
                    sb.Append(')');
                i++;
            }
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override string ToString()
    {
        return ToString(true);
    }

    private class StyleDebugView(Style style)
    {
        private Style style = style;

        public IStyleSelector? Selector => style.Selector;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ICollection<IStylableProperty> Properties => style.props.Values;
    }
}
