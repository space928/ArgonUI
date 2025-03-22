using ArgonUI.Drawing;
using ArgonUI.Input;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MouseButton = ArgonUI.Input.MouseButton;

namespace ArgonUI.Backends.OpenGL;

public class OpenGLWindow : UIWindow
{
    private IWindow? nativeWnd;
    private OpenGLDrawContext? drawContext;
    private readonly Thread uiThread;
    private readonly Thread eventThread;
    private GL? gl;
    private IInputContext? inputContext;
    private IKeyboard? mainKeyboard;
    private IMouse? mainMouse;
    private Vector2 lastScrollPos;
    private readonly CancellationTokenSource isClosingTokenSource;
    private readonly CancellationToken isClosingToken;
    private readonly BlockingCollection<InputEvent> inputEvents;
    private volatile int redrawRequested;

    public override string Title
    {
        get => nativeWnd?.Title ?? string.Empty;
        set
        {
            if (nativeWnd != null)
                nativeWnd.Title = value;
        }
    }

    public override IDrawContext? DrawContext => drawContext;

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
        isClosingTokenSource = new();
        isClosingToken = isClosingTokenSource.Token;
        inputEvents = new();
        uiThread = new(() => RunUIThread(title));
        uiThread.Priority = ThreadPriority.AboveNormal;
        uiThread.Name = "ArgonUIThread";

        eventThread = new(RunEventThread);
        eventThread.Priority = ThreadPriority.Normal;
        eventThread.Name = "ArgonUIEventThread";

        uiThread.Start();
    }

    private void CreateWindow(string title)
    {
        var wndOptions = new WindowOptions(ViewOptions.Default);
        wndOptions.Samples = 4;
        wndOptions.Title = title;
        wndOptions.IsEventDriven = true;
        wndOptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new(3, 3));
        nativeWnd = Window.Create(wndOptions);
        nativeWnd.IsEventDriven = true;
        nativeWnd.Load += OnLoaded;
        nativeWnd.FramebufferResize += _ => OnResize?.Invoke();
        nativeWnd.FileDrop += OnFileDrop;
        nativeWnd.Closing += OnClosing;
        nativeWnd.Render += delta => OnRender?.Invoke((float)delta);
        nativeWnd.Closing += () => isClosingTokenSource.Cancel();

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
#if NETSTANDARD
                var msg = string.Empty;
                unsafe
                {
                    if (message != 0 && length > 0)
                        msg = Encoding.UTF8.GetString((byte*)message, length);
                }
#else
                string msg = Marshal.PtrToStringUTF8(message, length);
#endif
                Debug.WriteLine($"[GL_Debug] [{(DebugSeverity)severity}] [{(DebugType)type}] {msg}");

                //if ((uint)severity == (uint)DebugSeverity.DebugSeverityHigh)
                //    Debugger.Break();
            }, ref tmp);
            gl.Enable(EnableCap.DebugOutput);
#endif

            inputContext = nativeWnd.CreateInput();
            mainKeyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            mainMouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;

            drawContext = new(gl);
            drawContext.InitRenderer(this);

            MapInput();
            eventThread.Start();
        };
    }

    private void RunUIThread(string title)
    {
        CreateWindow(title);
        //nativeWnd?.Run();
        nativeWnd?.Initialize();
        nativeWnd?.DoEvents();
        while ((!nativeWnd?.IsClosing) ?? true)
        {
#if DEBUG_LATENCY
            LogLatency(DateTime.UtcNow.Ticks, "UI frame start");
            if (Interlocked.CompareExchange(ref redrawRequested, 0, 1) == 1)
                nativeWnd?.DoRender();
            LogLatency(DateTime.UtcNow.Ticks, "UI events start");
            nativeWnd?.DoEvents();
            LogLatency(DateTime.UtcNow.Ticks, "UI end");
#else
            // Don't redraw the frame unless the UI engine requests it
            // This is very important in reducing rendering latency,
            // since otherwise, we need to wait for the previous frames
            // to finish drawing before a frame with updated data can be
            // drawn.
            if (Interlocked.CompareExchange(ref redrawRequested, 0, 1) == 1)
                nativeWnd?.DoRender();
            // Wait for events, this can be cancelled if a redraw is requested
            nativeWnd?.DoEvents();
#endif
        }
    }

    private void RunEventThread()
    {
        try
        {
            while ((!nativeWnd?.IsClosing) ?? true)
            {
                var evnt = inputEvents.Take(isClosingToken);
                switch (evnt.type)
                {
                    case InputEventType.KeyDown:
                        OnKeyDown(this, evnt.key);
                        break;
                    case InputEventType.KeyUp:
                        OnKeyUp(this, evnt.key);
                        break;
                    case InputEventType.MouseDown:
#if DEBUG_LATENCY
                        dbg_latencyStartTime = DateTime.UtcNow.Ticks;
#endif
                        OnMouseDown(this, evnt.button);
                        break;
                    case InputEventType.MouseUp:
                        OnMouseUp(this, evnt.button);
                        break;
                    case InputEventType.MouseMove:
#if DEBUG_LATENCY
                        var nt = DateTime.UtcNow.Ticks;
                        if (nt - dbg_latencyStartTime > 10000000)
                            dbg_latencyStartTime = nt;
#endif
                        OnMouseMove(this, new((int)evnt.pos.X, (int)evnt.pos.Y));
                        break;
                    case InputEventType.MouseScroll:
                        var delta = evnt.pos - lastScrollPos;
                        OnMouseWheel(this, delta);
                        lastScrollPos = evnt.pos;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        } catch (OperationCanceledException) { }
    }

#if DEBUG_LATENCY
    private long dbg_latencyStartTime = 0;
    private long dbg_latencyLast = 0;

    public void LogLatency(long endTime, string endState)
    {
        var diff = endTime - dbg_latencyStartTime;
        if (diff > 10000000)
            return;
        TimeSpan ts = new(diff);
        TimeSpan delta = new(diff - dbg_latencyLast);
        dbg_latencyLast = diff;
        Debug.WriteLine($"[LatencyDebug] Event start to {endState} time: {ts.TotalMilliseconds:F2} ms (delta: {delta.TotalMilliseconds:F2} ms)");
    }
#endif

    public override void Dispose()
    {
        base.Dispose();
        drawContext?.Dispose();
        isClosingTokenSource?.Cancel();
        isClosingTokenSource?.Dispose();
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

    public override void RequestRedraw()
    {
        redrawRequested = 1;
        nativeWnd?.ContinueEvents();
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

        mainKeyboard.KeyDown += (keyboard, key, ind) => inputEvents.Add(new(InputEventType.KeyDown, (KeyCode)key));
        mainKeyboard.KeyUp += (keyboard, key, ind) => inputEvents.Add(new(InputEventType.KeyUp, (KeyCode)key));
        mainMouse.MouseDown += (mouse, button) => inputEvents.Add(new(InputEventType.MouseDown, (MouseButton)button));
        mainMouse.MouseUp += (mouse, button) => inputEvents.Add(new(InputEventType.MouseUp, (MouseButton)button));
        mainMouse.MouseMove += (mouse, pos) => inputEvents.Add(new(InputEventType.MouseMove, pos));
        mainMouse.Scroll += (mouse, pos) => inputEvents.Add(new(InputEventType.MouseScroll, new Vector2(pos.X, pos.Y)));
    }

    public static Stream LoadResourceFile(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string? resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(str => str.EndsWith(Path.GetFileName(path)));

        if (resourceName == null || assembly == null)
            throw new FileNotFoundException(path);

        return assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException(path);
    }

    private struct InputEvent
    {
        public InputEventType type;
        public KeyCode key;
        public MouseButton button;
        public Vector2 pos;
        //public Vector2 delta;

        public InputEvent()
        {

        }

        public InputEvent(InputEventType type, KeyCode key)
        {
            this.type = type;
            this.key = key;
        }

        public InputEvent(InputEventType type, MouseButton button)
        {
            this.type = type;
            this.button = button;
        }

        public InputEvent(InputEventType type, Vector2 pos)
        {
            this.type = type;
            this.pos = pos;
        }
    }

    private enum InputEventType
    {
        None,
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp,
        MouseMove,
        MouseScroll
    }
}
