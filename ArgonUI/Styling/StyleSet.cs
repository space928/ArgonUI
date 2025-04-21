using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.UIElements;

namespace ArgonUI.Styling;

/// <summary>
/// A style set contains a number of stylable properties which can be applied to <see cref="UIElement"/> instances.
/// </summary>
public class StyleSet : IList<Style>
{
    private readonly List<Style> styles = [];
    private readonly List<UIElement> controlledElements = [];
    private readonly ReadOnlyCollection<UIElement> controlledElementsRO;

    public ReadOnlyCollection<UIElement> ControlledElements => controlledElementsRO;

    public int Count => styles.Count;
    public bool IsReadOnly => false;
    public Style this[int index] { get => styles[index]; set => styles[index] = value; }

    public StyleSet()
    {
        controlledElementsRO = new(controlledElements);
    }

    public StyleSet(IEnumerable<Style> styles) : this()
    {
        this.styles.AddRange(styles);
    }

    public StyleSet(Style style) : this()
    {
        this.styles.Add(style);
    }

    /// <summary>
    /// Registers a <see cref="UIElement"/> with this style, so that it can respond to style changes.
    /// <para/>
    /// This is done automatically when setting the <see cref="UIElement.Style"/> property.
    /// </summary>
    /// <param name="uiElement">The element to register.</param>
    public void Register(UIElement uiElement)
    {
        controlledElements.Add(uiElement);
        ApplyStyles(uiElement); // TODO: This needs to apply to all children as well...
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
        if (controlledElements.Remove(uiElement))
        {
            if (!applyParentStyle)
                return true;
            // Search up the hierarchy to see if we can apply any parent styles
            // Go all the way up the hierarchy and then apply styles on the way back down
            // TODO: All this gets complicated, what if each StylableProp stored a link to it's parent property
            // (must enforce lack of cycles!) to effectively make a big linked list so we don't have to search 
            // the element tree every time.
            List<UIElement> parents = [];

            var parent = uiElement;
            while ((parent = parent.Parent) != null)
                parents.Add(parent);

            for (int i = parents.Count - 1; i >= 0; i--)
                if (parents[i].Style is StyleSet parentStyle)
                    parentStyle.ApplyStyles(uiElement);

            return true;
        }
        return false;
    }

    /// <summary>
    /// Manually apply all of the styles in this style set to the given <see cref="UIElement"/>.
    /// Most of the time you shouldn't need to call this method.
    /// </summary>
    /// <param name="element">The element to apply styles to.</param>
    public void ApplyStyles(UIElement element)
    {
        foreach (var style in styles)
            style.ApplyStyle(element);
    }

    private void ApplyStyleToElements(Style style)
    {
        IEnumerable<UIElement> elements = controlledElements;
        if (style.Selector is IStyleSelector selector)
            elements = selector.Filter(controlledElements);

        foreach (var element in elements)
            style.ApplyStyle(element);
    }

    private void HandleStyleChange(Style style, IStylableProperty prop)
    {
        IEnumerable<UIElement> elements = controlledElements;
        if (style.Selector is IStyleSelector selector)
            elements = selector.Filter(controlledElements);

        foreach (var element in elements)
            prop.Apply(element);
    }

    public void Add(Style item)
    {
        styles.Add(item);
        ApplyStyleToElements(item);
        item.OnStyleChanged += HandleStyleChange;
        //item.OnReapplyParentStyles += 
    }

    public void Clear()
    {
        foreach (var style in styles)
            style.OnStyleChanged -= HandleStyleChange;
        styles.Clear();
        // TODO: Apply parent styles
    }

    public void Insert(int index, Style item)
    {
        styles.Insert(index, item);
        ApplyStyleToElements(item);
        item.OnStyleChanged += HandleStyleChange;
    }

    public bool Remove(Style item)
    {
        var res = styles.Remove(item);
        if (res)
            item.OnStyleChanged -= HandleStyleChange;
        // TODO: Apply parent + this styles
        return res;
    }

    public void RemoveAt(int index)
    {
        var style = styles[index];
        styles.RemoveAt(index);
        style.OnStyleChanged -= HandleStyleChange;
        // TODO: Apply parent + this styles
    }

    public bool Contains(Style item) => styles.Contains(item);
    public void CopyTo(Style[] array, int arrayIndex) => styles.CopyTo(array, arrayIndex);
    public int IndexOf(Style item) => styles.IndexOf(item);

    IEnumerator IEnumerable.GetEnumerator() => styles.GetEnumerator();
    public IEnumerator<Style> GetEnumerator() => styles.GetEnumerator();
}
