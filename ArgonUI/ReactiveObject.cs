using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArgonUI.Helpers;

namespace ArgonUI;

/// <summary>
/// Base class for any object which has bindable properties.
/// </summary>
public abstract class ReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Invokes the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="propName">The property name. (Automatic when called from a property setter)</param>
    protected void OnChanging([CallerMemberName] string? propName = null)
    {
        var e = PropertyChangedArgsPool.RentChanging(propName);
        PropertyChanging?.Invoke(this, e);
        PropertyChangedArgsPool.Return(e);
    }

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propName">The property name. (Automatic when called from a property setter)</param>
    protected void OnChanged([CallerMemberName] string? propName = null)
    {
        var e = PropertyChangedArgsPool.RentChanged(propName);
        PropertyChanged?.Invoke(this, e);
        PropertyChangedArgsPool.Return(e);
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
        var e1 = PropertyChangedArgsPool.RentChanging(propName);
        var e2 = PropertyChangedArgsPool.RentChanged(propName);
        PropertyChanging?.Invoke(this, e1);
        prop = val;
        PropertyChanged?.Invoke(this, e2);
        PropertyChangedArgsPool.Return(e1);
        PropertyChangedArgsPool.Return(e2);
    }
}
