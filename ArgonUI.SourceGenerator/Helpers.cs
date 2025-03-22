using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ArgonUI.SourceGenerator;

internal static class Helpers
{
    public static string GetText(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => "",
            Accessibility.Private => "private",
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => ""
        };
    }

    public static EquatableArray<T> AsEquatable<T>(this ImmutableArray<T> immutable) => new(immutable);
}
