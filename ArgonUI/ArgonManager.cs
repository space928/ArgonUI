using ArgonUI.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

    /// <summary>
    /// Loads a file by name eithr from disk or embedded in the assembly.
    /// <para/>
    /// Starts by checking if the given path exists using <see cref="File.Exists(string?)"/>;
    /// if this succeeds it attempts to open that file. Otherwise, it searches for the file
    /// in the manifest resource of the given assembly and returns that if found.
    /// </summary>
    /// <param name="path">The path of the file to look for.</param>
    /// <param name="assembly">The assembly to search for the file in, defaults to the ArgonUI assembly.</param>
    /// <returns>A read only stream of the specified file.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static Stream LoadResourceFile(string? path, Assembly? assembly = null)
    {
        if (path == null)
            throw new FileNotFoundException(path);
        if (File.Exists(path))
            return File.OpenRead(path);

        assembly ??= typeof(ArgonManager).Assembly; //Assembly.GetCallingAssembly();
        string? resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(str => str.EndsWith(Path.GetFileName(path)));

        if (resourceName == null || assembly == null)
            throw new FileNotFoundException(path);

        return assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException(path);
    }
}
