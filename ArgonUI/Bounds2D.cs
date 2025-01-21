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

    public Bounds2D() { }

    public Bounds2D(Vector2 topLeft, Vector2 bottomRight)
    {
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
    }

    public Bounds2D(float left, float right, float top, float bottom) : this(new(left, top), new(right, bottom)) { }

    public readonly float Width => bottomRight.X - topLeft.X;
    public readonly float Height => bottomRight.Y - topLeft.Y;
}
