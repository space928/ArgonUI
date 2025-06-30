using ArgonUI.Helpers;
using ArgonUI.Styling.Selectors;
using ArgonUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ArgonUI.Styling;

/// <summary>
/// A style set contains a number of stylable properties which can be applied to <see cref="UIElement"/> instances.
/// </summary>
[DebuggerDisplay("StyleSet ({Count} styles, {registeredElements.Count} uses)")]
[DebuggerTypeProxy(typeof(StyleSetDebugView))]
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
        uiElement.OnStylableInputEvent += HandleElementInputChanged;
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
        uiElement.OnStylableInputEvent -= HandleElementInputChanged;

        if (applyParentStyle)
            ApplyParentStyles(uiElement, null, false);

        return true;
    }

    #region Event Handlers
    private void HandleChildElementChanged(UIElement target, UIElementTreeChange treeChange)
    {
        HandleElementChanged(target, null, treeChange, UIElementInputChange.None);
    }

    private void HandleElementPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        HandleElementChanged((sender as UIElement)!, eventArgs.PropertyName, UIElementTreeChange.None, UIElementInputChange.None);
    }

    private void HandleElementPropertyChanged(UIElement target, string? propertyName)
    {
        HandleElementChanged(target, propertyName, UIElementTreeChange.None, UIElementInputChange.None);
    }

    private void HandleElementInputChanged(UIElement target, UIElementInputChange inputChange)
    {
        HandleElementChanged(target, null, UIElementTreeChange.None, inputChange);
    }

    private void HandleElementChanged(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        target.window?.renderer.StartUITreeOperation();
        foreach (var style in styles)
        {
            var res = style.NeedsReevaluation(target, propertyName, treeChange, inputChange);
            switch (res)
            {
                case StyleSelectorUpdate.AddedElement:
                    style.ApplyStyle(target);
                    break;
                case StyleSelectorUpdate.ChangedElement:
                    // Since the selector might have de-selected elements we need to re-apply all parent styles (with matching props)
                    // (We also need to re-apply other styles within this style set)
                    ApplyParentStyles(target, style.Keys, true);
                    //style.ApplyStyle(target);
                    break;
                case StyleSelectorUpdate.AllElements:
                    ApplyParentStyles(style.Keys, true);
                    //ApplyStyleToElements(style);
                    break;
            }
        }
        target.window?.renderer.EndUITreeOperation();
    }

    private void HandleStyleChange(Style style, IStylableProperty prop)
    {
        foreach (var element in registeredElements)
        {
            element.window?.renderer.StartUITreeOperation();

            var selector = style.Selector ?? AllSelector.Shared;
            var selectedElements = selector.Filter(element);

            foreach (var selected in selectedElements)
                prop.Apply(selected);

            element.window?.renderer.EndUITreeOperation();
        }
    }

    private void HandleStylePropRemoved(Style style, IStylableProperty? prop)
    {
        // TODO: It might be faster in some cases if we only re-apply styles to elements selected by this style.
        ApplyParentStyles(prop?.Name?.AsOneEnumerable());
    }

    private void HandleStyleSelectorChanged(Style style)
    {
        // If the style selector changes, we need to invalidate any element which could have been selected by it.
        // Luckily we can filter this to only the properties controlled by the style.
        ApplyParentStyles(style.Keys);
    }
    #endregion

    #region Style Application Logic
    /// <summary>
    /// Applies parent styles to all registered elements.
    /// </summary>
    /// <param name="matchProp">Optionally, only apply style properties which match the names of the given properties.</param>
    /// <param name="includeSelf">Whether styles in this style set should also be re-applied.</param>
    private void ApplyParentStyles(IEnumerable<string>? matchProp = null, bool includeSelf = true)
    {
        foreach (var element in registeredElements)
            ApplyParentStyles(element, matchProp, includeSelf);
    }

    /// <summary>
    /// Applies all the styles belonging to the parents of the given <see cref="UIElement"/> and it's subtree.
    /// </summary>
    /// <param name="uiSubTree">The element subtree to apply styles to.</param>
    /// <param name="matchProp">Optionally, an enumerable of property names which should be applied; 
    /// when set to <see langword="null"/>, all properties are applied.</param>
    /// <param name="includeSelf">Whether <paramref name="uiSubTree"/>'s styles should be re-applied if no parent styles exist.</param>
    private static void ApplyParentStyles(UIElement uiSubTree, IEnumerable<string>? matchProp = null, bool includeSelf = true)
    {
        uiSubTree.window?.renderer.StartUITreeOperation();
        // Search up the hierarchy to see if we can apply any parent styles
        // Go all the way up the hierarchy and then apply styles on the way back down
        TemporaryList<UIElement> parents = [];

        if (includeSelf && uiSubTree.Style != null)
            parents.Add(uiSubTree);

        // Find all parent styles
        var parent = uiSubTree;
        while ((parent = parent.Parent) != null)
            if (parent.Style != null)
                parents.Add(parent);

        // Apply them top-down
        for (int i = parents.Count - 1; i >= 1; i--)
            // Don't apply child styles yet, we only need to do that for the last parent style.
            parents[i].Style!.ApplyStyles(uiSubTree, false, matchProp);
        if (parents.Count >= 1)
            parents[0].Style!.ApplyStyles(uiSubTree, true, matchProp);
        parents.Dispose();
        uiSubTree.window?.renderer.EndUITreeOperation();
    }

    /// <summary>
    /// Manually apply all of the styles in this style set to the given <see cref="UIElement"/>.
    /// Most of the time you shouldn't need to call this method.
    /// </summary>
    /// <param name="element">The element to apply styles to.</param>
    /// <param name="applyChildStyles">Whether <see cref="StyleSet"/> belonging to child elements 
    /// should be automatically re-applied</param>
    /// <param name="matchProp">Optionally, only apply style properties which match the names of the given properties.</param>
    public void ApplyStyles(UIElement element, bool applyChildStyles = true, IEnumerable<string>? matchProp = null)
    {
        // Apply All styles in set to One object.
        foreach (var style in styles)
            style.ApplyStyle(element, applyChildStyles, matchProp);
        // Technically, we could collect all the properties and selected elements from all the styles in
        // this set and only re-apply child styles once to these collected elements and properties. This
        // would likely be more complex/costly than it's worth though. Currently though, if two styles
        // in this set style the same property (and select the same elements) then, child styles for that
        // property will be re-applied more than once.
    }

    private void ApplyStyleToElements(Style style)
    {
        // Apply One style in set to All registered objects.
        foreach (var element in registeredElements)
            style.ApplyStyle(element, true, null);
    }
    #endregion

    #region ICollection Implementation
    public void Add(Style item)
    {
        styles.Add(item);
        ApplyStyleToElements(item);
        item.OnStyleChanged += HandleStyleChange;
        item.OnStylePropRemoved += HandleStylePropRemoved;
        item.OnStyleSelectorChanged += HandleStyleSelectorChanged;
    }

    public void Clear()
    {
        foreach (var style in styles)
        {
            style.OnStyleChanged -= HandleStyleChange;
            style.OnStylePropRemoved -= HandleStylePropRemoved;
        }
        styles.Clear();
        ApplyParentStyles(null, false);
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
    #endregion

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

    private class StyleSetDebugView(StyleSet styleSet)
    {
        private StyleSet styleSet = styleSet;

        public IReadOnlyCollection<UIElement> RegisteredElements => styleSet.RegisteredElements;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyCollection<Style> Styles => styleSet.styles;
    }
}
