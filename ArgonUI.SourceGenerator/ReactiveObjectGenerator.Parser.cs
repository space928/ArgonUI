using ArgonUI.UIElements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ArgonUI.SourceGenerator;

public partial class ReactiveObjectGenerator
{
    internal class Parser
    {
        public IEnumerable<ReactiveObjectResult> Parse(EquatableArray<GeneratorAttributeSyntaxContext> sources)
        {
            foreach (var sourceClass in sources.GroupBy(x => x.TargetNode?.Parent?.Parent?.Parent)) // VariableDeclaratorSyntax -> VariableDeclarationSyntax -> FieldDeclarationSyntax -> TypeDeclarationSyntax
            {
                if (sourceClass.Key == null)
                    continue;
                var targetType = (TypeDeclarationSyntax)sourceClass.Key;
                var classSymbol = sourceClass.First().SemanticModel.GetDeclaredSymbol(targetType);
                if (classSymbol == null)
                    continue;

                // Check that the declaring type is partial
                if (!targetType.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.MustBePartial, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                // Check that the declaring type is not generic
                if (classSymbol.TypeParameters.Length > 0)
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.CantBeGeneric, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                // Check that the class derives from ReactiveObject
                bool isReactive = false;
                bool isUIElement = false;
                var parent = classSymbol;
                while ((parent = parent.BaseType) != null)
                {
                    // TODO: Evil magic strings, not safe from refactoring
                    if (parent.Name == "UIElement")
                    {
                        isReactive = isUIElement = true;
                        break;
                    }
                    if (parent.Name == "ReactiveObject")
                    {
                        isReactive = true;
                        break;
                    }
                }
                if (!isReactive)
                {
                    var diag = Diagnostic.Create(DiagnosticDescriptors.MustDeriveFromReactiveObject, targetType.Identifier.GetLocation(), classSymbol.Name);
                    yield return new(null, diag);
                }

                var fields = ImmutableArray.CreateBuilder<ReactiveObjectField>();
                bool enableNullable = false;

                // Create fields
                foreach (var reactiveNode in sourceClass)
                {
                    var fieldName = reactiveNode.TargetSymbol.Name;
                    var varSyntax = (VariableDeclarationSyntax)reactiveNode.TargetNode.Parent!;
                    var type = ((IFieldSymbol)reactiveNode.TargetSymbol).Type;
                    var typeName = type.ToString();
                    bool nullable = varSyntax.Type.Kind() == SyntaxKind.NullableType;
                    enableNullable |= nullable;
                    var propName = FormatPropName(fieldName);
                    string? docComment = reactiveNode.TargetSymbol.GetDocumentationCommentXml();
                    string? getFunc = null;
                    string? setAction = null;
                    var dirtyFlags = DirtyFlags.None;

                    // Parse attributes
                    var attributes = reactiveNode.TargetSymbol.GetAttributes();
                    foreach (var attrib in attributes)
                    {
                        var args = attrib.ConstructorArguments;
                        switch (attrib?.AttributeClass?.Name)
                        {
                            case nameof(ReactiveAttribute):
                                if (args.Length >= 1)
                                    propName = (string)args[0].Value!;
                                break;
                            case nameof(CustomGetAttribute):
                                if (args.Length >= 1)
                                    getFunc = (string)args[0].Value!;
                                break;
                            case nameof(CustomSetAttribute):
                                if (args.Length >= 1)
                                    setAction = (string)args[0].Value!;
                                break;
                            case nameof(DirtyAttribute):
                                if (!isUIElement)
                                {
                                    var diag = Diagnostic.Create(DiagnosticDescriptors.MustDeriveFromUIElement, targetType.Identifier.GetLocation(), classSymbol.Name);
                                    yield return new(null, diag);
                                }
                                if (args.Length >= 1)
                                    dirtyFlags = (DirtyFlags)args[0].Value!;
                                break;
                            default:
                                break;
                        }
                    }

                    fields.Add(new(typeName, fieldName, propName, docComment, dirtyFlags, getFunc, setAction));
                }

                ReactiveObjectClass reactiveObjectClass = new(
                    classSymbol.DeclaredAccessibility, 
                    classSymbol.ContainingNamespace.ToString(), 
                    classSymbol.Name,
                    enableNullable,
                    fields.DrainToImmutable().AsEquatable());

                yield return new(reactiveObjectClass, null);
            }
        }

        private static string FormatPropName(string fieldName)
        {
            if (char.IsLower(fieldName[0]))
                return $"{char.ToUpper(fieldName[0])}{fieldName[1..]}";

            return $"_{fieldName}";
        }
    }

    public record ReactiveObjectResult(ReactiveObjectClass? Class, Diagnostic? Diagnostic);
    public record ReactiveObjectClass(Accessibility Accessibility, string Namespace, string ClassName, bool EnableNullable, EquatableArray<ReactiveObjectField> ReactiveFields);
    public record ReactiveObjectField(string FieldType, string FieldName, string PropName, string? DocComment, DirtyFlags DirtyFlags, string? OnGetFunc, string? OnSetAction);
}
