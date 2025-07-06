using ArgonUI.Styling;
using ArgonUI.UIElements.Abstract;
using System.Numerics;

namespace ArgonUI.SourceGenerator.Test;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

[UIClonable]
public partial class ReactiveTest : UIElement
{
    [Reactive("SpecialExample"), Stylable] private uint exampleValue;
    [Reactive(propName: "SpecialExample1")] private int exampleValue1;
    [Reactive, Dirty(DirtyFlag.ChildContent)] private UIElement? test2;
    [Reactive, Dirty(DirtyFlag.Layout), CustomGet(nameof(GetTest3))] private float test3;
    /// <summary>
    /// An example property.
    /// </summary>
    [Reactive, Dirty(DirtyFlag.Layout), CustomSet(nameof(SetTest4)), Stylable] private float test4;
    /// <summary>
    /// A vector example.
    /// </summary>
    /// <remarks>
    /// Very dimensional.
    /// </remarks>
    [Reactive, Stylable] private Vector2 testVec;
    [Reactive] private ComplexValue? testComplex;

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
        testComplex?.Clone();
    }

    private float GetTest3() => 1;
    private void SetTest4(float value) => test4 = value;

    public static StylableProp<int> SpecialExample9(int value)
    {
        return new(value, Apply_SpecialExample9, "SpecialExample9");
    }

    public static void Apply_SpecialExample9(UIElement elem, IStylableProperty prop)
    {

    }
}

[MergeStyles("ArgonUI.SourceGenerator.Test")]
internal static partial class Test_Styles
{ }

//internal static partial class ReactiveTest_Styles { }

public abstract class UIElement : ReactiveObject
{
    private DirtyFlag dirtyFlag;

    public UIElement? Parent { get; set; }

    /// <summary>
    /// Marks this element as dirty, forcing the UI engine to redraw this element and it's children when it's next dispatched.
    /// </summary>
    /// <param name="flags">Which <see cref="ArgonUI.UIElements.DirtyFlags"/> to set.</param>
    public virtual void Dirty(DirtyFlag flags)
    {
        UpdateProperty(ref dirtyFlag, dirtyFlag | flags, nameof(DirtyFlag));

        // Propagate dirty flags up
        if ((flags & DirtyFlag.Layout) != 0)
            Parent?.Dirty(DirtyFlag.ChildLayout);

        if ((flags & DirtyFlag.Content) != 0)
            Parent?.Dirty(DirtyFlag.ChildContent);
    }

    public virtual UIElement Clone() => throw new NotImplementedException();
    public virtual UIElement Clone(UIElement target) => target;
}

public class ComplexValue
{
    public float a;
    public float b;
}

public static partial class ExtensionMethods
{
    public static ComplexValue Clone(this ComplexValue obj)
    {
        var clone = new ComplexValue();
        clone.a = obj.a;
        clone.b = obj.b;
        return clone;
    }
}
