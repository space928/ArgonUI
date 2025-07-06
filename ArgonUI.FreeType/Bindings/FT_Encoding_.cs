namespace FreeType.Bindings;

public enum FT_Encoding_
{
    FT_ENCODING_NONE = unchecked((int)(((uint)((byte)(0)) << 24) | ((uint)((byte)(0)) << 16) | ((uint)((byte)(0)) << 8) | (uint)((byte)(0)))),
    FT_ENCODING_MS_SYMBOL = unchecked((int)(((uint)((byte)('s')) << 24) | ((uint)((byte)('y')) << 16) | ((uint)((byte)('m')) << 8) | (uint)((byte)('b')))),
    FT_ENCODING_UNICODE = unchecked((int)(((uint)((byte)('u')) << 24) | ((uint)((byte)('n')) << 16) | ((uint)((byte)('i')) << 8) | (uint)((byte)('c')))),
    FT_ENCODING_SJIS = unchecked((int)(((uint)((byte)('s')) << 24) | ((uint)((byte)('j')) << 16) | ((uint)((byte)('i')) << 8) | (uint)((byte)('s')))),
    FT_ENCODING_PRC = unchecked((int)(((uint)((byte)('g')) << 24) | ((uint)((byte)('b')) << 16) | ((uint)((byte)(' ')) << 8) | (uint)((byte)(' ')))),
    FT_ENCODING_BIG5 = unchecked((int)(((uint)((byte)('b')) << 24) | ((uint)((byte)('i')) << 16) | ((uint)((byte)('g')) << 8) | (uint)((byte)('5')))),
    FT_ENCODING_WANSUNG = unchecked((int)(((uint)((byte)('w')) << 24) | ((uint)((byte)('a')) << 16) | ((uint)((byte)('n')) << 8) | (uint)((byte)('s')))),
    FT_ENCODING_JOHAB = unchecked((int)(((uint)((byte)('j')) << 24) | ((uint)((byte)('o')) << 16) | ((uint)((byte)('h')) << 8) | (uint)((byte)('a')))),
    FT_ENCODING_GB2312 = FT_ENCODING_PRC,
    FT_ENCODING_MS_SJIS = FT_ENCODING_SJIS,
    FT_ENCODING_MS_GB2312 = FT_ENCODING_PRC,
    FT_ENCODING_MS_BIG5 = FT_ENCODING_BIG5,
    FT_ENCODING_MS_WANSUNG = FT_ENCODING_WANSUNG,
    FT_ENCODING_MS_JOHAB = FT_ENCODING_JOHAB,
    FT_ENCODING_ADOBE_STANDARD = unchecked((int)(((uint)((byte)('A')) << 24) | ((uint)((byte)('D')) << 16) | ((uint)((byte)('O')) << 8) | (uint)((byte)('B')))),
    FT_ENCODING_ADOBE_EXPERT = unchecked((int)(((uint)((byte)('A')) << 24) | ((uint)((byte)('D')) << 16) | ((uint)((byte)('B')) << 8) | (uint)((byte)('E')))),
    FT_ENCODING_ADOBE_CUSTOM = unchecked((int)(((uint)((byte)('A')) << 24) | ((uint)((byte)('D')) << 16) | ((uint)((byte)('B')) << 8) | (uint)((byte)('C')))),
    FT_ENCODING_ADOBE_LATIN_1 = unchecked((int)(((uint)((byte)('l')) << 24) | ((uint)((byte)('a')) << 16) | ((uint)((byte)('t')) << 8) | (uint)((byte)('1')))),
    FT_ENCODING_OLD_LATIN_2 = unchecked((int)(((uint)((byte)('l')) << 24) | ((uint)((byte)('a')) << 16) | ((uint)((byte)('t')) << 8) | (uint)((byte)('2')))),
    FT_ENCODING_APPLE_ROMAN = unchecked((int)(((uint)((byte)('a')) << 24) | ((uint)((byte)('r')) << 16) | ((uint)((byte)('m')) << 8) | (uint)((byte)('n')))),
}
