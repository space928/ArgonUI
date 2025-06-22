using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public interface IStylableProperty : INotifyStylablePropChanged
{
    /// <summary>
    /// Gets or sets the value of this property.
    /// </summary>
    /// <remarks>
    /// To avoid the (small) performance penalty of boxing and unboxing value types, consider 
    /// using <see cref="StylableProp{T}.Value"/> instead. This can be done by casting this 
    /// <see cref="IStylableProperty"/> to the appropriate <see cref="StylableProp{T}"/>
    /// and then getting/setting it's <see cref="StylableProp{T}.Value"/> property.
    /// </remarks>
    public object? Value { get; set; }
    /// <summary>
    /// Applies this property to targeted UI Element.
    /// </summary>
    /// <param name="target"></param>
    internal void Apply(UIElement target);
    /// <summary>
    /// Returns the name of the styled property. For optimisation reasons, this string MUST be 
    /// interned (see: <see cref="string.Intern(string)"/>).
    /// </summary>
    public string Name { get; }
}
