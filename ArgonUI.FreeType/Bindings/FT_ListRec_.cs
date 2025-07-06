namespace FreeType.Bindings;

public unsafe partial struct FT_ListRec_
{
    [NativeTypeName("FT_ListNode")]
    public FT_ListNodeRec_* head;

    [NativeTypeName("FT_ListNode")]
    public FT_ListNodeRec_* tail;
}
