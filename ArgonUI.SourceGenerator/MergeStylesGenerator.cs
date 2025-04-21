using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static ArgonUI.SourceGenerator.ReactiveObjectGenerator;

namespace ArgonUI.SourceGenerator;

// This class is an incremental generator but since it relies on the output of the ReactiveObjectGenerator,
// it does not implement IIncrementalGenerator and is instead called by ReactiveObjectGenerator
//[Generator(LanguageNames.CSharp)]
public partial class MergeStylesGenerator// : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context, 
        IncrementalValueProvider<EquatableArray<ReactiveObjectResult>> reactiveObjects)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            //Debugger.Launch();
        }
#endif 

        // TODO: The MergeStyleGenerator needs to run after the ReactiveObjectGenerator, the only way to do that is to call this generator from the ReactiveObjectGenerator

        var parser = new Parser();

        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(MergeStylesAttribute).FullName,
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is TypeDeclarationSyntax,
            transform: static (context, cancellationToken) => context // TODO: Parsing should happen here, so that parsed syntax can be cached
        );

        /*var generatedStylesPipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(GeneratedStylesAttribute).FullName,
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is TypeDeclarationSyntax,
            transform: static (context, cancellationToken) => context
        );*/

        /*var generatedStylesCollection = generatedStylesPipeline
            .Collect()
            .Select((x, _) => x.AsEquatable()
                .GroupBy(x => x.TargetSymbol.ContainingAssembly.Name)
                .ToEquatable());*/

        var reactiveObjectsGrouped = reactiveObjects.Select(Parser.ParseReactiveObjects);

        var pipelineParsed = pipeline
            .Collect()
            .Combine(reactiveObjectsGrouped)
            .Select((x, _) => 
                parser.Parse(x.Left.AsEquatable(), x.Right)
                .ToImmutableArray().AsEquatable());

        context.RegisterSourceOutput(pipelineParsed, Execute);
    }

    private static void Execute(SourceProductionContext context, EquatableArray<MergeStylesResult> sources)
    {
        //context.ReportDiagnostic(Diagnostic.Create(new("AR1000", "ReactiveObjectAttribute test", $"{sources.Length}", "TEST", DiagnosticSeverity.Warning, true), null));
        if (sources.Length == 0)
            return;

        foreach (var parsed in sources)
            Emit(context, parsed);
    }
}
