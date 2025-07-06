using ArgonUI;
using ArgonUI.UIElements;
using ArgonUI.Backends.OpenGL;
using ArgonUI.Styling;
using ArgonUI.Styling.Selectors;
using System.Numerics;

namespace ArgonUI.Examples.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        ArgonManager argon = new();
        var wnd = argon.CreateOpenGLWindow();

        wnd.RootElement.Style = new([
            new Style([
                ArgonUIStyles.Rounding(5),
                //ArgonUIStyles.FontSize(30),
                ArgonUIStyles.TextColour(Vector4.One)
            ])
        ]);

        StyleSet buttonStyle = new([
            new Style([
                ArgonUIStyles.Rounding(5),
                ArgonUIStyles.Colour(new(0.35f, 0.35f, 0.35f, 1)),
            ]),
            new Style(new HoveredSelector(), [
                ArgonUIStyles.Rounding(10),
                ArgonUIStyles.Colour(new(0.45f, 0.45f, 0.45f, 1)),
            ])
        ]);

        wnd.RootElement.BGColour = new(0.2f, 0.23f, 0.25f, 1);

        var stackPanel = new StackPanel();
        stackPanel.InnerPadding = new(2);
        stackPanel.HorizontalAlignment = Alignment.Stretch;
        wnd.RootElement.AddChild(stackPanel);

        var btn = new Button();
        btn.Width = 0;
        btn.Height = 100;
        btn.Rounding = 0;
        btn.HorizontalAlignment = Alignment.Stretch;
        btn.Style = buttonStyle;
        stackPanel.AddChild(btn);

        var label = (Label)btn.Content!;
        label.Text = "Hello World!";
        label.FontSize = 40;

        int counter = 0;

        btn.OnMouseDown += args =>
        {
#if DEBUG_LATENCY
            ((OpenGLWindow)wnd).LogLatency(DateTime.UtcNow.Ticks, "rect OnMouseDown");
            rect.logLatencyNow = true;
            rect.Colour = (counter & 1) == 0 ? new(1, 0, 0, 1) : new(0, 0, 1, 1);
#endif
            //rect.Dirty(DirtyFlag.Content);
            //Console.WriteLine("Rectangle clicked!");
            label.Text = $"Hello World! {counter++}";
            btn.Colour = new(0.30f, 0.30f, 0.30f, 1);
            args.Handled = true;
        };
        btn.OnMouseUp += args =>
        {
            btn.Colour = ((StylableProp<Vector4>)buttonStyle[1]["Colour"]).Value;
            args.Handled = true;
        };
        btn.OnMouseEnter += args =>
        {
#if DEBUG_LATENCY
            ((OpenGLWindow)wnd).LogLatency(DateTime.UtcNow.Ticks, "rect OnMouseEnter");
            rect.logLatencyNow = true;
#endif
        };

        for (int j = 0; j < 50; j++)
        {
            StackPanel newStackPanel = new(Direction.Horizontal);
            newStackPanel.InnerPadding = new(2);
            stackPanel.AddChild(newStackPanel);
            for (int i = 0; i < 50; i++)
            {
                Button newButton = new();
                newButton.Width = 20;
                newButton.Height = 20;
                newButton.HorizontalAlignment = Alignment.Centre;
                newButton.VerticalAlignment = Alignment.Centre;
                ((Label)newButton.Content!).FontSize = 8;
                newButton.Style = buttonStyle;
                newStackPanel.AddChild(newButton);

                newButton.OnMouseEnter += args =>
                {
                    newButton.Width = 24;
                    newButton.Height = 24;
                };
                newButton.OnMouseLeave += args =>
                {
                    newButton.Width = 20;
                    newButton.Height = 20;
                };
            }
        }


        wnd.Wait();
    }
}
