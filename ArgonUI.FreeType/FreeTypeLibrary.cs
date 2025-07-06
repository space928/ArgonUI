using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using FreeType.Bindings;

namespace ArgonUI.FreeType;

public unsafe struct FreeTypeLibrary : IDisposable
{
    internal FT_LibraryRec_* lib;

    public bool IsValid => lib != null;

    public FreeTypeLibrary()
    {
        //Debugger.Break();
        FT_LibraryRec_* lib_temp;
        CheckError(Methods.Init_FreeType(&lib_temp));
        lib = lib_temp;
        //Console.WriteLine($"Lib: 0x{(nint)ret_ptr:X8}");
    }

    /// <summary>
    /// Gets the version of the native FreeType library which is installed.
    /// </summary>
    /// <returns></returns>
    public (int major, int minor, int patch) GetVersion()
    {
        int major, minor, patch;
        Console.WriteLine($"Lib: 0x{(nint)lib:X8}");
        Methods.Library_Version(lib, &major, &minor, &patch);
        return (major, minor, patch);
    }

    public void Test()
    {
        FT_FaceRec_* face;
        CheckError(Methods.New_Memory_Face(lib, null, (nint)0, (nint)0, &face));
        CheckError(Methods.Set_Char_Size(face, (nint)0, (nint)(16 * 64), 72, 72));
        var glyphInd = Methods.Get_Char_Index(face, (UIntPtr)'Q');
        CheckError(Methods.Load_Glyph(face, glyphInd, 0));
        CheckError(Methods.Render_Glyph(face->glyph, FT_Render_Mode_.FT_RENDER_MODE_LCD));

        //face->glyph->bitmap->
    }

    public FreeTypeFace LoadFace(Memory<byte> fontFile)
    {
        FT_FaceRec_* face;
        using var filePtr = fontFile.Pin();
        CheckError(Methods.New_Memory_Face(lib, (byte*)filePtr.Pointer, (nint)fontFile.Length, (nint)0, &face));
        return new FreeTypeFace(lib, face);
    }

    public FreeTypeFace LoadFace(string fontFile)
    {
        FT_FaceRec_* face;
        var fileName = Encoding.ASCII.GetBytes(fontFile);
        fixed (byte* fname = fileName)
        {
            CheckError(Methods.New_Face(lib, (sbyte*)fname, (nint)0, &face));
        }
        return new FreeTypeFace(lib, face);
    }

    internal static void CheckError(int ftError)
    {
        if (ftError != 0)
        {
            // Try get error string
            string? msg = null;
            try
            {
                var str = Methods.Error_String(ftError);
                msg = Encoding.ASCII.GetString((byte*)str, 1024);
                //Marshal.FreeHGlobal((nint)str);
            }
            catch { }
            if (msg != null)
                throw new Exception($"[FreeType] {msg}");
            else
                throw new Exception($"[FreeType] Through an exception: 0x{ftError:X8}");
        }
    }

    public void Dispose()
    {
        if (lib != null)
        {
            CheckError(Methods.Done_FreeType(lib));
            lib = null;
        }
    }
}
