using ArgonUI.Input;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using MouseButton = ArgonUI.Input.MouseButton;

namespace ArgonUI.Backends.OpenGL;

public class OpenGLWindow : UIWindow
{
    private IWindow? nativeWnd;
    private OpenGLDrawContext? drawContext;
    private readonly Thread uiThread;
    private GL? gl;
    private IInputContext? inputContext;
    private IKeyboard? mainKeyboard;
    private IMouse? mainMouse;
    private Vector2 lastScrollPos;

    public override string Title 
    { 
        get => nativeWnd?.Title ?? string.Empty; 
        set 
        { 
            if (nativeWnd != null) 
                nativeWnd.Title = value; 
        } 
    }

    public override IDrawContext DrawContext => drawContext!;

    public override VectorInt2 Size
    {
        get 
        {
            if (nativeWnd == null)
                return VectorInt2.Zero;
            return nativeWnd.Size.ToVectorInt2(); 
        }
        set
        {
            if (nativeWnd != null)
                nativeWnd.Size = value.ToVector2D();
        }
    }
    public override VectorInt2 Position
    {
        get 
        {
            if (nativeWnd == null)
                return VectorInt2.Zero;
            return nativeWnd.Position.ToVectorInt2();
        }
        set
        {
            if (nativeWnd != null)
                nativeWnd.Position = value.ToVector2D();
        }
    }

    public override Thread UIThread => uiThread;

    public override event Action? OnLoaded;
    public override event Action? OnClosing;
    public override event Action? OnResize;
    public override event Action<float>? OnRender;
    public override event Action<IEnumerable<string>>? OnFileDrop;

    public OpenGLWindow(ArgonManager argon, string title = "ArgonUI Window") : base(argon)
    {
        uiThread = new(() =>
        {
            CreateWindow(title);
            //nativeWnd?.MakeCurrent();
            nativeWnd?.Run();
        });
        uiThread.Start();
    }

    private void CreateWindow(string title)
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

            drawContext = new(gl);

            MapInput();
        };
    }

    public override void Dispose()
    {
        base.Dispose();
        gl?.Dispose();
        inputContext?.Dispose();
        nativeWnd?.Dispose();
    }

    public override void Show()
    {
        nativeWnd?.Focus();
    }

    public override void Minimize()
    {
        if (nativeWnd != null)
            nativeWnd.WindowState = WindowState.Minimized;
    }

    public override void Maximize()
    {
        if (nativeWnd != null)
            nativeWnd.WindowState = WindowState.Maximized;
    }

    public override void Close()
    {
        nativeWnd?.Close();
    }

    protected override void RenderFrame()
    {
        nativeWnd?.DoRender();
    }

    protected override void SetMousePos(VectorInt2 mousePos)
    {
        if (mainMouse == null)
            return;

        mainMouse.Position = new(mousePos.x, mousePos.y);
    }

    private void MapInput()
    {
        if (mainKeyboard == null || mainMouse == null)
            return;

        mainKeyboard.KeyDown += (keyboard, key, ind) => OnKeyDown(this, (KeyCode)key);
        mainKeyboard.KeyUp += (keyboard, key, ind) => OnKeyUp(this, (KeyCode)key);
        mainMouse.MouseDown += (mouse, button) => OnMouseDown(this, (MouseButton)button);
        mainMouse.MouseUp += (mouse, button) => OnMouseUp(this, (MouseButton)button);
        mainMouse.MouseMove += (mouse, pos) => OnMouseMove(this, new((int)pos.X, (int)pos.Y));
        mainMouse.Scroll += (mouse, pos) =>
        {
            var posVec = new Vector2(pos.X, pos.Y);
            var delta = posVec - lastScrollPos;
            OnMouseWheel(this, delta);
            lastScrollPos = posVec;
        };
    }
}
