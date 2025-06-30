using System;

namespace ArgonUI.UIElements;

[Flags]
public enum DirtyFlags
{
    None,
    Layout = 1 << 0,
    Content = 1 << 1,
    ChildLayout = 1 << 2,
    ChildContent = 1 << 3,

    ContentAndLayout = Layout | Content,
    AllChild = ChildContent | ChildLayout,
    All = ContentAndLayout | AllChild,
}
