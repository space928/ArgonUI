using ArgonUI.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.Drawing;

/// <summary>
/// Represents a gradient which can be applied to a UI element.
/// </summary>
public partial class Gradient : ReactiveObject
{
    [Reactive] private Vector4 colourTL;
    [Reactive] private Vector4 colourTR;
    [Reactive] private Vector4 colourBL;
    [Reactive] private Vector4 colourBR;

    /// <summary>
    /// Creates a new 4-point gradient.
    /// </summary>
    /// <param name="colourTL">The top-left colour.</param>
    /// <param name="colourTR">The top-right colour.</param>
    /// <param name="colourBL">The bottom-left colour.</param>
    /// <param name="colourBR">The bottom-right colour.</param>
    public Gradient(Vector4 colourTL, Vector4 colourTR, Vector4 colourBL, Vector4 colourBR)
    {
        this.colourTL = colourTL;
        this.colourTR = colourTR;
        this.colourBL = colourBL;
        this.colourBR = colourBR;
    }

    /// <summary>
    /// Creates a new horizontal gradient from a start and end colour.
    /// </summary>
    /// <param name="colourLeft"></param>
    /// <param name="colourRight"></param>
    /// <returns></returns>
    public static Gradient CreateHorizontal(Vector4 colourLeft, Vector4 colourRight) => new(colourLeft, colourRight, colourLeft, colourRight);
    /// <summary>
    /// Creates a new vertical gradient from a start and end colour.
    /// </summary>
    /// <param name="colourTop"></param>
    /// <param name="colourBottom"></param>
    /// <returns></returns>
    public static Gradient CreateVertical(Vector4 colourTop, Vector4 colourBottom) => new(colourTop, colourTop, colourBottom, colourBottom);
    /// <summary>
    /// Creates a new diagonal gradient going from the top-left corner to the bottom-right corner.
    /// </summary>
    /// <param name="colourTopLeft"></param>
    /// <param name="colourBottomRight"></param>
    /// <returns></returns>
    public static Gradient CreateDiagonalTopLeft(Vector4 colourTopLeft, Vector4 colourBottomRight)
    {
        var mid = (colourTopLeft + colourBottomRight) * 0.5f;
        return new(colourTopLeft, mid, mid, colourBottomRight);
    }
    /// <summary>
    /// Creates a new diagonal gradient going from the bottom-left corner to the top-right corner.
    /// </summary>
    /// <param name="colourBottomLeft"></param>
    /// <param name="colourTopRight"></param>
    /// <returns></returns>
    public static Gradient CreateDiagonalBottomLeft(Vector4 colourBottomLeft, Vector4 colourTopRight)
    {
        var mid = (colourBottomLeft + colourTopRight) * 0.5f;
        return new(mid, colourTopRight, colourBottomLeft, mid);
    }
    /// <summary>
    /// Creates a new 4-point gradient.
    /// </summary>
    /// <param name="colourTL">The top-left colour.</param>
    /// <param name="colourTR">The top-right colour.</param>
    /// <param name="colourBL">The bottom-left colour.</param>
    /// <param name="colourBR">The bottom-right colour.</param>
    /// <returns></returns>
    public static Gradient CreateFourCorner(Vector4 colourTL, Vector4 colourTR, Vector4 colourBL, Vector4 colourBR) => new(colourTL, colourTR, colourBL, colourBR);
}
