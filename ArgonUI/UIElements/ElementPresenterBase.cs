﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

/// <summary>
/// Abstract class for a UIElement which contains a single child element.
/// </summary>
public abstract class ElementPresenterBase : UIContainer
{
    private readonly OneReadOnlyList<UIElement> childList = [];

    public override IReadOnlyList<UIElement> Children => childList;

    //public virtual Vector4 InnerPadding { get; set; }
    //public virtual bool ClipContents { get; set; }

    /// <summary>
    /// Gets or sets the UIElement contained in this element.
    /// </summary>
    public UIElement? Content
    {
        get => childList.value;
        set
        {
            if (value == null)
            {
                var child = childList.value;
                childList.value = null;
                if (child != null)
                {
                    child.Parent = null;
                    OnChildElementChanged(child, Styling.UIElementTreeChange.ElementRemoved);
                }
            }
            else
            {
                var old = childList.value;
                childList.value = value;
                value.Parent = this;

                if (old != null)
                {
                    old.Parent = null;
                    OnChildElementChanged(old, Styling.UIElementTreeChange.ElementRemoved);
                }
                OnChildElementChanged(value, Styling.UIElementTreeChange.ElementAdded);
            }
        }
    }

    public override void AddChild(UIElement child)
    {
        if (childList.value == null)
            throw new InvalidOperationException("Can't add more than one element to an ElementPresenter. " +
                "Consider wrapping the elements to add in another container element.");
        childList.value = child;
        child.Parent = this;
        OnChildElementChanged(child, Styling.UIElementTreeChange.ElementAdded);
    }

    public override void AddChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
        {
            AddChild(child);
        }
    }

    public override void InsertChild(UIElement child, int index)
    {
        if (index != 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Insertion index must be 0");
        Content = child;
    }

    public override bool RemoveChild(UIElement child)
    {
        if (child != null && childList.value == child)
        {
            child.Parent = null;
            OnChildElementChanged(child, Styling.UIElementTreeChange.ElementRemoved);
            return true;
        }
        return false;
    }

    public override void RemoveChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
            RemoveChild(child);
    }

    public override void ClearChildren()
    {
        var child = childList.value;
        childList.value = null;
        if (child != null)
        {
            child.Parent = null;
            OnChildElementChanged(child, Styling.UIElementTreeChange.ElementRemoved);
        }
    }
}

public class OneReadOnlyList<T> : IReadOnlyList<T>
{
    public T? value;

    public T this[int index] => value!;
    public int Count => 1;

    public IEnumerator<T> GetEnumerator()
    {
        yield return value!;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
