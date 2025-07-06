using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD
using System.Runtime.Intrinsics;
#endif

namespace ArgonUI;

/// <summary>
/// Represents a thickness in terms of top, right, bottom, and left edge.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Thickness
{
    /// <summary>
    /// Specified as a vector of (Left, Top, Right, Bottom).
    /// <para/>
    /// This is an alias for the <see cref="left"/>, <see cref="right"/>, <see cref="top"/>, and <see cref="bottom"/> fields.
    /// </summary>
    [FieldOffset(0x0)] public Vector4 value;

    /// <summary>
    /// Alias for (<see cref="left"/>, <see cref="top"/>).
    /// </summary>
    [FieldOffset(0x0)] public Vector2 leftTop;
    /// <summary>
    /// Alias for (<see cref="right"/>, <see cref="bottom"/>).
    /// </summary>
    [FieldOffset(0x8)] public Vector2 rightBottom;

    [FieldOffset(0x0)] public float left;
    [FieldOffset(0x4)] public float right;
    [FieldOffset(0x8)] public float top;
    [FieldOffset(0xC)] public float bottom;

    public static Thickness Zero => new();

    /// <summary>
    /// Gets the total width and height of this <see cref="Thickness"/>.
    /// </summary>
    public readonly Vector2 Size => leftTop + rightBottom;

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
    /// Constructs a thickness from a vector of (Left, Top, Right, Bottom).
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
