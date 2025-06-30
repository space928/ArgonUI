using System.Collections.Generic;
using System.ComponentModel;

namespace ArgonUI.Helpers;

/// <summary>
/// A helper class which provides <see cref="PropertyChangedEventArgs"/> and 
/// <see cref="PropertyChangingEventArgs"/> instances from a pool.
/// <para/>
/// Rented items must be returned and shouldn't be used after they are returned.
/// </summary>
public static class PropertyChangedArgsPool
{
    private static readonly Stack<ReusablePropertyChangedEventArgs> propChangedPool = [];
    private static readonly Stack<ReusablePropertyChangingEventArgs> propChangingPool = [];
    private const int MAX_POOLED_ITEMS = 128;

    public static ReusablePropertyChangedEventArgs RentChanged(string? propName)
    {
        if (propChangedPool.TryPop(out var res))
        {
            res.propertyName = propName;
            return res;
        }
        return new(propName);
    }

    public static ReusablePropertyChangingEventArgs RentChanging(string? propName)
    {
        if (propChangingPool.TryPop(out var res))
        {
            res.propertyName = propName;
            return res;
        }
        return new(propName);
    }

    public static void Return(ReusablePropertyChangedEventArgs e)
    {
        if (propChangedPool.Count < MAX_POOLED_ITEMS)
            propChangedPool.Push(e);
    }

    public static void Return(ReusablePropertyChangingEventArgs e)
    {
        if (propChangingPool.Count < MAX_POOLED_ITEMS)
            propChangingPool.Push(e);
    }
}

/// <summary>
/// A <see cref="PropertyChangedEventArgs"/> which can be pooled. It's only valid for the lifetime 
/// of a <see cref="ReactiveObject.PropertyChanged"/> invocation.
/// </summary>
public class ReusablePropertyChangedEventArgs : PropertyChangedEventArgs
{
    public ReusablePropertyChangedEventArgs(string? propertyName) : base(propertyName)
    { }

    internal string? propertyName;
    public override string? PropertyName => propertyName;
}

/// <summary>
/// A <see cref="PropertyChangingEventArgs"/> which can be pooled. It's only valid for the lifetime 
/// of a <see cref="ReactiveObject.PropertyChanging"/> invocation.
/// </summary>
public class ReusablePropertyChangingEventArgs : PropertyChangingEventArgs
{
    public ReusablePropertyChangingEventArgs(string? propertyName) : base(propertyName)
    { }

    internal string? propertyName;
    public override string? PropertyName => propertyName;
}
