using System;
using System.Diagnostics;
using FreeType.Bindings;

namespace ArgonUI.FreeType;

public unsafe struct FreeTypeFace : IDisposable
{
    private readonly FT_LibraryRec_* lib;
    private FT_FaceRec_* face;

    internal FreeTypeFace(FT_LibraryRec_* lib, FT_FaceRec_* face)
    {
        this.lib = lib;
        this.face = face;

        SetCharSize(14);
    }

    public void SetCharSize(int height, int width = 0, int resolution = 72)
    {
        FreeTypeLibrary.CheckError(Methods.Set_Char_Size(face, (nint)width, (nint)height, (uint)resolution, 0));
    }

    public int GetCharIndex(char c)
    {
        return (int)Methods.Get_Char_Index(face, (UIntPtr)c);
    }

    public (int width, int height) RenderGlyph(char c, Memory<byte> dst, FT_Render_Mode_ renderMode = FT_Render_Mode_.FT_RENDER_MODE_MONO)
    {
        //Debugger.Break();
        //var glyphInd = Methods.Get_Char_Index(face, (UIntPtr)c);
        //FreeTypeLibrary.CheckError(Methods.Load_Glyph(face, glyphInd, 0));
        //FreeTypeLibrary.CheckError(Methods.Render_Glyph(face->glyph, renderMode));

        FreeTypeLibrary.CheckError(Methods.Load_Char(face, (UIntPtr)c, (int)FT_LoadType.FT_LOAD_RENDER));

        using var handle = dst.Pin();
        var src = face->glyph->bitmap;
        for (int row = 0; row < src.rows; row++)
        {
            Buffer.MemoryCopy(src.buffer + src.pitch * row, (byte*)handle.Pointer + src.width * row, dst.Length, src.width);
        }
        return ((int)src.width, (int)src.rows);
    }

    public void Dispose()
    {
        if (face != null)
        {
            FreeTypeLibrary.CheckError(Methods.Done_Face(face));
            face = null;
        }
    }
}

public enum FT_LoadType : int
{
    FT_LOAD_DEFAULT                      = 0x0,
    FT_LOAD_NO_SCALE                     = ( 1 << 0  ),
    FT_LOAD_NO_HINTING                   = ( 1 << 1  ),
    FT_LOAD_RENDER                       = ( 1 << 2  ),
    FT_LOAD_NO_BITMAP                    = ( 1 << 3  ),
    FT_LOAD_VERTICAL_LAYOUT              = ( 1 << 4  ),
    FT_LOAD_FORCE_AUTOHINT               = ( 1 << 5  ),
    FT_LOAD_CROP_BITMAP                  = ( 1 << 6  ),
    FT_LOAD_PEDANTIC                     = ( 1 << 7  ),
    FT_LOAD_IGNORE_GLOBAL_ADVANCE_WIDTH  = ( 1 << 9  ),
    FT_LOAD_NO_RECURSE                   = ( 1 << 10 ),
    FT_LOAD_IGNORE_TRANSFORM             = ( 1 << 11 ),
    FT_LOAD_MONOCHROME                   = ( 1 << 12 ),
    FT_LOAD_LINEAR_DESIGN                = ( 1 << 13 ),
    FT_LOAD_SBITS_ONLY                   = ( 1 << 14 ),
    FT_LOAD_NO_AUTOHINT                  = ( 1 << 15 ),
    /* Bits 16-19 are used by `FT_LOAD_TARGET_` */
    FT_LOAD_COLOR                        = ( 1 << 20 ),
    FT_LOAD_COMPUTE_METRICS              = ( 1 << 21 ),
    FT_LOAD_BITMAP_METRICS_ONLY          = ( 1 << 22 ),
    FT_LOAD_NO_SVG                       = ( 1 << 24 ),

    /* */

    /* used internally only by certain font drivers */
    FT_LOAD_ADVANCE_ONLY                 = ( 1 << 8  ),
    FT_LOAD_SVG_ONLY                     = ( 1 << 23 ),
}
