using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public interface IStylableProperty : INotifyStylablePropChanged
{
    public object? GetValue();
    public void SetValue(object? value);
    /// <summary>
    /// Applies this property to targeted UI Element.
    /// </summary>
    /// <param name="target"></param>
    internal void Apply(UIElement target);
    //internal Action<UIElement, IStylableProperty> ApplyFunc { get; }
    /// <summary>
    /// Returns the hash of the name of the styled property. Used to optimise the styling engine.
    /// </summary>
    internal int NameHash { get; }
}
