using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.SourceGenerator;

/// <summary>
/// Marks the annotated field as a StylableProperty and generates a factory method to create a 
/// <see cref="Styling.StylableProp{T}"/> for this property such that it can be usedd in styles.
/// Properties which share names between different UIElement types in a style are consolidated, 
/// hence stylable properties sharing the same name must also be of the same type.
/// <para/>
/// For instance if a UIElement of type <c>Button</c> defines a stylable property 
/// <c>public Vector4 Background</c> and a UIElement of type <c>Rectangle</c> defines a stylable
/// property with the same name, then these will be combined into a single stylable property 
/// factory method.
/// <para/>
/// Must be used on fields which also have a <see cref="ReactiveAttribute"/>.
/// </summary>
[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class StylableAttribute : Attribute
{

}
