using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArgonUI;

/// <summary>
/// Represents a thickness in terms of top, right, bottom, and left edge.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Thickness
{
    // Specified as a vector of (Top, Right, Bottom, Left).
    [FieldOffset(0x0)] public Vector4 value;
    [FieldOffset(0x0)] public float top;
    [FieldOffset(0x4)] public float right;
    [FieldOffset(0x8)] public float bottom;
    [FieldOffset(0xC)] public float left;

    public static Thickness Zero => new();

    public Thickness(float all)
    {
        Unsafe.SkipInit(out this);
        this.value = new Vector4(all);
    }

    public Thickness(float leftRight, float topBottom)
    {
        Unsafe.SkipInit(out this);
        left = right = leftRight;
        top = bottom = topBottom;
    }

    public Thickness(float top, float right, float bottom, float left)
    {
        Unsafe.SkipInit(out this);
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.left = left;
    }

    /// <summary>
    /// Constructs a thickness from a vector of (Top, Right, Bottom, Left).
    /// </summary>
    /// <param name="value"></param>
    public Thickness(Vector4 value)
    {
        Unsafe.SkipInit(out this);
        this.value = value;
    }

    public Thickness()
    {
        Unsafe.SkipInit(out this);
        this.value = Vector4.Zero;
    }

    public static implicit operator Thickness(Vector4 value) => new(value);
    public static implicit operator Vector4(Thickness value) => value.value;
}
