using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Backends.Headless;

/// <summary>
/// 
/// </summary>
public static partial class ExtensionMethods
{
    /// <summary>
    /// Creates a new <see cref="UIWindow"/> using the headless backend.
    /// <para/>
    /// Note that this backend does not perform any rendering and does not create a window.
    /// </summary>
    /// <param name="argon"></param>
    /// <returns></returns>
    public static UIWindow CreateHeadlessWindow(this ArgonManager argon)
    {
        var wnd = new HeadlessWindow(argon);
        return wnd;
    }
}
