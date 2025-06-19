using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArgonUI.Styling;

public class StylableProp<T> : IStylableProperty
{
    internal T value;
    // This allows us to do some magic so that the following usage works:
    //   style.Colour = new Vector4(0.5);
    //   style.Colour.Transition = ...
    //   // When we next update Colour we want the metadata (ie: the transition, etc...)
    //   // to be preserved
    //   style.Colour = Vector4.One;
    //   // We can still completely override the stylable by making anew one ourselves
    //   style.Colour = new Stylable<Vector4>(Vector4.One, transition);
    private readonly bool isImplicit;
    private Transition? transition;
    private Action<UIElement, IStylableProperty> applyFunc;
    // Used by the styling engine to optimise which properties need to be reapplied
    private int nameHash;

    // Used by the implicit conversion operator
    [ThreadStatic]
    private static StylableProp<T>? implicitProp;

    /// <summary>
    /// Constructs a new stylable property from a given value and assigns it's apply function.
    /// This constructor is intended to be used from a factory method.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="applyFunc"></param>
    /// <param name="nameHash"></param>
    public StylableProp(T value, Action<UIElement, IStylableProperty> applyFunc, int nameHash)
    {
        this.isImplicit = false;
        this.value = value;
        this.applyFunc = applyFunc;
        this.nameHash = nameHash;
    }

    /// <summary>
    /// Constructs an implicit StylableProp, the property has no apply function and so is invalid.
    /// </summary>
    /// <param name="value"></param>
    private StylableProp(T value)
    {
        this.isImplicit = true;
        this.value = value;
        applyFunc = default!;
    }

    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            OnStylablePropChanged?.Invoke(this);
            //onSet?.Start(ref this);
        }
    }

    public Transition? Transition { get => transition; set => transition = value; }
    public event Action<IStylableProperty>? OnStylablePropChanged;
    int IStylableProperty.NameHash => nameHash;

    /// <summary>
    /// Used internally by setters for <see cref="StylableProp{T}"/>.
    /// </summary>
    /// <example>
    /// class Button
    /// {
    ///     public StylableProp{Vector4} Background
    ///     {
    ///         get => background;
    ///         set => background.Update(in value);
    ///     }
    /// }
    /// </example>
    /// <param name="value"></param>
    public void Update(in StylableProp<T> value)
    {
        if (value.isImplicit)
        {
            if (isImplicit)
                throw new InvalidOperationException("Attempted to update the value of an implicit stylable property. " +
                    "Note that implicitly casting a value of type T to a StylableProp<T> is only valid in the " +
                    "context of updating an already existing StylableProp!");
            this.value = value.value;
        }
        else
        {
            this.value = value.value;
            this.transition = value.transition;
            this.applyFunc = value.applyFunc;
            this.nameHash = value.nameHash;
        }

        OnStylablePropChanged?.Invoke(this);
    }

    public object? GetValue() => value;

    public void SetValue(object? value)
    {
        if (value is T val)
            this.value = val;
        else
            throw new InvalidCastException();

        OnStylablePropChanged?.Invoke(this);
    }

    void IStylableProperty.Apply(UIElement target)
    {
        applyFunc(target, this);
    }

    /// <summary>
    /// This is a magic operator that allows a value of type <typeparamref name="T"/> to be 
    /// directly assigned to a property of type <see cref="StylableProp{T}"/>.
    /// This works by wrapping the value of type <typeparamref name="T"/> in a temporary 
    /// <see cref="StylableProp{T}"/> instance (in this case a thread-static instance to 
    /// save allocations)
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator StylableProp<T>(T value)
    {
        implicitProp ??= new(default!);
        implicitProp.value = value;
        return implicitProp;
    }

    public static explicit operator T(StylableProp<T> value) => value!.Value;
}
