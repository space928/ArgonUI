using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.SourceGenerator;

/// <summary>
/// Marks this class as being an auto-generated style factory.
/// </summary>
[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class GeneratedStylesAttribute(string[] AssemblyNames, string? ClassName = null) : Attribute
{
    public string[] AssemblyName { get; set; } = AssemblyNames;
    public string? ClassName { get; set; } = ClassName;

    public GeneratedStylesAttribute(string AssemblyName, string? ClassName = null) : this([AssemblyName], ClassName)
    { }
}
