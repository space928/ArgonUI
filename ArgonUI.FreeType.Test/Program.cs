namespace ArgonUI.FreeType.Test;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Loading FreeType...");
        using var ft = new FreeTypeLibrary();
        var ver = ft.GetVersion();
        Console.WriteLine($"Version: {ver.major}.{ver.minor}.{ver.patch}");

        try
        {
            var fnt = File.ReadAllBytes(@"..\..\..\..\ArgonUI\Fonts\NotoSans-SemiBold.ttf");
            using var face = ft.LoadFace(fnt);
            //var face = ft.LoadFace(@"..\..\..\..\ArgonUI\Fonts\NotoSans-SemiBold.ttf");

            var buff = new byte[1024 * 16];
            face.SetCharSize(14 * 64);
            var size = face.RenderGlyph('P', buff);

            Console.WriteLine($"Rendered glyph size: ({size.width}, {size.height})");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.ReadKey();
    }
}
