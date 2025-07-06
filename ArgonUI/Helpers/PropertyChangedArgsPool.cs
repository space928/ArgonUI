using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace ArgonUI.Helpers;

/// <summary>
/// A helper class which provides <see cref="PropertyChangedEventArgs"/> and 
/// <see cref="PropertyChangingEventArgs"/> instances from a pool.
/// <para/>
/// Rented items must be returned and shouldn't be used after they are returned.
/// </summary>
public static class PropertyChangedArgsPool
{
    //private static readonly Stack<ReusablePropertyChangedEventArgs> propChangedPool = [];
    //private static readonly Stack<ReusablePropertyChangingEventArgs> propChangingPool = [];

    private static readonly ReusablePropertyChangedEventArgs?[] propChangedPool = new ReusablePropertyChangedEventArgs?[MAX_POOLED_ITEMS];
    private static readonly ReusablePropertyChangingEventArgs?[] propChangingPool = new ReusablePropertyChangingEventArgs?[MAX_POOLED_ITEMS];

    private static ReusablePropertyChangedEventArgs? propChangedInst;
    private static ReusablePropertyChangingEventArgs? propChangingInst;

    private const int MAX_POOLED_ITEMS = 128;

    public static ReusablePropertyChangedEventArgs RentChanged(string? propName)
    {
        var res = propChangedInst;
        if (res != null && res == Interlocked.CompareExchange(ref propChangedInst, null, res))
        {
            res.propertyName = propName;
            return res;
        }
        return RentChangedSlow(propName);
    }

    private static ReusablePropertyChangedEventArgs RentChangedSlow(string? propName)
    {
        for (int i = 0; i < propChangedPool.Length; i++)
        {
            var res = propChangedPool[i];
            if (res != null)
            {
                if (res == Interlocked.CompareExchange(ref propChangedPool[i], null, res))
                {
                    res.propertyName = propName;
                    return res;
                }
                break;
            }
        }
        return new(propName);
    }

    public static ReusablePropertyChangingEventArgs RentChanging(string? propName)
    {
        var res = propChangingInst;
        if (res != null && res == Interlocked.CompareExchange(ref propChangingInst, null, res))
        {
            res.propertyName = propName;
            return res;
        }
        return RentChangingSlow(propName);
    }

    private static ReusablePropertyChangingEventArgs RentChangingSlow(string? propName)
    {
        for (int i = 0; i < propChangingPool.Length; i++)
        {
            var res = propChangingPool[i];
            if (res != null)
            {
                if (res == Interlocked.CompareExchange(ref propChangingPool[i], null, res))
                {
                    res.propertyName = propName;
                    return res;
                }
                break;
            }
        }
        return new(propName);
    }

    public static void Return(ReusablePropertyChangedEventArgs e)
    {
        // No need to interlock here, worst case is we overwrite an existing pooled object, no big deal.
        if (propChangedInst == null)
        {
            propChangedInst = e;
            return;
        }

        ReturnSlow(e);
    }

    private static void ReturnSlow(ReusablePropertyChangedEventArgs e)
    {
        var items = propChangedPool;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = e;
                break;
            }
        }
    }

    public static void Return(ReusablePropertyChangingEventArgs e)
    {
        // No need to interlock here, worst case is we overwrite an existing pooled object, no big deal.
        if (propChangingInst == null)
        {
            propChangingInst = e;
            return;
        }

        ReturnSlow(e);
    }

    private static void ReturnSlow(ReusablePropertyChangingEventArgs e)
    {
        var items = propChangingPool;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = e;
                break;
            }
        }
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
