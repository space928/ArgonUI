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
    /// Notifies the subscriber that the specified styled property has been removed from this style. The parent styles which 
    /// affect this property needs to be re-applied. If the stylable property in this event is <see langword="null"/> then
    /// all stylable properties were removed.
    /// </summary>
    public event Action<Style, IStylableProperty?> OnStylePropRemoved;
}
