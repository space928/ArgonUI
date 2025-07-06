using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ArgonUI.SourceGenerator;

public partial class ReactiveObjectGenerator
{
    private static void Emit(SourceProductionContext context, ReactiveObjectResult result)
    {
        if (result.Diagnostic != null)
            context.ReportDiagnostic(result.Diagnostic);
        if (result.Class is not ReactiveObjectClass model)
            return;

        IndentedStringBuilder sb = new();

        Helpers.EmitFileHeader(sb, model.Namespace, model.EnableNullable, ["ArgonUI.SourceGenerator"]);

        sb.AppendLine($"{model.Accessibility.GetText()} partial class {model.ClassName}");
        using (sb.EnterCurlyBracket())
        {
            foreach (var prop in model.ReactiveFields)
            {
                // Generate a property
                if (!string.IsNullOrEmpty(prop.DocComment))
                    Helpers.PrintDocComment(sb, prop.DocComment!);
                if (prop.CustomAccessibility != null)
                    sb.AppendLine($"{prop.CustomAccessibility} {prop.FieldType} {prop.PropName}");
                else
                    sb.AppendLine($"public {prop.FieldType} {prop.PropName}");
                using (sb.EnterCurlyBracket())
                {
                    // Getter
                    if (prop.OnGetFunc != null)
                    {
                        if (prop.GetInline)
                            sb.AppendLine($"get => {prop.OnGetFunc};");
                        else
                            sb.AppendLine($"get => {prop.OnGetFunc}();");
                    }
                    else
                    {
                        sb.AppendLine($"get => {prop.FieldName};");
                    }

                    // Setter
                    sb.AppendLine($"set");
                    using (sb.EnterCurlyBracket())
                    {
                        //sb.AppendLine($"{prop.FieldName} = value;");
                        if (prop.OnSetAction != null)
                        {
                            sb.AppendLine($"OnChanging(\"{prop.PropName}\");");
                            if (prop.SetInline)
                                sb.AppendLine($"{prop.OnSetAction};");
                            else
                                sb.AppendLine($"{prop.OnSetAction}(value);");
                            sb.AppendLine($"OnChanged(\"{prop.PropName}\");");
                        }
                        else
                        {
                            string prefix = string.Empty;
                            if (prop.FieldName == "value")
                                prefix = "this.";
                            sb.AppendLine($"UpdateProperty(ref {prefix}{prop.FieldName}, value);");
                        }
                        if (prop.DirtyFlags != UIElements.Abstract.DirtyFlag.None)
                        {
                            sb.AppendLine($"Dirty(ArgonUI.UIElements.Abstract.DirtyFlag.{prop.DirtyFlags});");
                        }
                    }
                }
                sb.AppendLine();
            }
        }

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

        context.AddSource($"{model.ClassName}_ReactiveObject.g.cs", sourceText);

        //EmitStylableClass(context, result);
    }

    /// <summary>
    /// Emits a factory class for making stylable properties for a given UIElement.
    /// </summary>
    /// <example>
    /// /* Outputs the following code: */
    /// 
    /// // Factory method for a new stylable prop
	/// public StylableProp<Colour> BgColour(Colour value)
    /// {
    ///     return new(value, Apply_Background);
    /// }
    /// 
    /// private void Apply_Background(UIElement elem, StylableProp<Colour> prop)
    /// {
    ///     switch (elem)
    ///     {
    ///         case Button button:
    ///             {
    ///                 button.Background = prop.Value;
    ///                 break;
    ///             }
    ///         default:
    ///             throw ...
	/// 	}
    /// }
    /// </example>
    /// <param name="context"></param>
    /// <param name="result"></param>
    private static void EmitStylableClass(SourceProductionContext context, ReactiveObjectResult result)
    {
        if (result.Diagnostic != null)
            context.ReportDiagnostic(result.Diagnostic);
        if (result.Class is not ReactiveObjectClass model)
            return;

        IndentedStringBuilder sb = new();

        Helpers.EmitFileHeader(sb, model.Namespace, model.EnableNullable, ["System.Runtime.CompilerServices", "ArgonUI.SourceGenerator", "ArgonUI.Styling", "ArgonUI.UIElements"]);

        sb.AppendLine($"[GeneratedStyles(\"{model.Assembly}\", \"{model.ClassName}\")]");
        sb.AppendLine($"{model.Accessibility.GetText()} static partial class {model.ClassName}_Styles");
        using (sb.EnterCurlyBracket())
        {
            foreach (var prop in model.ReactiveFields)
            {
                if (prop.Stylable == null)
                    continue;

                // Generate a factory method
                if (!string.IsNullOrEmpty(prop.DocComment))
                    Helpers.PrintDocComment(sb, prop.DocComment!);
                sb.AppendLine("/// <remarks>This is a factory method for a stylable property.</remarks>");
                sb.AppendLine("/// <param name=\"value\">The initial value of the new stylable property.</param>");
                sb.AppendLine($"public static StylableProp<{prop.FieldType}> {prop.PropName}({prop.FieldType} value)");
                using (sb.EnterCurlyBracket())
                {
                    sb.AppendLine($"return new(value, Apply_{prop.PropName}, \"{prop.PropName}\");");
                }
                sb.AppendLine();

                // Generate the Apply method
                sb.AppendLine($"[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                sb.AppendLine($"private static void Apply_{prop.PropName}(UIElement elem, IStylableProperty prop)");
                using (sb.EnterCurlyBracket())
                {
                    sb.AppendLine($"(({model.ClassName})elem).{prop.PropName} = ((StylableProp<{prop.FieldType}>)prop).Value;");
                }
                sb.AppendLine();
            }
        }

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

        context.AddSource($"{model.ClassName}_Styles.g.cs", sourceText);
    }
}
