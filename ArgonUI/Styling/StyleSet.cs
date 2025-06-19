using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.Styling.Selectors;
using ArgonUI.UIElements;

namespace ArgonUI.Styling;

/// <summary>
/// A style set contains a number of stylable properties which can be applied to <see cref="UIElement"/> instances.
/// </summary>
public partial class StyleSet : IList<Style>
{
    private readonly List<Style> styles = [];
    /// <summary>
    /// This dictionary stores a reference to all UIElements which have registered this style set.
    /// IE: All UIElements with <see cref="UIElement.Style"/> set to this instance.
    /// </summary>
    private readonly HashSet<UIElement> registeredElements = [];
    //private readonly List<RegisteredElement> registeredElements = [];
    //private readonly ReadOnlyCollection<RegisteredElement> registeredElementsRO;

    public IReadOnlyCollection<UIElement> RegisteredElements => registeredElements;//registeredElementsRO;

    public int Count => styles.Count;
    public bool IsReadOnly => false;
    public Style this[int index] { get => styles[index]; set => styles[index] = value; }

    public StyleSet()
    {
        //registeredElementsRO = new(registeredElements);
    }

    public StyleSet(IEnumerable<Style> styles) : this()
    {
        foreach (var style in styles)
            Add(style);
    }

    public StyleSet(Style style) : this()
    {
        Add(style);
    }

    /// <summary>
    /// Registers a <see cref="UIElement"/> with this style, so that it can respond to style changes.
    /// <para/>
    /// This is done automatically when setting the <see cref="UIElement.Style"/> property.
    /// </summary>
    /// <param name="uiElement">The element to register.</param>
    public void Register(UIElement uiElement)
    {
        registeredElements.Add(uiElement);
        uiElement.ChildElementChanged += HandleChildElementChanged;
        uiElement.ChildPropertyChanged += HandleElementPropertyChanged;
        uiElement.PropertyChanged += HandleElementPropertyChanged;
        ApplyStyles(uiElement);
    }

    /// <summary>
    /// Unregisters a <see cref="UIElement"/> from this style.
    /// <para/>
    /// This is done automatically when setting the <see cref="UIElement.Style"/> property.
    /// </summary>
    /// <param name="uiElement">The element to unregister.</param>
    /// <param name="applyParentStyle">Whether any styles set on the parents should be applied when unregistering this element.</param>
    public bool Unregister(UIElement uiElement, bool applyParentStyle = true)
    {
        if (!registeredElements.Remove(uiElement))
            return false;

        uiElement.ChildElementChanged -= HandleChildElementChanged;
        uiElement.ChildPropertyChanged -= HandleElementPropertyChanged;
        uiElement.PropertyChanged -= HandleElementPropertyChanged;

        if (applyParentStyle)
            ApplyParentStyles(uiElement);

        return true;
    }

    private void HandleChildElementChanged(UIElement target, UIElementTreeChange treeChange)
    {
        foreach (var style in styles)
        {
            var res = style.NeedsReevaluation(target, null, treeChange);
            switch (res)
            {
                case StyleSelectorUpdate.ChangedElement:
                    style.ApplyStyle(target);
                    break;
                case StyleSelectorUpdate.AllElements:
                    foreach (var registered in registeredElements)
                        style.ApplyStyle(registered);
                    break;
            }
        }
    }

    private void HandleElementPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        HandleElementPropertyChanged((sender as UIElement)!, eventArgs.PropertyName);
    }

    private void HandleElementPropertyChanged(UIElement target, string? propertyName)
    {
        foreach (var style in styles)
        {
            var res = style.NeedsReevaluation(target, propertyName, UIElementTreeChange.None);
            switch (res)
            {
                case StyleSelectorUpdate.ChangedElement:
                    style.ApplyStyle(target, true, propertyName?.GetHashCode());
                    break;
                case StyleSelectorUpdate.AllElements:
                    int? hash = propertyName?.GetHashCode();
                    foreach (var registered in registeredElements)
                        style.ApplyStyle(registered, true, hash);
                    break;
            }
        }
    }

    private static void ApplyParentStyles(UIElement uiElement, IStylableProperty? matchProp = null)
    {
        // Search up the hierarchy to see if we can apply any parent styles
        // Go all the way up the hierarchy and then apply styles on the way back down
        TemporaryList<UIElement> parents = [];

        // Find all parent styles
        var parent = uiElement;
        while ((parent = parent.Parent) != null)
            if (parent.Style != null)
                parents.Add(parent);

        // Apply them top-down
        for (int i = parents.Count - 1; i >= 1; i--)
            // Don't apply child styles yet, we only need to do that for the last parent style.
            parents[i].Style!.ApplyStyles(uiElement, false, matchProp);
        if (parents.Count >= 1)
            parents[0].Style!.ApplyStyles(uiElement, true, matchProp);
        parents.Dispose();
    }

    private static StyleSet? FindParentStyle(UIElement uiElement)
    {
        var parent = uiElement.Parent;
        while (parent != null)
        {
            if (parent.Style != null)
                return parent.Style;
            parent = parent.Parent;
        }
        return null;
    }

    /// <summary>
    /// Manually apply all of the styles in this style set to the given <see cref="UIElement"/>.
    /// Most of the time you shouldn't need to call this method.
    /// </summary>
    /// <param name="element">The element to apply styles to.</param>
    /// <param name="applyChildStyles">Whether <see cref="StyleSet"/> belonging to child elements 
    /// should be automatically re-applied</param>
    /// <param name="matchProp">Optionally, only apply style properties which match the name of the given property.</param>
    public void ApplyStyles(UIElement element, bool applyChildStyles = true, IStylableProperty? matchProp = null)
    {
        // Apply All styles in set to One object.
        foreach (var style in styles)
            style.ApplyStyle(element, applyChildStyles, matchProp?.NameHash);
    }

    private void ApplyStyleToElements(Style style)
    {
        // Apply One styles in set to All objects.
        foreach (var element in registeredElements)
            style.ApplyStyle(element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchProp">Optionally, only apply style properties which match the name of the given property.</param>
    private void ApplyParentStyles(IStylableProperty? matchProp = null)
    {
        foreach (var element in registeredElements)
            ApplyParentStyles(element, matchProp);
    }

    private void HandleStyleChange(Style style, IStylableProperty prop)
    {
        foreach (var element in registeredElements)
        {
            var selector = style.Selector ?? AllSelector.Shared;
            var selectedElements = selector.Filter(element);

            foreach (var selected in selectedElements)
                prop.Apply(selected);
        }
    }

    private void HandleStylePropRemoved(Style style, IStylableProperty? prop)
    {
        // TODO: It might be faster in some cases if we only re-apply styles to elements selected by this style.
        ApplyParentStyles(prop);
    }

    public void Add(Style item)
    {
        styles.Add(item);
        ApplyStyleToElements(item);
        item.OnStyleChanged += HandleStyleChange;
        item.OnStylePropRemoved += HandleStylePropRemoved;
    }

    public void Clear()
    {
        foreach (var style in styles)
        {
            style.OnStyleChanged -= HandleStyleChange;
            style.OnStylePropRemoved -= HandleStylePropRemoved;
        }
        styles.Clear();
        ApplyParentStyles();
    }

    public void Insert(int index, Style item)
    {
        styles.Insert(index, item);
        ApplyStyleToElements(item);
        item.OnStyleChanged += HandleStyleChange;
        item.OnStylePropRemoved += HandleStylePropRemoved;
    }

    public bool Remove(Style item)
    {
        var res = styles.Remove(item);
        if (res)
        {
            item.OnStyleChanged -= HandleStyleChange;
            item.OnStylePropRemoved -= HandleStylePropRemoved;
        }
        ApplyParentStyles();
        return res;
    }

    public void RemoveAt(int index)
    {
        var style = styles[index];
        styles.RemoveAt(index);
        style.OnStyleChanged -= HandleStyleChange;
        style.OnStylePropRemoved -= HandleStylePropRemoved;
        ApplyParentStyles();
    }

    public bool Contains(Style item) => styles.Contains(item);
    public void CopyTo(Style[] array, int arrayIndex) => styles.CopyTo(array, arrayIndex);
    public int IndexOf(Style item) => styles.IndexOf(item);

    IEnumerator IEnumerable.GetEnumerator() => styles.GetEnumerator();
    public IEnumerator<Style> GetEnumerator() => styles.GetEnumerator();

    public struct RegisteredElement : IEquatable<RegisteredElement>, IEquatable<UIElement>
    {
        public readonly UIElement element;
        public StyleSet? parent;

        public RegisteredElement(UIElement element, StyleSet? parent = null)
        {
            this.element = element;
            this.parent = parent;
        }

        public readonly override string ToString() => element?.ToString() ?? string.Empty;
        public readonly override int GetHashCode() => element?.GetHashCode() ?? 0;
#if NETSTANDARD
        public readonly override bool Equals(object? obj)
#else
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
#endif
        {
            if (obj == element)
                return true;
            if (obj is RegisteredElement registered)
                return registered.element == element;
            return false;
        }

        public readonly bool Equals(RegisteredElement other) => element == other.element;
        public readonly bool Equals(UIElement? other) => other == element;

        public static bool operator ==(RegisteredElement left, RegisteredElement right) => left.Equals(right);
        public static bool operator !=(RegisteredElement left, RegisteredElement right) => !(left == right);
    }
}
