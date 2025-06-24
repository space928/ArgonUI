using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using static ArgonUI.SourceGenerator.ReactiveObjectGenerator;

namespace ArgonUI.SourceGenerator;

public partial class MergeStylesGenerator
{
    internal class Parser
    {
        public IEnumerable<MergeStylesResult> Parse(EquatableArray<GeneratorAttributeSyntaxContext> sources,
            EquatableDictionary<string, EquatableArray<ReactiveObjectClass>> reactiveObjects)
        {
            foreach (var source in sources)
            {
                if (source.TargetNode is not TypeDeclarationSyntax targetType)
                    continue;

                if (source.TargetSymbol is not INamedTypeSymbol classSymbol)
                    continue;

                if (!targetType.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.StyleMustBePartial, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                if (!targetType.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.StyleMustBeStatic, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                // Check that the declaring type is not generic
                if (classSymbol.TypeParameters.Length > 0)
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.StyleCantBeGeneric, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                // Get the MergeStyles Attribute
                string[]? assemblyNames = null;
                var attributes = classSymbol.GetAttributes();
                foreach (var attrib in attributes)
                {
                    var args = attrib.ConstructorArguments;
                    switch (attrib?.AttributeClass?.Name)
                    {
                        case nameof(MergeStylesAttribute):
                            if (args.Length >= 1)
                            {
                                if (args[0].Value is string assemblyName)
                                    assemblyNames = [assemblyName];
                                else
                                    assemblyNames = (string[])args[0].Value!;
                            }
                            break;
                    }
                }

                // Get the generated styles from the assembly
                if (assemblyNames == null)
                    continue;
                IEnumerable<ReactiveObjectClass> reactiveObjectsFiltered = [];
                foreach (var assemblyName in assemblyNames)
                {
                    if (!reactiveObjects.TryGetValue(assemblyName, out var assemblyStylesSources))
                    {
                        var diag = Diagnostic.Create(DiagnosticDescriptors.AssemblyNotFound, targetType.Identifier.GetLocation(), classSymbol.Name, assemblyName ?? string.Empty);
                        yield return new(null, diag);
                    }
                    else
                    {
                        reactiveObjectsFiltered = reactiveObjectsFiltered.Concat(assemblyStylesSources);
                    }
                }

                bool enableNullable = false;
                Dictionary<string, MergedStylablePropTemp> styledProps = [];
                foreach (var reactiveObject in reactiveObjectsFiltered)
                {
                    // Iterate through each of the classes annotated with [GeneratedStyles] in the chosen assemblies
                    var styleClassName = reactiveObject.ClassName;
                    enableNullable |= reactiveObject.EnableNullable;

                    // Check for the GeneratedStylesAttribute
                    /*var styleAttrs = classSymbol.GetAttributes();
                    bool isAssemblyStyleClass = false;
                    foreach (var attrib in styleAttrs)
                    {
                        var args = attrib.ConstructorArguments;
                        switch (attrib?.AttributeClass?.Name)
                        {
                            case nameof(GeneratedStylesAttribute):
                                if (args.Length <= 1)
                                    isAssemblyStyleClass = true;
                                break;
                        }
                    }
                    // Skip merged style classes to prevent recursive execution
                    if (isAssemblyStyleClass)
                        continue;*/

                    foreach (var member in reactiveObject.ReactiveFields)
                    {
                        // Find all the stylable property factory methods
                        //if (member.DeclaredAccessibility != Accessibility.Public || member.Kind != SymbolKind.Method)
                        //    continue;
                        //var factorySymbol = (IMethodSymbol)member;
                        //string name = factorySymbol.Name;
                        //var typeSymb = factorySymbol.Parameters[0].Type;
                        //string type = typeSymb.Name;

                        //bool nullable = typeSymb.SpecialType == SpecialType.System_Nullable_T;
                        //enableNullable |= nullable;
                        if (member.Stylable is not StylableField stylableField)
                            continue;

                        if (styledProps.TryGetValue(member.PropName, out var mergedStylablePropTemp))
                        {
                            mergedStylablePropTemp.StyledTypes.Add(styleClassName);
                        }
                        else
                        {
                            //var docComment = factorySymbol.GetDocumentationCommentXml();
                            styledProps.Add(member.PropName, new(member.PropName, member.FieldType, member.DocComment, [styleClassName]));
                        }
                    }
                }

                var styledPropsEquatable = new EquatableArray<MergedStylableProp>(
                    styledProps.Values.Select(x => new MergedStylableProp(x.Name, x.Type, x.DocComment, new(x.StyledTypes))));

                MergeStylesClass reactiveObjectClass = new(
                    classSymbol.DeclaredAccessibility,
                    classSymbol.ContainingNamespace.ToString(),
                    assemblyNames,
                    classSymbol.Name,
                    enableNullable,
                    styledPropsEquatable);

                yield return new(reactiveObjectClass, null);
            }
        }

        public static EquatableDictionary<string, EquatableArray<ReactiveObjectClass>> ParseReactiveObjects(
            EquatableArray<ReactiveObjectResult> reactiveObjects, CancellationToken cancellationToken)
        {
            return reactiveObjects
                .Where(x => x.Class != null)
                .Select(x => x.Class!)
                .GroupBy(x => x.Assembly).ToEquatable();
        }

        private static string FormatPropName(string fieldName)
        {
            if (char.IsLower(fieldName[0]))
                return $"{char.ToUpper(fieldName[0])}{fieldName[1..]}";

            return $"_{fieldName}";
        }
    }

    public record MergeStylesResult(MergeStylesClass? Class, Diagnostic? Diagnostic);
    public record MergeStylesClass(Accessibility Accessibility, string Namespace, string[] Assemblies, string ClassName, bool EnableNullable, EquatableArray<MergedStylableProp> ReactiveFields);
    public record MergedStylableProp(string Name, string Type, string? DocComment, EquatableArray<string> StyledTypes);
    public record MergedStylablePropTemp(string Name, string Type, string? DocComment, List<string> StyledTypes);
}
