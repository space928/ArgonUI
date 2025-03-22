using System;

namespace ArgonUI.UIElements;

[Flags]
public enum DirtyFlags
{
    None,
    Layout = 1 << 0,
    Content = 1 << 1,
    ChildContent = 1 << 2,
    ChildLayout = 1 << 3,

    //ContentAndLayout = Layout | Content
}
