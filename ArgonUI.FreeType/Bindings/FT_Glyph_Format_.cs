using System;

namespace FreeType.Bindings;

public enum FT_Glyph_Format_
{
    FT_GLYPH_FORMAT_NONE = unchecked((int)(((nuint)((byte)(0)) << 24) | ((nuint)((byte)(0)) << 16) | ((nuint)((byte)(0)) << 8) | (nuint)((byte)(0)))),
    FT_GLYPH_FORMAT_COMPOSITE = unchecked((int)(((nuint)((byte)('c')) << 24) | ((nuint)((byte)('o')) << 16) | ((nuint)((byte)('m')) << 8) | (nuint)((byte)('p')))),
    FT_GLYPH_FORMAT_BITMAP = unchecked((int)(((nuint)((byte)('b')) << 24) | ((nuint)((byte)('i')) << 16) | ((nuint)((byte)('t')) << 8) | (nuint)((byte)('s')))),
    FT_GLYPH_FORMAT_OUTLINE = unchecked((int)(((nuint)((byte)('o')) << 24) | ((nuint)((byte)('u')) << 16) | ((nuint)((byte)('t')) << 8) | (nuint)((byte)('l')))),
    FT_GLYPH_FORMAT_PLOTTER = unchecked((int)(((nuint)((byte)('p')) << 24) | ((nuint)((byte)('l')) << 16) | ((nuint)((byte)('o')) << 8) | (nuint)((byte)('t')))),
    FT_GLYPH_FORMAT_SVG = unchecked((int)(((nuint)((byte)('S')) << 24) | ((nuint)((byte)('V')) << 16) | ((nuint)((byte)('G')) << 8) | (nuint)((byte)(' ')))),
}
