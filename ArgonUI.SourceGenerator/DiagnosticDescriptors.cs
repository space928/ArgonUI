using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.SourceGenerator;

internal static class DiagnosticDescriptors
{
    const string Category = "GenerateReactiveObject";

    public static DiagnosticDescriptor MustBePartial => new(
        "AR1001",
        "ReactiveAttribute annotated field's declaring type must be partial",
        "The field declared in type '{0}' annotated with ReactiveAttribute's class must be declared as partial.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor CantBeGeneric => new(
        "AR1002",
        "ReactiveAttribute annotated field's declaring type cannot be a generic type",
        "The field declared in type '{0}' annotated with ReactiveAttribute's class cannot be a generic type.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MustDeriveFromReactiveObject => new(
        "AR1003",
        "ReactiveAttribute annotated field's declaring type must derive from 'ArgonUI.ReactiveObject'",
        "The field declared in type '{0}' annotated with ReactiveAttribute's class must derive from 'ArgonUI.ReactiveObject'.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MustDeriveFromUIElement => new(
        "AR1004",
        "ReactiveAttribute annotated field's declaring type must derive from 'ArgonUI.UIElements.UIElement'",
        "The field declared in type '{0}' annotated with ReactiveAttribute's class must derive from 'ArgonUI.UIElements.UIElement' to be able to set DirtyFlags.",
        Category,
        DiagnosticSeverity.Warning,
        true);


    public static DiagnosticDescriptor StyleMustBePartial => new(
        "AR2001",
        "MergeStylesAttribute annotated class must be partial",
        "The class '{0}' annotated with a MergeStylesAttribute must be declared as partial.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor StyleMustBeStatic => new(
        "AR2001",
        "MergeStylesAttribute annotated class must be static",
        "The class '{0}' annotated with a MergeStylesAttribute must be declared as static.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor StyleCantBeGeneric => new(
        "AR2003",
        "MergeStylesAttribute annotated class cannot be a generic type",
        "The class '{0}' annotated with a MergeStylesAttribute cannot be a generic type.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor AssemblyNotFound => new(
        "AR2004",
        "MergeStylesAttribute specified assembly couldn't be found",
        "The class '{0}' annotated with a MergeStylesAttribute specifies an assembly '{1}' which could not be found.",
        Category,
        DiagnosticSeverity.Error,
        true);
}
