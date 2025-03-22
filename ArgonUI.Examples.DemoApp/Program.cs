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
        var label = new Label();
        wnd.RootElement.AddChild(rect);

        int counter = 0;

        wnd.RootElement.BGColour = new(0, 0.5f, 1, 1);
        rect.Width = 100;
        rect.Height = 100;
        rect.Rounding = 5;
        rect.Colour = new(0, .8f, .25f, 1);
        rect.OnMouseDown += () =>
        {
#if DEBUG_LATENCY
            ((OpenGLWindow)wnd).LogLatency(DateTime.UtcNow.Ticks, "rect OnMouseDown");
            rect.logLatencyNow = true;
            rect.Colour = (counter & 1) == 0 ? new(1, 0, 0, 1) : new(0, 0, 1, 1);
#endif
            //rect.Dirty(DirtyFlags.Content);
            //Console.WriteLine("Rectangle clicked!");
            label.Text = $"Hello World! {counter++}";
        };
        rect.OnMouseEnter += () =>
        {
#if DEBUG_LATENCY
            ((OpenGLWindow)wnd).LogLatency(DateTime.UtcNow.Ticks, "rect OnMouseEnter");
            rect.logLatencyNow = true;
#endif

            rect.Colour = new(1f, .5f, .1f, 1);
            rect.Rounding = 20;
        };
        rect.OnMouseLeave += () =>
        {
            rect.Colour = new(0, .8f, .25f, 1);
            rect.Rounding = 5;
        };

        wnd.RootElement.AddChild(label);
        label.Text = "Hello World!";
        label.FontSize = 40;

        wnd.Wait();
    }
}
