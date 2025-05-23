﻿using ArgonUI.UIElements;
using System.Numerics;

namespace ArgonUI.SourceGenerator.Test;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

public partial class ReactiveTest : UIElement
{
    [Reactive("SpecialExample")] private int exampleValue;
    [Reactive(propName: "SpecialExample1")] private int exampleValue1;
    [Reactive, Dirty(DirtyFlags.ChildContent)] private UIElement? test2;
    [Reactive, Dirty(DirtyFlags.Layout), CustomGet(nameof(GetTest3))] private float test3;
    /// <summary>
    /// An example property.
    /// </summary>
    [Reactive, Dirty(DirtyFlags.Layout), CustomSet(nameof(SetTest4)), Stylable] private float test4;
    /// <summary>
    /// A vector example.
    /// </summary>
    /// <remarks>
    /// Very dimensional.
    /// </remarks>
    [Reactive, Stylable] private Vector2 testVec;

    public void Test()
    {
        //exampleValue
        //ExampleValue = 1;
        //ExampleValue = 1;
        //Test2 = null;
        Test3 = 1;
        Test4 = 2;
        TestVec = new Vector2();
        ReactiveTest_Styles.SpecialExample(1);
        Test_Styles.Test4(1);
    }

    private float GetTest3() => 1;
    private void SetTest4(float value) => test4 = value;
}

[MergeStyles("ArgonUI.SourceGenerator.Test")]
internal static partial class Test_Styles
{ }

//internal static partial class ReactiveTest_Styles { }

public abstract class UIElement : ReactiveObject
{
    private DirtyFlags dirtyFlag;

    public UIElement? Parent { get; set; }

    /// <summary>
    /// Marks this element as dirty, forcing the UI engine to redraw this element and it's children when it's next dispatched.
    /// </summary>
    /// <param name="flags">Which <see cref="ArgonUI.UIElements.DirtyFlags"/> to set.</param>
    public virtual void Dirty(DirtyFlags flags)
    {
        UpdateProperty(ref dirtyFlag, dirtyFlag | flags, nameof(DirtyFlags));

        // Propagate dirty flags up
        if ((flags & DirtyFlags.Layout) != 0)
            Parent?.Dirty(DirtyFlags.ChildLayout);

        if ((flags & DirtyFlags.Content) != 0)
            Parent?.Dirty(DirtyFlags.ChildContent);
    }
}
