using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

// TODO: This seems like a premature optimisation, maybe explore in the future?

[StructLayout(LayoutKind.Explicit)]
public struct DrawCommand
{
    [FieldOffset(0)] public DrawCommands command;
    [FieldOffset(4)] public DrawRectParams rectParams;
    [FieldOffset(4)] public DrawRectParams textureParams;
}

public struct DrawRectParams
{

}

public struct DrawTextureParams
{

}

public enum DrawCommands
{
    NoOp,
    DrawRect,
    DrawTexture,
    DrawGradient,
    DrawText,
    //DrawTextBlurred
}
