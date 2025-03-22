using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.SourceGenerator;

/// <summary>
/// Automatically generates a property for the annotated field which automatically raised 
/// PropertyChanged notifications when set.
/// </summary>
/// <remarks>
/// If a custom property name is not specified, the generated property will take the name of field
/// with the first letter capitalised, or prefixed with an underscore. IE: <c>int exampleProp</c> -> 
/// <c>int ExampleProp</c>, and <c>int OtherExample</c> -> <c>int _OtherExample</c>
/// </remarks>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class ReactiveAttribute : Attribute
{
    public ReactiveAttribute() { }

    /// <inheritdoc cref="ReactiveAttribute"/>
    /// <param name="propName">The name of the property to generate.</param>
    public ReactiveAttribute(string propName)
    {
        PropName = propName;
    }

    public string? PropName { get; }
}

/// <summary>
/// Allows a custom getter function to be used in a property generated using a <see cref="ReactiveAttribute"/>.
/// </summary>
/// <param name="getter">A function which returns the value of the property.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomGetAttribute(string getter) : Attribute
{
    public string Getter { get; } = getter;
}

/// <summary>
/// Allows a custom setter function to be used in a property generated using a <see cref="ReactiveAttribute"/>.
/// </summary>
/// <param name="setter">A method which takes as input the new value to assign to the property.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomSetAttribute(string setter) : Attribute
{
    public string Setter { get; } = setter;
}
