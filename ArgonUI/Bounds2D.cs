using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace ArgonUI;

[StructLayout(LayoutKind.Explicit, Pack = 8)]
public struct Bounds2D : IEquatable<Bounds2D>
{
    [FieldOffset(0x0)] private Vector4 _value;
    [FieldOffset(0x0)] public Vector2 topLeft;
    [FieldOffset(0x8)] public Vector2 bottomRight;

    public static Bounds2D Zero => new(Vector4.Zero);
    
    public readonly Vector2 TopRight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(bottomRight.X, topLeft.Y);
    }
    public readonly Vector2 BottomLeft
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(topLeft.X, bottomRight.Y);
    }

    public Bounds2D() 
    {
        Unsafe.SkipInit(out this);
        _value = Vector4.Zero;
    }

    public Bounds2D(Vector2 topLeft, Vector2 bottomRight)
    {
        Unsafe.SkipInit(out this);
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
    }

    public Bounds2D(float left, float right, float top, float bottom) : this(new(left, top, right, bottom)) { }

    internal Bounds2D(Vector4 value)
    {
        Unsafe.SkipInit(out this);
        _value = value;
    }

    public readonly float Width => bottomRight.X - topLeft.X;
    public readonly float Height => bottomRight.Y - topLeft.Y;
    /// <summary>
    /// The width and height of these bounds.
    /// </summary>
    public readonly Vector2 Size => bottomRight - topLeft;
    /// <summary>
    /// The coordinates of the centre of these bounds.
    /// </summary>
    public readonly Vector2 Centre => (bottomRight + topLeft) * 0.5f;

    /// <summary>
    /// Checks if the given point is inside or on the bounds.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public readonly bool Contains(Vector2 point)
    {
#if false && !NETSTANDARD
        if (Vector128<float>.IsSupported && Vector128.IsHardwareAccelerated && Sse41.IsSupported)
        {
            // This vectorised version is sometimes marginally faster
            // Take care refactoring it, it's fairly carefully written
            // such that it emits the desired instructions.

            // AsVector128() is a special intrinsic which prevents needed to copy vectors from xmm
            // registers to and from the stack to convert them to Vector128s.
            var boundsVec = _value.AsVector128().AsDouble();
            // Calling AsVector128 here results in an extra vinsertps instruction which isn't needed
            // but all the alternatives resulted in the vector2 being copied to the stack and then
            // back into the xmm register, which was worse
            var pointVec = point.AsVector128().AsDouble();
            // lhs => point.xy, bottomRight.xy; rhs => topLeft.xy, point.xy
            // Blend is slightly faster than shuffle, so we use it when we can
            var lhs = Sse41.Blend(pointVec, boundsVec, 2).AsSingle();
            var rhs = Sse41.Shuffle(boundsVec, pointVec, 0).AsSingle();

            return Vector128.GreaterThanOrEqualAll(lhs, rhs);
        }
#endif
        return point.X >= topLeft.X && point.Y >= topLeft.Y && point.X <= bottomRight.X && point.Y <= bottomRight.Y;
    }

    /// <summary>
    /// Checks if the given bounds are fully contained by this bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly bool Contains(Bounds2D bounds)
    {
#if !NETSTANDARD
        if (Vector128<float>.IsSupported && Vector128.IsHardwareAccelerated && Sse41.IsSupported)
        {
            var thisVec = _value.AsVector128().AsDouble();
            var otherVec = bounds._value.AsVector128().AsDouble();
            var lhs = Sse41.Blend(otherVec, thisVec, 2).AsSingle();
            var rhs = Sse41.Blend(thisVec, otherVec, 2).AsSingle();

            return Vector128.GreaterThanOrEqualAll(lhs, rhs);
        }
#endif

        return bounds.topLeft.X >= topLeft.X && bounds.topLeft.Y >= topLeft.Y 
            && bounds.bottomRight.X <= bottomRight.X && bounds.bottomRight.Y <= bottomRight.Y;
    }

    /// <summary>
    /// Checks if the given bounds intersects this bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly bool Intersects(Bounds2D bounds)
    {
        return topLeft.X <= bounds.bottomRight.X && bottomRight.X >= bounds.topLeft.X
            && topLeft.Y <= bounds.bottomRight.Y && bottomRight.Y >= bounds.topLeft.Y;
    }

    /// <summary>
    /// Computes the intersection bounds between this and the given bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly Bounds2D Intersect(Bounds2D bounds)
    {
#if !NETSTANDARD
        if (Vector128.IsHardwareAccelerated && Sse41.IsSupported)
        {
            // Inspired by clang's implementation...
            var thisVec = _value.AsVector128();
            var otherVec = bounds._value.AsVector128();
            var a = Sse41.Blend(thisVec, otherVec, 3); // (other.tl, this.br)
            var b = Sse41.Blend(otherVec, thisVec, 3); // (this.tl, other.br)
            var cmp = Sse.CompareGreaterThan(a, b);       // (this.tl < other.tl, this.br > other.br)
            var res = Sse41.BlendVariable(thisVec, otherVec, cmp); // (this < other ? other.tl : this.tl, this > other ? other.br : this.br)
            return new(res.AsVector4());
        }
#endif
        return new Bounds2D(Vector2.Max(topLeft, bounds.topLeft), Vector2.Min(bottomRight, bounds.bottomRight));
    }

    /// <summary>
    /// Returns a bounds which encapsulates both this bounds and the given bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly Bounds2D Union(Bounds2D bounds)
    {
#if !NETSTANDARD
        if (Vector128.IsHardwareAccelerated && Sse41.IsSupported)
        {
            // Inspired by clang's implementation...
            var thisVec = _value.AsVector128();
            var otherVec = bounds._value.AsVector128();
            var a = Sse41.Blend(thisVec, otherVec, 3); // (other.tl, this.br)
            var b = Sse41.Blend(otherVec, thisVec, 3); // (this.tl, other.br)
            var cmp = Sse.CompareLessThan(a, b);       // (this.tl > other.tl, this.br < other.br)
            var res = Sse41.BlendVariable(thisVec, otherVec, cmp); // (this > other ? other.tl : this.tl, this < other ? other.br : this.br)
            return new(res.AsVector4());
        }
#endif
        return new Bounds2D(Vector2.Min(topLeft, bounds.topLeft), Vector2.Max(bottomRight, bounds.bottomRight));
    }

    /// <summary>
    /// Returns <see langword="true"/> if the <see cref="topLeft"/> coordinate is less than or equal to the <see cref="bottomRight"/> coordinate.
    /// </summary>
    public readonly bool IsValid => topLeft.X <= bottomRight.X && topLeft.Y <= bottomRight.Y;

    /// <summary>
    /// Shrinks the bounds by the given margin, returning the shrunken bounds.
    /// </summary>
    /// <param name="margin">The amount in each direction to shrink the bounds by.</param>
    /// <returns>A new bounds which has been shrunk.</returns>
    public readonly Bounds2D SubtractMargin(Thickness margin)
    {
        var add = new Vector4(margin.leftTop, -margin.right, -margin.bottom);
        var res = _value + add;
        var ret = new Bounds2D(res);
        if (!ret.IsValid)
            ret.topLeft = ret.bottomRight = ret.Centre;
        return ret;
    }

    /// <summary>
    /// Grows the bounds by the given margin, returning the grown bounds.
    /// </summary>
    /// <param name="margin">The amount in each direction to grow the bounds by.</param>
    /// <returns>A new bounds which has been grown.</returns>
    public readonly Bounds2D AddMargin(Thickness margin)
    {
        var add = new Vector4(-margin.leftTop, margin.right, margin.bottom);
        var res = _value + add;
        var ret = new Bounds2D(res);
        if (!ret.IsValid)
            ret.topLeft = ret.bottomRight = ret.Centre;
        return ret;
    }

    /// <summary>
    /// Creates a new <see cref="Bounds2D"/> with the same top-left coordinate as <see langword="this"/> 
    /// bounds, but with the given <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The final <see cref="Size"/> of the new bounds.</param>
    /// <returns>A new bounds with the given size.</returns>
    public readonly Bounds2D WithSize(Vector2 size)
    {
        var tl = topLeft;
        var br = size + tl;
        return new(tl, br);
    }

    /// <summary>
    /// Creates a new <see cref="Bounds2D"/> with the same top-left coordinate as <see langword="this"/> 
    /// bounds, but with the given <paramref name="size"/>. For any dimension of 
    /// <paramref name="size"/> less than or equal to zero, this bounds' original size is returned.
    /// </summary>
    /// <param name="size">The final <see cref="Size"/> of the new bounds.</param>
    /// <returns>A new bounds with the given size.</returns>
    public readonly Bounds2D WithSizeNonZero(Vector2 size)
    {
        var tl = topLeft;
        var br = size + tl;
        if (size.X <= 0)
            br.X = bottomRight.X;
        if (size.Y <= 0)
            br.Y = bottomRight.Y;
        return new(tl, br);
    }

    /// <summary>
    /// Creates a new <see cref="Bounds2D"/> with the same <see cref="Centre"/> coordinate as 
    /// <see langword="this"/> bounds, but with the given <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The final <see cref="Size"/> of the new bounds.</param>
    /// <returns>A new bounds with the given size.</returns>
    public readonly Bounds2D WithSizeCentred(Vector2 size)
    {
        var centre = bottomRight + topLeft;

        var sizeV4 = new Vector4(-size, size.X, size.Y);
        var res = new Vector4(centre, centre.X, centre.Y) + sizeV4;
        res *= 0.5f;

        return new(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Bounds2D other) => _value == other._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals(object? obj) => obj is Bounds2D b && Equals(b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bounds2D left, Bounds2D right) => left._value == right._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bounds2D left, Bounds2D right) => left._value != right._value;
    public readonly override int GetHashCode() => _value.GetHashCode();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bounds2D operator*(Bounds2D bounds, Vector2 scale)
    {
        //bounds.topLeft *= scale;
        //bounds.bottomRight *= scale;
        //return bounds;

        Vector4 scale4 = new(scale, scale.X, scale.Y);
        return new(bounds._value * scale4);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bounds2D operator +(Bounds2D bounds, Vector2 offset)
    {
        Vector4 offset4 = new(offset, offset.X, offset.Y);
        return new(bounds._value + offset4);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bounds2D operator -(Bounds2D bounds, Vector2 offset)
    {
        Vector4 offset4 = new(offset, offset.X, offset.Y);
        return new(bounds._value - offset4);
    }

    /// <summary>
    /// Constructs a new <see cref="Bounds2D"/> from a centre point and a total size.
    /// </summary>
    /// <param name="centre">The coordinates of the centre of the new bounds.</param>
    /// <param name="scale">The width and height of the new bounds.</param>
    /// <returns></returns>
    public static Bounds2D FromCentreScale(Vector2 centre, Vector2 scale)
    {
        var halfScale = scale * 0.5f;
        Vector2 tl = centre - halfScale;
        Vector2 br = centre + halfScale;
        return new(tl, br);
    }

    /// <summary>
    /// Constructs a new <see cref="Bounds2D"/> from a top-left and a bottom-right corner position.
    /// </summary>
    /// <param name="topLeft">The coordinates of the top-left point within the new bounds.</param>
    /// <param name="bottomRight">The coordinates of the bottom-right point within the new bounds.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bounds2D FromCorners(Vector2 topLeft, Vector2 bottomRight) => new(topLeft, bottomRight);
}
