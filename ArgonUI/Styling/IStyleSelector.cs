using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public interface IStyleSelector
{
    /// <summary>
    /// Filters an enumerable of ui elements, returning the ones which match the selector.
    /// </summary>
    /// <param name="elements">The elements to filter.</param>
    /// <returns>An enumerable of filtered elements.</returns>
    public IEnumerable<UIElement> Filter(IEnumerable<UIElement> elements);
}
