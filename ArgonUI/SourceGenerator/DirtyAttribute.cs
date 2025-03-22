using ArgonUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.SourceGenerator;

/// <summary>
/// When used on a property generated using a <see cref="ReactiveAttribute"/>, generates a call to 
/// the <see cref="UIElement.Dirty(DirtyFlags)"/> method when the property is set.
/// This attribute can only be used in classes which derive from <see cref="ArgonUI.UIElements.UIElement"/>.
/// </summary>
/// <param name="dirtyFlags">The dirty flags to set when the generated property is set.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class DirtyAttribute(DirtyFlags dirtyFlags) : Attribute
{
    private readonly DirtyFlags dirtyFlags = dirtyFlags;

    public DirtyFlags DirtyFlags => dirtyFlags;
}
