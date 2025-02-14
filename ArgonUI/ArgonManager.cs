using ArgonUI.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

public class ArgonManager
{
    private readonly List<UIWindow> windows = [];

    public ReadOnlyCollection<UIWindow> Windows { get; init; }
    public InputManager InputManager { get; init; }

    public ArgonManager()
    {
        Windows = new(windows);
        InputManager = new(this);
    }

    internal void CreateWindow(UIWindow window)
    {
        windows.Add(window);
    }

    internal void DestroyWindow(UIWindow window)
    {
        windows.Remove(window);
        window.Dispose();
    }
}
