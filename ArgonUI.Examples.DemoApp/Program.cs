using ArgonUI;
using ArgonUI.UIElements;
using ArgonUI.Backends.OpenGL;

namespace ArgonUI.Examples.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        ArgonManager argon = new();
        var wnd = argon.CreateOpenGLWindow();
        var rect = new Rectangle();
        wnd.RootElement.AddChild(rect);

        wnd.RootElement.BGColour = new(0, 0.5f, 1, 1);
        rect.Width = 100;
        rect.Height = 100;
        rect.Rounding = 5;
        rect.Colour = new(0, .8f, .25f, 1);
        rect.OnMouseDown += () =>
        {
            //rect.Dirty(DirtyFlags.Content);
            //Console.WriteLine("Rectangle clicked!");
        };
        rect.OnMouseEnter += () =>
        {
            rect.Colour = new(1f, .5f, .1f, 1);
            rect.Rounding = 20;
        };
        rect.OnMouseLeave += () =>
        {
            rect.Colour = new(0, .8f, .25f, 1);
            rect.Rounding = 5;
        };

        wnd.UIThread.Join();
    }
}
