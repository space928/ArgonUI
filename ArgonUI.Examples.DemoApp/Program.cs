using ArgonUI;
using ArgonUI.Backends.OpenGL;

namespace ArgonUI.Examples.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to ArgonUI!");
        
        ArgonManager argon = new();
        argon.CreateOpenGLWindow();

        Console.ReadLine();
    }
}
