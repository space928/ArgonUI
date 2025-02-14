using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

public struct Bounds2D
{
    public Vector2 topLeft;
    public Vector2 bottomRight;

    public static readonly Bounds2D Zero = new();

    public Bounds2D() { }

    public Bounds2D(Vector2 topLeft, Vector2 bottomRight)
    {
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
    }

    public Bounds2D(float left, float right, float top, float bottom) : this(new(left, top), new(right, bottom)) { }

    public readonly float Width => bottomRight.X - topLeft.X;
    public readonly float Height => bottomRight.Y - topLeft.Y;

    /// <summary>
    /// Checks if the given point is inside or on the bounds.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public readonly bool Contains(Vector2 point)
    {
        return point.X >= topLeft.X && point.Y >= topLeft.Y && point.X <= bottomRight.X && point.Y <= bottomRight.Y;
    }

    /// <summary>
    /// Checks if the given bounds are fully contained by this bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly bool Contains(Bounds2D bounds)
    {
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
        return Contains(bounds.topLeft) || Contains(bounds.bottomRight);
    }

    /// <summary>
    /// Computes the intersection bounds between this and the given bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly Bounds2D Intersect(Bounds2D bounds)
    {
        return new Bounds2D(Vector2.Max(topLeft, bounds.topLeft), Vector2.Min(bottomRight, bounds.bottomRight));
    }

    /// <summary>
    /// Returns a bounds which encapsulates both this bounds and the given bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public readonly Bounds2D Union(Bounds2D bounds)
    {
        return new Bounds2D(Vector2.Min(topLeft, bounds.topLeft), Vector2.Max(bottomRight, bounds.bottomRight));
    }

    /// <summary>
    /// Returns <see langword="true"/> if the <see cref="topLeft"/> coordinate is less than or equal to the <see cref="bottomRight"/> coordinate.
    /// </summary>
    public readonly bool IsValid => topLeft.X <= bottomRight.X && topLeft.Y <= bottomRight.Y;
}
