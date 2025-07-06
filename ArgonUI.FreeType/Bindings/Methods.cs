using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

public static unsafe partial class Methods
{
    public const int FT_Mod_Err_Base = 0;
    public const int FT_Mod_Err_Autofit = 0;
    public const int FT_Mod_Err_BDF = 0;
    public const int FT_Mod_Err_Bzip2 = 0;
    public const int FT_Mod_Err_Cache = 0;
    public const int FT_Mod_Err_CFF = 0;
    public const int FT_Mod_Err_CID = 0;
    public const int FT_Mod_Err_Gzip = 0;
    public const int FT_Mod_Err_LZW = 0;
    public const int FT_Mod_Err_OTvalid = 0;
    public const int FT_Mod_Err_PCF = 0;
    public const int FT_Mod_Err_PFR = 0;
    public const int FT_Mod_Err_PSaux = 0;
    public const int FT_Mod_Err_PShinter = 0;
    public const int FT_Mod_Err_PSnames = 0;
    public const int FT_Mod_Err_Raster = 0;
    public const int FT_Mod_Err_SFNT = 0;
    public const int FT_Mod_Err_Smooth = 0;
    public const int FT_Mod_Err_TrueType = 0;
    public const int FT_Mod_Err_Type1 = 0;
    public const int FT_Mod_Err_Type42 = 0;
    public const int FT_Mod_Err_Winfonts = 0;
    public const int FT_Mod_Err_GXvalid = 0;
    public const int FT_Mod_Err_Sdf = 0;
    public const int FT_Mod_Err_Max = 1;

    public const int FT_Err_Ok = 0x00;
    public const int FT_Err_Cannot_Open_Resource = 0x01 + 0;
    public const int FT_Err_Unknown_File_Format = 0x02 + 0;
    public const int FT_Err_Invalid_File_Format = 0x03 + 0;
    public const int FT_Err_Invalid_Version = 0x04 + 0;
    public const int FT_Err_Lower_Module_Version = 0x05 + 0;
    public const int FT_Err_Invalid_Argument = 0x06 + 0;
    public const int FT_Err_Unimplemented_Feature = 0x07 + 0;
    public const int FT_Err_Invalid_Table = 0x08 + 0;
    public const int FT_Err_Invalid_Offset = 0x09 + 0;
    public const int FT_Err_Array_Too_Large = 0x0A + 0;
    public const int FT_Err_Missing_Module = 0x0B + 0;
    public const int FT_Err_Missing_Property = 0x0C + 0;
    public const int FT_Err_Invalid_Glyph_Index = 0x10 + 0;
    public const int FT_Err_Invalid_Character_Code = 0x11 + 0;
    public const int FT_Err_Invalid_Glyph_Format = 0x12 + 0;
    public const int FT_Err_Cannot_Render_Glyph = 0x13 + 0;
    public const int FT_Err_Invalid_Outline = 0x14 + 0;
    public const int FT_Err_Invalid_Composite = 0x15 + 0;
    public const int FT_Err_Too_Many_Hints = 0x16 + 0;
    public const int FT_Err_Invalid_Pixel_Size = 0x17 + 0;
    public const int FT_Err_Invalid_SVG_Document = 0x18 + 0;
    public const int FT_Err_Invalid_Handle = 0x20 + 0;
    public const int FT_Err_Invalid_Library_Handle = 0x21 + 0;
    public const int FT_Err_Invalid_Driver_Handle = 0x22 + 0;
    public const int FT_Err_Invalid_Face_Handle = 0x23 + 0;
    public const int FT_Err_Invalid_Size_Handle = 0x24 + 0;
    public const int FT_Err_Invalid_Slot_Handle = 0x25 + 0;
    public const int FT_Err_Invalid_CharMap_Handle = 0x26 + 0;
    public const int FT_Err_Invalid_Cache_Handle = 0x27 + 0;
    public const int FT_Err_Invalid_Stream_Handle = 0x28 + 0;
    public const int FT_Err_Too_Many_Drivers = 0x30 + 0;
    public const int FT_Err_Too_Many_Extensions = 0x31 + 0;
    public const int FT_Err_Out_Of_Memory = 0x40 + 0;
    public const int FT_Err_Unlisted_Object = 0x41 + 0;
    public const int FT_Err_Cannot_Open_Stream = 0x51 + 0;
    public const int FT_Err_Invalid_Stream_Seek = 0x52 + 0;
    public const int FT_Err_Invalid_Stream_Skip = 0x53 + 0;
    public const int FT_Err_Invalid_Stream_Read = 0x54 + 0;
    public const int FT_Err_Invalid_Stream_Operation = 0x55 + 0;
    public const int FT_Err_Invalid_Frame_Operation = 0x56 + 0;
    public const int FT_Err_Nested_Frame_Access = 0x57 + 0;
    public const int FT_Err_Invalid_Frame_Read = 0x58 + 0;
    public const int FT_Err_Raster_Uninitialized = 0x60 + 0;
    public const int FT_Err_Raster_Corrupted = 0x61 + 0;
    public const int FT_Err_Raster_Overflow = 0x62 + 0;
    public const int FT_Err_Raster_Negative_Height = 0x63 + 0;
    public const int FT_Err_Too_Many_Caches = 0x70 + 0;
    public const int FT_Err_Invalid_Opcode = 0x80 + 0;
    public const int FT_Err_Too_Few_Arguments = 0x81 + 0;
    public const int FT_Err_Stack_Overflow = 0x82 + 0;
    public const int FT_Err_Code_Overflow = 0x83 + 0;
    public const int FT_Err_Bad_Argument = 0x84 + 0;
    public const int FT_Err_Divide_By_Zero = 0x85 + 0;
    public const int FT_Err_Invalid_Reference = 0x86 + 0;
    public const int FT_Err_Debug_OpCode = 0x87 + 0;
    public const int FT_Err_ENDF_In_Exec_Stream = 0x88 + 0;
    public const int FT_Err_Nested_DEFS = 0x89 + 0;
    public const int FT_Err_Invalid_CodeRange = 0x8A + 0;
    public const int FT_Err_Execution_Too_Long = 0x8B + 0;
    public const int FT_Err_Too_Many_Function_Defs = 0x8C + 0;
    public const int FT_Err_Too_Many_Instruction_Defs = 0x8D + 0;
    public const int FT_Err_Table_Missing = 0x8E + 0;
    public const int FT_Err_Horiz_Header_Missing = 0x8F + 0;
    public const int FT_Err_Locations_Missing = 0x90 + 0;
    public const int FT_Err_Name_Table_Missing = 0x91 + 0;
    public const int FT_Err_CMap_Table_Missing = 0x92 + 0;
    public const int FT_Err_Hmtx_Table_Missing = 0x93 + 0;
    public const int FT_Err_Post_Table_Missing = 0x94 + 0;
    public const int FT_Err_Invalid_Horiz_Metrics = 0x95 + 0;
    public const int FT_Err_Invalid_CharMap_Format = 0x96 + 0;
    public const int FT_Err_Invalid_PPem = 0x97 + 0;
    public const int FT_Err_Invalid_Vert_Metrics = 0x98 + 0;
    public const int FT_Err_Could_Not_Find_Context = 0x99 + 0;
    public const int FT_Err_Invalid_Post_Table_Format = 0x9A + 0;
    public const int FT_Err_Invalid_Post_Table = 0x9B + 0;
    public const int FT_Err_DEF_In_Glyf_Bytecode = 0x9C + 0;
    public const int FT_Err_Missing_Bitmap = 0x9D + 0;
    public const int FT_Err_Missing_SVG_Hooks = 0x9E + 0;
    public const int FT_Err_Syntax_Error = 0xA0 + 0;
    public const int FT_Err_Stack_Underflow = 0xA1 + 0;
    public const int FT_Err_Ignore = 0xA2 + 0;
    public const int FT_Err_No_Unicode_Glyph_Name = 0xA3 + 0;
    public const int FT_Err_Glyph_Too_Big = 0xA4 + 0;
    public const int FT_Err_Missing_Startfont_Field = 0xB0 + 0;
    public const int FT_Err_Missing_Font_Field = 0xB1 + 0;
    public const int FT_Err_Missing_Size_Field = 0xB2 + 0;
    public const int FT_Err_Missing_Fontboundingbox_Field = 0xB3 + 0;
    public const int FT_Err_Missing_Chars_Field = 0xB4 + 0;
    public const int FT_Err_Missing_Startchar_Field = 0xB5 + 0;
    public const int FT_Err_Missing_Encoding_Field = 0xB6 + 0;
    public const int FT_Err_Missing_Bbx_Field = 0xB7 + 0;
    public const int FT_Err_Bbx_Too_Big = 0xB8 + 0;
    public const int FT_Err_Corrupted_Font_Header = 0xB9 + 0;
    public const int FT_Err_Corrupted_Font_Glyphs = 0xBA + 0;
    public const int FT_Err_Max = 187;

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Error_String", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* Error_String([NativeTypeName("FT_Error")] int error_code);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Init_FreeType", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Init_FreeType([NativeTypeName("FT_Library *")] FT_LibraryRec_** alibrary);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_FreeType", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Done_FreeType([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Face", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int New_Face([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const char *")] sbyte* filepathname, [NativeTypeName("FT_Long")] IntPtr face_index, [NativeTypeName("FT_Face *")] FT_FaceRec_** aface);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Memory_Face", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int New_Memory_Face([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Byte *")] byte* file_base, [NativeTypeName("FT_Long")] IntPtr file_size, [NativeTypeName("FT_Long")] IntPtr face_index, [NativeTypeName("FT_Face *")] FT_FaceRec_** aface);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Open_Face", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Open_Face([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Open_Args *")] FT_Open_Args_* args, [NativeTypeName("FT_Long")] IntPtr face_index, [NativeTypeName("FT_Face *")] FT_FaceRec_** aface);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Attach_File", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Attach_File([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const char *")] sbyte* filepathname);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Attach_Stream", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Attach_Stream([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const FT_Open_Args *")] FT_Open_Args_* parameters);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Reference_Face", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Reference_Face([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Face", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Done_Face([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Select_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Select_Size([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Int")] int strike_index);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Request_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Request_Size([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Size_Request")] FT_Size_RequestRec_* req);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Char_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Char_Size([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_F26Dot6")] IntPtr char_width, [NativeTypeName("FT_F26Dot6")] IntPtr char_height, [NativeTypeName("FT_UInt")] uint horz_resolution, [NativeTypeName("FT_UInt")] uint vert_resolution);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Pixel_Sizes", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Pixel_Sizes([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint pixel_width, [NativeTypeName("FT_UInt")] uint pixel_height);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Load_Glyph", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Load_Glyph([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint glyph_index, [NativeTypeName("FT_Int32")] int load_flags);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Load_Char", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Load_Char([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr char_code, [NativeTypeName("FT_Int32")] int load_flags);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Transform", ExactSpelling = true)]
    public static extern void Set_Transform([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Matrix *")] FT_Matrix_* matrix, [NativeTypeName("FT_Vector *")] FT_Vector_* delta);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Transform", ExactSpelling = true)]
    public static extern void Get_Transform([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Matrix *")] FT_Matrix_* matrix, [NativeTypeName("FT_Vector *")] FT_Vector_* delta);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Render_Glyph", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Render_Glyph([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Render_Mode")] FT_Render_Mode_ render_mode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Kerning", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Kerning([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint left_glyph, [NativeTypeName("FT_UInt")] uint right_glyph, [NativeTypeName("FT_UInt")] uint kern_mode, [NativeTypeName("FT_Vector *")] FT_Vector_* akerning);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Track_Kerning", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Track_Kerning([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Fixed")] IntPtr point_size, [NativeTypeName("FT_Int")] int degree, [NativeTypeName("FT_Fixed *")] IntPtr* akerning);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Select_Charmap", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Select_Charmap([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Encoding")] FT_Encoding_ encoding);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Charmap", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Charmap([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_CharMap")] FT_CharMapRec_* charmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Charmap_Index", ExactSpelling = true)]
    [return: NativeTypeName("FT_Int")]
    public static extern int Get_Charmap_Index([NativeTypeName("FT_CharMap")] FT_CharMapRec_* charmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Char_Index", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt")]
    public static extern uint Get_Char_Index([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr charcode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_First_Char", ExactSpelling = true)]
    [return: NativeTypeName("FT_ULong")]
    public static extern UIntPtr Get_First_Char([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt *")] uint* agindex);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Next_Char", ExactSpelling = true)]
    [return: NativeTypeName("FT_ULong")]
    public static extern UIntPtr Get_Next_Char([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr char_code, [NativeTypeName("FT_UInt *")] uint* agindex);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_Properties", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Face_Properties([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_properties, [NativeTypeName("FT_Parameter *")] FT_Parameter_* properties);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Name_Index", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt")]
    public static extern uint Get_Name_Index([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const FT_String *")] sbyte* glyph_name);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Glyph_Name", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Glyph_Name([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint glyph_index, [NativeTypeName("FT_Pointer")] void* buffer, [NativeTypeName("FT_UInt")] uint buffer_max);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Postscript_Name", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* Get_Postscript_Name([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_SubGlyph_Info", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_SubGlyph_Info([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* glyph, [NativeTypeName("FT_UInt")] uint sub_index, [NativeTypeName("FT_Int *")] int* p_index, [NativeTypeName("FT_UInt *")] uint* p_flags, [NativeTypeName("FT_Int *")] int* p_arg1, [NativeTypeName("FT_Int *")] int* p_arg2, [NativeTypeName("FT_Matrix *")] FT_Matrix_* p_transform);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_FSType_Flags", ExactSpelling = true)]
    [return: NativeTypeName("FT_UShort")]
    public static extern ushort Get_FSType_Flags([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_GetCharVariantIndex", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt")]
    public static extern uint Face_GetCharVariantIndex([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr charcode, [NativeTypeName("FT_ULong")] UIntPtr variantSelector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_GetCharVariantIsDefault", ExactSpelling = true)]
    [return: NativeTypeName("FT_Int")]
    public static extern int Face_GetCharVariantIsDefault([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr charcode, [NativeTypeName("FT_ULong")] UIntPtr variantSelector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_GetVariantSelectors", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt32 *")]
    public static extern uint* Face_GetVariantSelectors([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_GetVariantsOfChar", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt32 *")]
    public static extern uint* Face_GetVariantsOfChar([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr charcode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_GetCharsOfVariant", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt32 *")]
    public static extern uint* Face_GetCharsOfVariant([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr variantSelector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_MulDiv", ExactSpelling = true)]
    [return: NativeTypeName("FT_Long")]
    public static extern IntPtr MulDiv([NativeTypeName("FT_Long")] IntPtr a, [NativeTypeName("FT_Long")] IntPtr b, [NativeTypeName("FT_Long")] IntPtr c);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_MulFix", ExactSpelling = true)]
    [return: NativeTypeName("FT_Long")]
    public static extern IntPtr MulFix([NativeTypeName("FT_Long")] IntPtr a, [NativeTypeName("FT_Long")] IntPtr b);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_DivFix", ExactSpelling = true)]
    [return: NativeTypeName("FT_Long")]
    public static extern IntPtr DivFix([NativeTypeName("FT_Long")] IntPtr a, [NativeTypeName("FT_Long")] IntPtr b);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_RoundFix", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr RoundFix([NativeTypeName("FT_Fixed")] IntPtr a);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_CeilFix", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr CeilFix([NativeTypeName("FT_Fixed")] IntPtr a);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_FloorFix", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr FloorFix([NativeTypeName("FT_Fixed")] IntPtr a);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_Transform", ExactSpelling = true)]
    public static extern void Vector_Transform([NativeTypeName("FT_Vector *")] FT_Vector_* vector, [NativeTypeName("const FT_Matrix *")] FT_Matrix_* matrix);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Library_Version", ExactSpelling = true)]
    public static extern void Library_Version([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Int *")] int* amajor, [NativeTypeName("FT_Int *")] int* aminor, [NativeTypeName("FT_Int *")] int* apatch);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_CheckTrueTypePatents", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Face_CheckTrueTypePatents([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Face_SetUnpatentedHinting", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Face_SetUnpatentedHinting([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Bool")] byte value);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Decompose", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Decompose([NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("const FT_Outline_Funcs *")] FT_Outline_Funcs_* func_interface, void* user);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_New", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_New([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_UInt")] uint numPoints, [NativeTypeName("FT_Int")] int numContours, [NativeTypeName("FT_Outline *")] FT_Outline_* anoutline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Done", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Done([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Check", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Check([NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Get_CBox", ExactSpelling = true)]
    public static extern void Outline_Get_CBox([NativeTypeName("const FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_BBox *")] FT_BBox_* acbox);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Translate", ExactSpelling = true)]
    public static extern void Outline_Translate([NativeTypeName("const FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_Pos")] IntPtr xOffset, [NativeTypeName("FT_Pos")] IntPtr yOffset);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Copy", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Copy([NativeTypeName("const FT_Outline *")] FT_Outline_* source, [NativeTypeName("FT_Outline *")] FT_Outline_* target);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Transform", ExactSpelling = true)]
    public static extern void Outline_Transform([NativeTypeName("const FT_Outline *")] FT_Outline_* outline, [NativeTypeName("const FT_Matrix *")] FT_Matrix_* matrix);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Embolden", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Embolden([NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_Pos")] IntPtr strength);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_EmboldenXY", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_EmboldenXY([NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_Pos")] IntPtr xstrength, [NativeTypeName("FT_Pos")] IntPtr ystrength);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Reverse", ExactSpelling = true)]
    public static extern void Outline_Reverse([NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Get_Bitmap", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Get_Bitmap([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("const FT_Bitmap *")] FT_Bitmap_* abitmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Render", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Render([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_Raster_Params *")] FT_Raster_Params_* @params);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Get_Orientation", ExactSpelling = true)]
    [return: NativeTypeName("FT_Orientation")]
    public static extern FT_Orientation_ Outline_Get_Orientation([NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int New_Size([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Size *")] FT_SizeRec_** size);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Done_Size([NativeTypeName("FT_Size")] FT_SizeRec_* size);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Activate_Size", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Activate_Size([NativeTypeName("FT_Size")] FT_SizeRec_* size);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Add_Module", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Add_Module([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Module_Class *")] FT_Module_Class_* clazz);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Module", ExactSpelling = true)]
    [return: NativeTypeName("FT_Module")]
    public static extern FT_ModuleRec_* Get_Module([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const char *")] sbyte* module_name);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Remove_Module", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Remove_Module([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Module")] FT_ModuleRec_* module);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Property_Set", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Property_Set([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_String *")] sbyte* module_name, [NativeTypeName("const FT_String *")] sbyte* property_name, [NativeTypeName("const void *")] void* value);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Property_Get", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Property_Get([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_String *")] sbyte* module_name, [NativeTypeName("const FT_String *")] sbyte* property_name, void* value);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Default_Properties", ExactSpelling = true)]
    public static extern void Set_Default_Properties([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Reference_Library", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Reference_Library([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Library", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int New_Library([NativeTypeName("FT_Memory")] FT_MemoryRec_* memory, [NativeTypeName("FT_Library *")] FT_LibraryRec_** alibrary);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Library", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Done_Library([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Debug_Hook", ExactSpelling = true)]
    public static extern void Set_Debug_Hook([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_UInt")] uint hook_index, [NativeTypeName("FT_DebugHook_Func")] IntPtr debug_hook);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Add_Default_Modules", ExactSpelling = true)]
    public static extern void Add_Default_Modules([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_TrueType_Engine_Type", ExactSpelling = true)]
    [return: NativeTypeName("FT_TrueTypeEngineType")]
    public static extern FT_TrueTypeEngineType_ Get_TrueType_Engine_Type([NativeTypeName("FT_Library")] FT_LibraryRec_* library);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Glyph", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int New_Glyph([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Glyph_Format")] FT_Glyph_Format_ format, [NativeTypeName("FT_Glyph *")] FT_GlyphRec_** aglyph);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Glyph", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Glyph([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Glyph *")] FT_GlyphRec_** aglyph);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_Copy", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Glyph_Copy([NativeTypeName("FT_Glyph")] FT_GlyphRec_* source, [NativeTypeName("FT_Glyph *")] FT_GlyphRec_** target);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_Transform", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Glyph_Transform([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph, [NativeTypeName("const FT_Matrix *")] FT_Matrix_* matrix, [NativeTypeName("const FT_Vector *")] FT_Vector_* delta);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_Get_CBox", ExactSpelling = true)]
    public static extern void Glyph_Get_CBox([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph, [NativeTypeName("FT_UInt")] uint bbox_mode, [NativeTypeName("FT_BBox *")] FT_BBox_* acbox);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_To_Bitmap", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Glyph_To_Bitmap([NativeTypeName("FT_Glyph *")] FT_GlyphRec_** the_glyph, [NativeTypeName("FT_Render_Mode")] FT_Render_Mode_ render_mode, [NativeTypeName("const FT_Vector *")] FT_Vector_* origin, [NativeTypeName("FT_Bool")] byte destroy);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Glyph", ExactSpelling = true)]
    public static extern void Done_Glyph([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Matrix_Multiply", ExactSpelling = true)]
    public static extern void Matrix_Multiply([NativeTypeName("const FT_Matrix *")] FT_Matrix_* a, [NativeTypeName("FT_Matrix *")] FT_Matrix_* b);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Matrix_Invert", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Matrix_Invert([NativeTypeName("FT_Matrix *")] FT_Matrix_* matrix);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Renderer", ExactSpelling = true)]
    [return: NativeTypeName("FT_Renderer")]
    public static extern FT_RendererRec_* Get_Renderer([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Glyph_Format")] FT_Glyph_Format_ format);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Renderer", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Renderer([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Renderer")] FT_RendererRec_* renderer, [NativeTypeName("FT_UInt")] uint num_params, [NativeTypeName("FT_Parameter *")] FT_Parameter_* parameters);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Has_PS_Glyph_Names", ExactSpelling = true)]
    [return: NativeTypeName("FT_Int")]
    public static extern int Has_PS_Glyph_Names([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PS_Font_Info", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_PS_Font_Info([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("PS_FontInfo")] PS_FontInfoRec_* afont_info);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PS_Font_Private", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_PS_Font_Private([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("PS_Private")] PS_PrivateRec_* afont_private);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PS_Font_Value", ExactSpelling = true)]
    [return: NativeTypeName("FT_Long")]
    public static extern IntPtr Get_PS_Font_Value([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("PS_Dict_Keys")] PS_Dict_Keys_ key, [NativeTypeName("FT_UInt")] uint idx, void* value, [NativeTypeName("FT_Long")] IntPtr value_len);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Sfnt_Table", ExactSpelling = true)]
    public static extern void* Get_Sfnt_Table([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Sfnt_Tag")] FT_Sfnt_Tag_ tag);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Load_Sfnt_Table", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Load_Sfnt_Table([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ULong")] UIntPtr tag, [NativeTypeName("FT_Long")] IntPtr offset, [NativeTypeName("FT_Byte *")] byte* buffer, [NativeTypeName("FT_ULong *")] UIntPtr* length);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Sfnt_Table_Info", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Sfnt_Table_Info([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint table_index, [NativeTypeName("FT_ULong *")] UIntPtr* tag, [NativeTypeName("FT_ULong *")] UIntPtr* length);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_CMap_Language_ID", ExactSpelling = true)]
    [return: NativeTypeName("FT_ULong")]
    public static extern UIntPtr Get_CMap_Language_ID([NativeTypeName("FT_CharMap")] FT_CharMapRec_* charmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_CMap_Format", ExactSpelling = true)]
    [return: NativeTypeName("FT_Long")]
    public static extern IntPtr Get_CMap_Format([NativeTypeName("FT_CharMap")] FT_CharMapRec_* charmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_BDF_Charset_ID", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_BDF_Charset_ID([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const char **")] sbyte** acharset_encoding, [NativeTypeName("const char **")] sbyte** acharset_registry);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_BDF_Property", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_BDF_Property([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const char *")] sbyte* prop_name, [NativeTypeName("BDF_PropertyRec *")] BDF_PropertyRec_* aproperty);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_CID_Registry_Ordering_Supplement", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_CID_Registry_Ordering_Supplement([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("const char **")] sbyte** registry, [NativeTypeName("const char **")] sbyte** ordering, [NativeTypeName("FT_Int *")] int* supplement);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_CID_Is_Internally_CID_Keyed", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_CID_Is_Internally_CID_Keyed([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Bool *")] byte* is_cid);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_CID_From_Glyph_Index", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_CID_From_Glyph_Index([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint glyph_index, [NativeTypeName("FT_UInt *")] uint* cid);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stream_OpenGzip", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stream_OpenGzip([NativeTypeName("FT_Stream")] FT_StreamRec_* stream, [NativeTypeName("FT_Stream")] FT_StreamRec_* source);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Gzip_Uncompress", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Gzip_Uncompress([NativeTypeName("FT_Memory")] FT_MemoryRec_* memory, [NativeTypeName("FT_Byte *")] byte* output, [NativeTypeName("FT_ULong *")] UIntPtr* output_len, [NativeTypeName("const FT_Byte *")] byte* input, [NativeTypeName("FT_ULong")] UIntPtr input_len);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stream_OpenLZW", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stream_OpenLZW([NativeTypeName("FT_Stream")] FT_StreamRec_* stream, [NativeTypeName("FT_Stream")] FT_StreamRec_* source);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stream_OpenBzip2", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stream_OpenBzip2([NativeTypeName("FT_Stream")] FT_StreamRec_* stream, [NativeTypeName("FT_Stream")] FT_StreamRec_* source);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_WinFNT_Header", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_WinFNT_Header([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_WinFNT_HeaderRec *")] FT_WinFNT_HeaderRec_* aheader);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Palette_Data_Get", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Palette_Data_Get([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Palette_Data *")] FT_Palette_Data_* apalette);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Palette_Select", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Palette_Select([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UShort")] ushort palette_index, [NativeTypeName("FT_Color **")] FT_Color_** apalette);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Palette_Set_Foreground_Color", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Palette_Set_Foreground_Color([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Color")] FT_Color_ foreground_color);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Color_Glyph_Layer", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Color_Glyph_Layer([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint base_glyph, [NativeTypeName("FT_UInt *")] uint* aglyph_index, [NativeTypeName("FT_UInt *")] uint* acolor_index, [NativeTypeName("FT_LayerIterator *")] FT_LayerIterator_* iterator);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Color_Glyph_Paint", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Color_Glyph_Paint([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint base_glyph, [NativeTypeName("FT_Color_Root_Transform")] FT_Color_Root_Transform_ root_transform, [NativeTypeName("FT_OpaquePaint *")] FT_Opaque_Paint_* paint);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Color_Glyph_ClipBox", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Color_Glyph_ClipBox([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint base_glyph, [NativeTypeName("FT_ClipBox *")] FT_ClipBox_* clip_box);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Paint_Layers", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Paint_Layers([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_LayerIterator *")] FT_LayerIterator_* iterator, [NativeTypeName("FT_OpaquePaint *")] FT_Opaque_Paint_* paint);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Colorline_Stops", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Colorline_Stops([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_ColorStop *")] FT_ColorStop_* color_stop, [NativeTypeName("FT_ColorStopIterator *")] FT_ColorStopIterator_* iterator);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Paint", ExactSpelling = true)]
    [return: NativeTypeName("FT_Bool")]
    public static extern byte Get_Paint([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_OpaquePaint")] FT_Opaque_Paint_ opaque_paint, [NativeTypeName("FT_COLR_Paint *")] FT_COLR_Paint_* paint);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Init", ExactSpelling = true)]
    public static extern void Bitmap_Init([NativeTypeName("FT_Bitmap *")] FT_Bitmap_* abitmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_New", ExactSpelling = true)]
    public static extern void Bitmap_New([NativeTypeName("FT_Bitmap *")] FT_Bitmap_* abitmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Copy", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Bitmap_Copy([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Bitmap *")] FT_Bitmap_* source, [NativeTypeName("FT_Bitmap *")] FT_Bitmap_* target);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Embolden", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Bitmap_Embolden([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Bitmap *")] FT_Bitmap_* bitmap, [NativeTypeName("FT_Pos")] IntPtr xStrength, [NativeTypeName("FT_Pos")] IntPtr yStrength);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Convert", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Bitmap_Convert([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Bitmap *")] FT_Bitmap_* source, [NativeTypeName("FT_Bitmap *")] FT_Bitmap_* target, [NativeTypeName("FT_Int")] int alignment);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Blend", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Bitmap_Blend([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("const FT_Bitmap *")] FT_Bitmap_* source, [NativeTypeName("const FT_Vector")] FT_Vector_ source_offset, [NativeTypeName("FT_Bitmap *")] FT_Bitmap_* target, [NativeTypeName("FT_Vector *")] FT_Vector_* atarget_offset, [NativeTypeName("FT_Color")] FT_Color_ color);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_GlyphSlot_Own_Bitmap", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int GlyphSlot_Own_Bitmap([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Bitmap_Done", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Bitmap_Done([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Bitmap *")] FT_Bitmap_* bitmap);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_Get_BBox", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Outline_Get_BBox([NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_BBox *")] FT_BBox_* abbox);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_Manager_New([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_UInt")] uint max_faces, [NativeTypeName("FT_UInt")] uint max_sizes, [NativeTypeName("FT_ULong")] UIntPtr max_bytes, [NativeTypeName("FTC_Face_Requester")] IntPtr requester, [NativeTypeName("FT_Pointer")] void* req_data, [NativeTypeName("FTC_Manager *")] FTC_ManagerRec_** amanager);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void FTC_Manager_Reset([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void FTC_Manager_Done([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_Manager_LookupFace([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_FaceID")] void* face_id, [NativeTypeName("FT_Face *")] FT_FaceRec_** aface);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_Manager_LookupSize([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_Scaler")] FTC_ScalerRec_* scaler, [NativeTypeName("FT_Size *")] FT_SizeRec_** asize);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void FTC_Node_Unref([NativeTypeName("FTC_Node")] FTC_NodeRec_* node, [NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void FTC_Manager_RemoveFaceID([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_FaceID")] void* face_id);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_CMapCache_New([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_CMapCache *")] FTC_CMapCacheRec_** acache);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt")]
    public static extern uint FTC_CMapCache_Lookup([NativeTypeName("FTC_CMapCache")] FTC_CMapCacheRec_* cache, [NativeTypeName("FTC_FaceID")] void* face_id, [NativeTypeName("FT_Int")] int cmap_index, [NativeTypeName("FT_UInt32")] uint char_code);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_ImageCache_New([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_ImageCache *")] FTC_ImageCacheRec_** acache);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_ImageCache_Lookup([NativeTypeName("FTC_ImageCache")] FTC_ImageCacheRec_* cache, [NativeTypeName("FTC_ImageType")] FTC_ImageTypeRec_* type, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FT_Glyph *")] FT_GlyphRec_** aglyph, [NativeTypeName("FTC_Node *")] FTC_NodeRec_** anode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_ImageCache_LookupScaler([NativeTypeName("FTC_ImageCache")] FTC_ImageCacheRec_* cache, [NativeTypeName("FTC_Scaler")] FTC_ScalerRec_* scaler, [NativeTypeName("FT_ULong")] UIntPtr load_flags, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FT_Glyph *")] FT_GlyphRec_** aglyph, [NativeTypeName("FTC_Node *")] FTC_NodeRec_** anode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_SBitCache_New([NativeTypeName("FTC_Manager")] FTC_ManagerRec_* manager, [NativeTypeName("FTC_SBitCache *")] FTC_SBitCacheRec_** acache);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_SBitCache_Lookup([NativeTypeName("FTC_SBitCache")] FTC_SBitCacheRec_* cache, [NativeTypeName("FTC_ImageType")] FTC_ImageTypeRec_* type, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FTC_SBit *")] FTC_SBitRec_** sbit, [NativeTypeName("FTC_Node *")] FTC_NodeRec_** anode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int FTC_SBitCache_LookupScaler([NativeTypeName("FTC_SBitCache")] FTC_SBitCacheRec_* cache, [NativeTypeName("FTC_Scaler")] FTC_ScalerRec_* scaler, [NativeTypeName("FT_ULong")] UIntPtr load_flags, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FTC_SBit *")] FTC_SBitRec_** sbit, [NativeTypeName("FTC_Node *")] FTC_NodeRec_** anode);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Multi_Master", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Multi_Master([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Multi_Master *")] FT_Multi_Master_* amaster);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_MM_Var", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_MM_Var([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_MM_Var **")] FT_MM_Var_** amaster);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_MM_Var", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Done_MM_Var([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_MM_Var *")] FT_MM_Var_* amaster);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_MM_Design_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_MM_Design_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Long *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Var_Design_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Var_Design_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Var_Design_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Var_Design_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_MM_Blend_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_MM_Blend_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_MM_Blend_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_MM_Blend_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Var_Blend_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Var_Blend_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Var_Blend_Coordinates", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Var_Blend_Coordinates([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint num_coords, [NativeTypeName("FT_Fixed *")] IntPtr* coords);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_MM_WeightVector", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_MM_WeightVector([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint len, [NativeTypeName("FT_Fixed *")] IntPtr* weightvector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_MM_WeightVector", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_MM_WeightVector([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt *")] uint* len, [NativeTypeName("FT_Fixed *")] IntPtr* weightvector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Var_Axis_Flags", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Var_Axis_Flags([NativeTypeName("FT_MM_Var *")] FT_MM_Var_* master, [NativeTypeName("FT_UInt")] uint axis_index, [NativeTypeName("FT_UInt *")] uint* flags);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Named_Instance", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Set_Named_Instance([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint instance_index);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Default_Named_Instance", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Default_Named_Instance([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt *")] uint* instance_index);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Sfnt_Name_Count", ExactSpelling = true)]
    [return: NativeTypeName("FT_UInt")]
    public static extern uint Get_Sfnt_Name_Count([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Sfnt_Name", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Sfnt_Name([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint idx, [NativeTypeName("FT_SfntName *")] FT_SfntName_* aname);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Sfnt_LangTag", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Sfnt_LangTag([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint langID, [NativeTypeName("FT_SfntLangTag *")] FT_SfntLangTag_* alangTag);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_OpenType_Validate", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int OpenType_Validate([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint validation_flags, [NativeTypeName("FT_Bytes *")] byte** BASE_table, [NativeTypeName("FT_Bytes *")] byte** GDEF_table, [NativeTypeName("FT_Bytes *")] byte** GPOS_table, [NativeTypeName("FT_Bytes *")] byte** GSUB_table, [NativeTypeName("FT_Bytes *")] byte** JSTF_table);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_OpenType_Free", ExactSpelling = true)]
    public static extern void OpenType_Free([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Bytes")] byte* table);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_TrueTypeGX_Validate", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int TrueTypeGX_Validate([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint validation_flags, [NativeTypeName("FT_Bytes[10]")] byte** tables, [NativeTypeName("FT_UInt")] uint table_length);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_TrueTypeGX_Free", ExactSpelling = true)]
    public static extern void TrueTypeGX_Free([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Bytes")] byte* table);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_ClassicKern_Validate", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int ClassicKern_Validate([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint validation_flags, [NativeTypeName("FT_Bytes *")] byte** ckern_table);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_ClassicKern_Free", ExactSpelling = true)]
    public static extern void ClassicKern_Free([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_Bytes")] byte* table);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PFR_Metrics", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_PFR_Metrics([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt *")] uint* aoutline_resolution, [NativeTypeName("FT_UInt *")] uint* ametrics_resolution, [NativeTypeName("FT_Fixed *")] IntPtr* ametrics_x_scale, [NativeTypeName("FT_Fixed *")] IntPtr* ametrics_y_scale);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PFR_Kerning", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_PFR_Kerning([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint left, [NativeTypeName("FT_UInt")] uint right, [NativeTypeName("FT_Vector *")] FT_Vector_* avector);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_PFR_Advance", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_PFR_Advance([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FT_Pos *")] IntPtr* aadvance);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_GetInsideBorder", ExactSpelling = true)]
    [return: NativeTypeName("FT_StrokerBorder")]
    public static extern FT_StrokerBorder_ Outline_GetInsideBorder([NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Outline_GetOutsideBorder", ExactSpelling = true)]
    [return: NativeTypeName("FT_StrokerBorder")]
    public static extern FT_StrokerBorder_ Outline_GetOutsideBorder([NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_New", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_New([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Stroker *")] FT_StrokerRec_** astroker);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_Set", ExactSpelling = true)]
    public static extern void Stroker_Set([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Fixed")] IntPtr radius, [NativeTypeName("FT_Stroker_LineCap")] FT_Stroker_LineCap_ line_cap, [NativeTypeName("FT_Stroker_LineJoin")] FT_Stroker_LineJoin_ line_join, [NativeTypeName("FT_Fixed")] IntPtr miter_limit);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_Rewind", ExactSpelling = true)]
    public static extern void Stroker_Rewind([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_ParseOutline", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_ParseOutline([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Outline *")] FT_Outline_* outline, [NativeTypeName("FT_Bool")] byte opened);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_BeginSubPath", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_BeginSubPath([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Vector *")] FT_Vector_* to, [NativeTypeName("FT_Bool")] byte open);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_EndSubPath", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_EndSubPath([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_LineTo", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_LineTo([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Vector *")] FT_Vector_* to);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_ConicTo", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_ConicTo([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Vector *")] FT_Vector_* control, [NativeTypeName("FT_Vector *")] FT_Vector_* to);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_CubicTo", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_CubicTo([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Vector *")] FT_Vector_* control1, [NativeTypeName("FT_Vector *")] FT_Vector_* control2, [NativeTypeName("FT_Vector *")] FT_Vector_* to);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_GetBorderCounts", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_GetBorderCounts([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_StrokerBorder")] FT_StrokerBorder_ border, [NativeTypeName("FT_UInt *")] uint* anum_points, [NativeTypeName("FT_UInt *")] uint* anum_contours);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_ExportBorder", ExactSpelling = true)]
    public static extern void Stroker_ExportBorder([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_StrokerBorder")] FT_StrokerBorder_ border, [NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_GetCounts", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Stroker_GetCounts([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_UInt *")] uint* anum_points, [NativeTypeName("FT_UInt *")] uint* anum_contours);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_Export", ExactSpelling = true)]
    public static extern void Stroker_Export([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Outline *")] FT_Outline_* outline);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Stroker_Done", ExactSpelling = true)]
    public static extern void Stroker_Done([NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_Stroke", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Glyph_Stroke([NativeTypeName("FT_Glyph *")] FT_GlyphRec_** pglyph, [NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Bool")] byte destroy);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Glyph_StrokeBorder", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Glyph_StrokeBorder([NativeTypeName("FT_Glyph *")] FT_GlyphRec_** pglyph, [NativeTypeName("FT_Stroker")] FT_StrokerRec_* stroker, [NativeTypeName("FT_Bool")] byte inside, [NativeTypeName("FT_Bool")] byte destroy);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_GlyphSlot_Embolden", ExactSpelling = true)]
    public static extern void GlyphSlot_Embolden([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_GlyphSlot_AdjustWeight", ExactSpelling = true)]
    public static extern void GlyphSlot_AdjustWeight([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Fixed")] IntPtr xdelta, [NativeTypeName("FT_Fixed")] IntPtr ydelta);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_GlyphSlot_Oblique", ExactSpelling = true)]
    public static extern void GlyphSlot_Oblique([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_GlyphSlot_Slant", ExactSpelling = true)]
    public static extern void GlyphSlot_Slant([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Fixed")] IntPtr xslant, [NativeTypeName("FT_Fixed")] IntPtr yslant);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Font_Format", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* Get_Font_Format([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_X11_Font_Format", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* Get_X11_Font_Format([NativeTypeName("FT_Face")] FT_FaceRec_* face);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Sin", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr Sin([NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Cos", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr Cos([NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Tan", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr Tan([NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Atan2", ExactSpelling = true)]
    [return: NativeTypeName("FT_Angle")]
    public static extern IntPtr Atan2([NativeTypeName("FT_Fixed")] IntPtr x, [NativeTypeName("FT_Fixed")] IntPtr y);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Angle_Diff", ExactSpelling = true)]
    [return: NativeTypeName("FT_Angle")]
    public static extern IntPtr Angle_Diff([NativeTypeName("FT_Angle")] IntPtr angle1, [NativeTypeName("FT_Angle")] IntPtr angle2);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_Unit", ExactSpelling = true)]
    public static extern void Vector_Unit([NativeTypeName("FT_Vector *")] FT_Vector_* vec, [NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_Rotate", ExactSpelling = true)]
    public static extern void Vector_Rotate([NativeTypeName("FT_Vector *")] FT_Vector_* vec, [NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_Length", ExactSpelling = true)]
    [return: NativeTypeName("FT_Fixed")]
    public static extern IntPtr Vector_Length([NativeTypeName("FT_Vector *")] FT_Vector_* vec);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_Polarize", ExactSpelling = true)]
    public static extern void Vector_Polarize([NativeTypeName("FT_Vector *")] FT_Vector_* vec, [NativeTypeName("FT_Fixed *")] IntPtr* length, [NativeTypeName("FT_Angle *")] IntPtr* angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Vector_From_Polar", ExactSpelling = true)]
    public static extern void Vector_From_Polar([NativeTypeName("FT_Vector *")] FT_Vector_* vec, [NativeTypeName("FT_Fixed")] IntPtr length, [NativeTypeName("FT_Angle")] IntPtr angle);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Library_SetLcdFilter", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Library_SetLcdFilter([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_LcdFilter")] FT_LcdFilter_ filter);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Library_SetLcdFilterWeights", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Library_SetLcdFilterWeights([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("unsigned char *")] byte* weights);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Library_SetLcdGeometry", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Library_SetLcdGeometry([NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Vector[3]")] FT_Vector_* sub);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Gasp", ExactSpelling = true)]
    [return: NativeTypeName("FT_Int")]
    public static extern int Get_Gasp([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint ppem);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Advance", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Advance([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint gindex, [NativeTypeName("FT_Int32")] int load_flags, [NativeTypeName("FT_Fixed *")] IntPtr* padvance);

    [DllImport("freetype6", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Advances", ExactSpelling = true)]
    [return: NativeTypeName("FT_Error")]
    public static extern int Get_Advances([NativeTypeName("FT_Face")] FT_FaceRec_* face, [NativeTypeName("FT_UInt")] uint start, [NativeTypeName("FT_UInt")] uint count, [NativeTypeName("FT_Int32")] int load_flags, [NativeTypeName("FT_Fixed *")] IntPtr* padvances);
}
