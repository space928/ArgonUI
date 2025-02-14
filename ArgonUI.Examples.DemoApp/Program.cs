using ArgonUI;
using ArgonUI.UIElements;
using ArgonUI.Backends.OpenGL;

namespace ArgonUI.Examples.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to ArgonUI!");
        
        ArgonManager argon = new();
        var wnd = argon.CreateOpenGLWindow();
        var rect = new Rectangle();
        wnd.RootElement.AddChild(rect);

        rect.Width = 100;
        rect.Height = 100;
        rect.OnMouseDown += () =>
        {
            Console.WriteLine("Rectangle clicked!");
        };

        wnd.UIThread.Join();
    }
}
