using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.Drawing;
using SixLabors.Fonts;

namespace ArgonUI.Typography;

public static class TTFFontManager
{
    private static readonly object fontCollectionLock;
    private static readonly FontCollection fontCollection;
    private static readonly ConcurrentDictionary<string, TTFFontFamily> loadedFonts;

    static TTFFontManager()
    {
        fontCollectionLock = new();
        fontCollection = new();
        loadedFonts = [];

        Fonts.DefaultFontLoader = x => LoadFont(x).GetScaledFont(16);
    }

    public static TTFFontFamily LoadFont(string fileName, Assembly? containingAssembly = null)
    {
        using var stream = ArgonManager.LoadResourceFile(fileName, containingAssembly);
        return LoadFont(stream);
    }

    public static TTFFontFamily LoadFont(Stream fileStream, bool leaveOpen = true)
    {
        FontFamily family;
        lock (fontCollectionLock)
        {
            family = fontCollection.Add(fileStream, out var desc);
        }
        return new TTFFontFamily(family);
    }
}
