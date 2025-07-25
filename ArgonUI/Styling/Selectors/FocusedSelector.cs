﻿using ArgonUI.Input;
using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ArgonUI.Styling.Selectors;

/// <summary>
/// A style selector which selects elements which currently have input focus.
/// </summary>
/// <param name="invert">Whether the behaviour of this selector should be inverted.</param>
public class FocusedSelector(bool invert = false) : IStyleSelector, IFlattenedStyleSelector
{
    private readonly bool invert = invert;

    /// <summary>
    /// When inverted, the focused selector selects all elements which do not have input focus.
    /// </summary>
    public bool IsInverted => invert;
    public bool SupportsFlattenedSelection => true;

    public event Action<IStyleSelector>? RequestReevaluation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool SelectElement(UIElement elementTree) => elementTree.IsFocused ^ invert;

    private IEnumerable<UIElement> Select(UIElement elementTree)
    {
        if (elementTree == null || !SelectElement(elementTree))
            yield break;
        yield return elementTree;
        if (elementTree is UIContainer container)
        {
            foreach (UIElement element in container.Children)
            {
                foreach (var child in Select(element))
                    yield return child;
            }
        }
    }

    public IEnumerable<UIElement> Filter(UIElement elementTree) => Select(elementTree);

    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements)
    {
        int lastSelectedDepth = int.MaxValue;
        foreach (var element in elements)
        {
            bool selected = SelectElement(element);
            if (selected)
            {
                lastSelectedDepth = element.TreeDepth;
                yield return element;
                continue;
            }

            int depth = element.TreeDepth;
            if (depth < lastSelectedDepth) // Parent of selected => reset the selected depth to prevent selecting cousins
                lastSelectedDepth = int.MaxValue;
            else if (depth > lastSelectedDepth) // Child of selected
                yield return element;
        }
    }

    public StyleSelectorUpdate NeedsReevaluation(UIElement target, string? propertyName, UIElementTreeChange treeChange, UIElementInputChange inputChange)
    {
        if (inputChange == UIElementInputChange.Focus)
            return StyleSelectorUpdate.ChangedElement;

        return AllSelector.TestReevaluation(target, propertyName, treeChange, inputChange);
    }
}
