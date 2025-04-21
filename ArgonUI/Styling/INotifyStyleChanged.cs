using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public interface INotifyStyleChanged
{
    /// <summary>
    /// Notifies the subscriber that a given property in the style has changed and needs to be reapplied to the styled elements.
    /// </summary>
    public event Action<Style, IStylableProperty> OnStyleChanged;
    /// <summary>
    /// Notifies the subscriber that the styled properties in the parent styles with the matching name hashes must be reapplied to the styled elements.
    /// </summary>
    public event Action<IEnumerable<int>>? OnReapplyParentStyles;
}
