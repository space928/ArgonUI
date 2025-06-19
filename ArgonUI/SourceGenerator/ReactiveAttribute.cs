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
/// <param name="inline">When <see langword="false"/>, the setter parameter expects a method name; 
/// when <see langword="true"/>, the getter parameter takes an expression which is placed inline in the generator getter.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomGetAttribute(string getter, bool inline = false) : Attribute
{
    public string Getter { get; } = getter;
    public bool Inline { get; } = inline;
}

/// <summary>
/// Allows a custom setter function to be used in a property generated using a <see cref="ReactiveAttribute"/>.
/// </summary>
/// <param name="setter">A method which takes as input the new value to assign to the property.</param>
/// <param name="inline">When <see langword="false"/>, the setter parameter expects a method name; 
/// when <see langword="true"/>, the setter parameter takes an expression which is placed inline in the generator setter.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomSetAttribute(string setter, bool inline = false) : Attribute
{
    public string Setter { get; } = setter;
    public bool Inline { get; } = inline;
}

/// <summary>
/// Allows custom accessibility keywords to be used on a property generated using a <see cref="ReactiveAttribute"/>.
/// </summary>
/// <example>
/// [Reactive, CustomAccessibility("protected virtual")] string prop;
/// </example>
/// <param name="accessibilityModifiers">The accessibility modifiers to add to the generated property.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CustomAccessibilityAttribute(string accessibilityModifiers) : Attribute
{
    public string AccessibilityModifiers { get; } = accessibilityModifiers;
}
