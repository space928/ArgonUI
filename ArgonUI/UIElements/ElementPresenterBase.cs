using System;
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
public abstract class ElementPresenterBase : UIElement, IContainer
{
    private readonly ReadOnlyCollection<UIElement> childrenRO;
    private readonly List<UIElement> children;

    public ReadOnlyCollection<UIElement> Children => childrenRO;

    public virtual Vector4 InnerPadding { get; set; }
    public virtual bool ClipContents { get; set; }

    /// <summary>
    /// Gets or sets the UIElement contained in this element.
    /// </summary>
    public UIElement? Content
    {
        get => children.FirstOrDefault();
        set
        {
            if (value == null)
            {
                children.Clear();
            }
            else
            {
                if (children.Count == 0)
                    AddChild(value);
                else
                    children[0] = value;
            }
        }
    }

    public ElementPresenterBase()
    {
        children = [];
        childrenRO = new(children);
    }

    public void AddChild(UIElement child)
    {
        if (children.Count == 0)
            children.Add(child);
    }

    public void AddChildren(UIElement[] children)
    {
        foreach (UIElement child in children)
            AddChild(child);
    }

    public void InsertChild(UIElement child, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0);
        Content = child;
    }

    public void RemoveChild(UIElement child)
    {
        children.Remove(child);
    }

    public void RemoveChildren(UIElement[] children)
    {
        foreach (UIElement child in children)
            RemoveChild(child);
    }
}
