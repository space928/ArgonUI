using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.SourceGenerator;

/// <summary>
/// Generates <see cref="Styling.StylableProp{T}"/> factory methods for all the fields marked with
/// <see cref="ReactiveAttribute"/> and <see cref="StylableAttribute"/> in the specified assemblies.
/// <para/>
/// The target class must be declared as <see langword="static"/> and <see langword="partial"/>.
/// </summary>
/// <example>
/// [MergeStyles("ArgonUI")]
/// public static partial class ArgonUIStyles
/// { }
/// </example>
[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MergeStylesAttribute(string[] AssemblyNames) : Attribute
{
    public string[] AssemblyNames { get; set; } = AssemblyNames;

    public MergeStylesAttribute(string AssemblyName) : this([AssemblyName]) { }
}
