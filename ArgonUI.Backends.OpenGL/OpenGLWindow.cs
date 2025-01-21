using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArgonUI.Backends.OpenGL;

public class OpenGLWindow : IUIWindow, IDisposable
{
    private readonly IWindow nativeWnd;
    private GL? gl;
    private IInputContext? inputContext;
    private IKeyboard? mainKeyboard;
    private IMouse? mainMouse;

    public string Title { get => nativeWnd.Title; set => nativeWnd.Title = value; }

    public IDrawContext DrawContext => throw new NotImplementedException();

    public VectorInt2 Size { get => nativeWnd.Size.ToVectorInt2(); set => nativeWnd.Size = value.ToVector2D(); }
    public VectorInt2 Position { get => nativeWnd.Position.ToVectorInt2(); set => nativeWnd.Position = value.ToVector2D(); }

    public event Action? OnLoaded;
    public event Action? OnClosing;
    public event Action? OnResize;
    public event Action<float>? OnRender;
    public event Action<IEnumerable<string>>? OnFileDrop;

    public OpenGLWindow(string title = "ArgonUI Window")
    {
        var wndOptions = new WindowOptions(ViewOptions.Default);
        wndOptions.Samples = 4;
        wndOptions.Title = title;
        wndOptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new(3, 3));
        nativeWnd = Window.Create(wndOptions);
        //nativeWnd.IsEventDriven = true;
        nativeWnd.Load += OnLoaded;
        nativeWnd.FramebufferResize += _ => OnResize?.Invoke();
        nativeWnd.FileDrop += OnFileDrop;
        nativeWnd.Closing += OnClosing;
        nativeWnd.Render += delta => OnRender?.Invoke((float)delta);

        nativeWnd.Load += () =>
        {
            gl = nativeWnd.CreateOpenGL();

#if DEBUG
            gl.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DebugSeverityLow, null, true);
            nint tmp = 0;
            gl.DebugMessageCallback<nint>((GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam) =>
            {
                if ((uint)severity == (uint)DebugSeverity.DebugSeverityNotification)
                    return;
                string msg = Marshal.PtrToStringUTF8(message, length);
                Debug.WriteLine($"[GL_Debug] [{(DebugSeverity)severity}] [{(DebugType)type}] {msg}");
            }, ref tmp);
            gl.Enable(EnableCap.DebugOutput);
#endif

            inputContext = nativeWnd.CreateInput();
            mainKeyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            mainMouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;
        };

        nativeWnd.Run();
    }

    public void Dispose()
    {
        gl?.Dispose();
        inputContext?.Dispose();
        nativeWnd.Dispose();
    }

    public void Show()
    {
        nativeWnd.Focus();
    }

    public void Minimize()
    {
        nativeWnd.WindowState = WindowState.Minimized;
    }

    public void Maximize()
    {
        nativeWnd.WindowState = WindowState.Maximized;
    }

    public void Close()
    {
        nativeWnd.Close();
    }
}
