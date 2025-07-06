using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Helpers;

/// <summary>
/// Represents a very simple, non thread-safe pool of objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ObjectPool<T>
    where T : class, new()
{
    private const int MAX_POOLED_ITEMS = 128;
    private static readonly Stack<T> pooledObjects = [];

    public static Action<T>? factoryMethod;

    public static T Rent()
    {
        if (pooledObjects.TryPop(out var res))
            return res;

        return new();
    }

    public static void Return(T obj)
    {
        if (pooledObjects.Count < MAX_POOLED_ITEMS)
            pooledObjects.Push(obj);
    }
}
