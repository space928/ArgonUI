using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArgonUI;

/// <summary>
/// Base class for any object which has bindable properties.
/// </summary>
public abstract class ReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propName">The property name. (Automatic when called from a property setter)</param>
    protected void OnChanged([CallerMemberName] string? propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    /// <summary>
    /// Updates the value of the property specified and raises the <see cref="PropertyChanging"/> 
    /// and <see cref="PropertyChanged"/> events.
    /// </summary>
    /// <remarks>
    /// Example:<br/>
    /// <c>
    /// public string Name
    /// {
    ///     get => name;
    ///     set => UpdateProperty(ref name, value);
    /// }
    /// </c>
    /// </remarks>
    /// <typeparam name="T">The type of the property being updated.</typeparam>
    /// <param name="prop">A reference to the backing field of the property.</param>
    /// <param name="val">The new value of the property.</param>
    /// <param name="propName">The property name. (Automatic when called from a property setter)</param>
    protected void UpdateProperty<T>(ref T prop, in T val, [CallerMemberName] string? propName = null)
    {
        PropertyChanging?.Invoke(this, new(propName));
        prop = val;
        PropertyChanged?.Invoke(this, new(propName));
    }
}
