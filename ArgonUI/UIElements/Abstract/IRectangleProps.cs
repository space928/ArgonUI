using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArgonUI.UIElements.Abstract;

/// <summary>
/// Represents the properties which a generic rectangle should expose.
/// </summary>
/// <remarks>
/// This interface exists to help ensure that <see cref="UIElement"/>s which draw a rectangle have feature and property name parity.
/// </remarks>
internal interface IRectangleProps
{
    /// <summary>
    /// The colour of this rectangle.
    /// </summary>
    public Vector4 Colour { get; set; }
    /// <summary>
    /// The outline colour.
    /// </summary>
    public Vector4 OutlineColour { get; set; }
    /// <summary>
    /// The thickness of the outline in pixels.
    /// </summary>
    public float OutlineThickness { get; set; }
    /// <summary>
    /// The radius of the corners of this rectangle.
    /// </summary>
    public float Rounding { get; set; }
    /// <summary>
    /// The texture to fill the rectangle with.
    /// </summary>
    public ArgonTexture? Texture { get; set; }
    /// <summary>
    /// The gradient to fill the rectangle with.
    /// </summary>
    public Gradient? GradientFill { get; set; }
}
