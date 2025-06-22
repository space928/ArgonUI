using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.SourceGenerator;

public partial class MergeStylesGenerator
{
    /*
    // Generates:
    [GeneratedStyles("ArgonUI")]
    public static class ArgonUI_Styles
    {
        // Factory method for a new stylable prop
	    public StylableProp<Colour> BgColour(Colour value)
        {
            return new(value, Apply_Background);
        }
                    
        private void Apply_Background(UIElement elem, StylableProp<Colour> prop)
        {
            switch (elem)
            {
                case Button button:
                    {
                        button.Background = prop.Value;
                        break;
                    }
                default:
                    throw ...
	        }
        }
    }
    */
    private static void Emit(SourceProductionContext context, MergeStylesResult result)
    {
        if (result.Diagnostic != null)
            context.ReportDiagnostic(result.Diagnostic);
        if (result.Class is not MergeStylesClass model)
            return;

        IndentedStringBuilder sb = new();

        Helpers.EmitFileHeader(sb, model.Namespace, model.EnableNullable, ["System.Runtime.CompilerServices", "ArgonUI.Styling", "ArgonUI.SourceGenerator", "ArgonUI.UIElements"]);

        sb.Append($"[GeneratedStyles([");
        AppendAssemblies(sb, model.Assemblies);
        sb.AppendLine($"])]");
        sb.AppendLine($"{model.Accessibility.GetText()} static partial class {model.ClassName}");
        using (sb.EnterCurlyBracket())
        {
            foreach (var prop in model.ReactiveFields)
            {
                // Generate a factory method
                if (!string.IsNullOrEmpty(prop.DocComment))
                    Helpers.PrintDocComment(sb, prop.DocComment!);
                sb.AppendLine("/// <remarks>This is a factory method for a stylable property.</remarks>");
                sb.AppendLine("/// <param name=\"value\">The initial value of the new stylable property.</param>");
                sb.AppendLine($"public static StylableProp<{prop.Type}> {prop.Name}({prop.Type} value)");
                using (sb.EnterCurlyBracket())
                {
                    sb.AppendLine($"return new(value, Apply_{prop.Name}, \"{prop.Name}\");");
                }
                sb.AppendLine();

                // Generate the Apply method
                //sb.AppendLine($"[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                sb.AppendLine($"private static void Apply_{prop.Name}(UIElement elem, IStylableProperty prop)");
                using (sb.EnterCurlyBracket())
                {
                    sb.AppendLine($"var typedProp = ((StylableProp<{prop.Type}>)prop);");
                    sb.AppendLine($"switch (elem)");
                    using (sb.EnterCurlyBracket())
                    {
                        foreach (var styled in prop.StyledTypes)
                            sb.AppendLine($"case {styled} _{styled}: _{styled}.{prop.Name} = typedProp.Value; break;");
                        sb.AppendLine($"default: break;");
                        //sb.AppendLine($"default: throw new InvalidOperationException(\"Can't set property '{prop.Name}' on element of type '\" + elem.GetType().Name + \"'\");");
                    }
                    //sb.AppendLine($"(({model.ClassName})elem).{prop.PropName} = prop.Value;");
                }
                sb.AppendLine();
            }
        }

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

        context.AddSource($"{model.ClassName}.g.cs", sourceText);
    }

    private static void AppendAssemblies(IndentedStringBuilder sb, string[] assemblies)
    {
        if (assemblies.Length == 0)
            return;

        foreach (var assembly in assemblies.AsSpan()[..^1])
        {
            sb.Append('"');
            sb.Append(assembly);
            sb.Append("\", ");
        }

        sb.Append('"');
        sb.Append(assemblies[^1]);
        sb.Append('"');
    }
}
