using ArgonUI.Drawing;
using ArgonUI.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace ArgonUI.Backends.Headless;

/// <summary>
/// A UI window that handles events and manages UIElements, but doesn't create a physical window.
/// </summary>
public class HeadlessWindow : UIWindow
{
    private HeadlessDrawContext? drawContext;
    private readonly Thread uiThread;
    private readonly Thread eventThread;
    private Vector2 lastScrollPos;
    private readonly CancellationTokenSource isClosingTokenSource;
    private readonly CancellationToken isClosingToken;
    private readonly BlockingCollection<InputEvent> inputEvents;
    private AutoResetEvent requestRedrawEvent;
    private VectorInt2 size;
    private VectorInt2 wndMousePos;
    private WindowActiveState wndState = WindowActiveState.Active;

    public override Thread UIThread => uiThread;
    public override string Title { get; set; }
    public override IDrawContext? DrawContext => drawContext;
    public override VectorInt2 Size
    {
        get => size;
        set
        {
            size = value;
            OnResize?.Invoke();
        }
    }
    public override VectorInt2 Position { get; set; }
    public override event Action? OnLoaded;
    public override event Action? OnClosing;
    public override event Action? OnResize;
    public override event Action<float>? OnRender;
    public override event Action<IEnumerable<string>>? OnFileDrop;

    public WindowActiveState WindowState => wndState;

    public HeadlessWindow(ArgonManager argonManager, string title = "Headless Argon Window") : base(argonManager)
    {
        Title = title;
        isClosingTokenSource = new();
        isClosingToken = isClosingTokenSource.Token;
        inputEvents = [];
        uiThread = new(() => RunUIThread(title));
        uiThread.Priority = ThreadPriority.AboveNormal;
        uiThread.Name = "ArgonUIThread";

        eventThread = new(RunEventThread);
        eventThread.Priority = ThreadPriority.Normal;
        eventThread.Name = "ArgonUIEventThread";

        requestRedrawEvent = new(true);
        drawContext = new();

        uiThread.Start();
        eventThread.Start();
    }

    public override void Dispose()
    {
        base.Dispose();
        drawContext?.Dispose();
        isClosingTokenSource?.Cancel();
        isClosingTokenSource?.Dispose();
    }

    public override void Show()
    {
        wndState = WindowActiveState.Active;
    }

    public override void Minimize()
    {
        wndState = WindowActiveState.Minimised;
    }

    public override void Maximize()
    {
        wndState = WindowActiveState.Active;
    }

    public override void Close()
    {
        OnClosing?.Invoke();
        wndState = WindowActiveState.Closed;
        isClosingTokenSource?.Cancel();
    }

    public override void RequestRedraw()
    {
        requestRedrawEvent?.Set();
    }

    protected override void SetMousePos(VectorInt2 mousePos)
    {
        SendMouseMove(mousePos);
    }

    private void RunUIThread(string title)
    {
        DateTime startTime = DateTime.UtcNow;
        Stopwatch sw = Stopwatch.StartNew();
        OnLoaded?.Invoke();
        WaitHandle[] waitHandles = [requestRedrawEvent, isClosingToken.WaitHandle];
        try
        {
            while (!isClosingToken.IsCancellationRequested)
            {
                WaitHandle.WaitAny(waitHandles);
                if (isClosingToken.IsCancellationRequested)
                    break;
                float elapsed = (float)sw.Elapsed.TotalSeconds;
                sw.Restart();
                OnRender?.Invoke(elapsed);
            }
        } 
        catch (OperationCanceledException) { }
    }

    private void RunEventThread()
    {
        try
        {
            while (true)
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
                        OnMouseMove(this, evnt.pos);
                        break;
                    case InputEventType.MouseScroll:
                        var delta = evnt.delta - lastScrollPos;
                        OnMouseWheel(this, delta);
                        lastScrollPos = evnt.delta;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>
    /// Sends a simulated click to this headless window.
    /// </summary>
    /// <param name="clickPos">Where to click on the window in screen coordinates.</param>
    /// <param name="button">Which button to click with.</param>
    /// <param name="restoreMousePos">Whether the mouse should be moved back to it's previous position after clicking.</param>
    public void SendClick(VectorInt2 clickPos, MouseButton button = MouseButton.Left, bool restoreMousePos = true)
    {
        var prevPos = wndMousePos;
        SendMouseMove(clickPos);
        SendMouseDown(button);
        SendMouseUp(button);
        if (restoreMousePos)
            SendMouseMove(prevPos);
    }

    /// <summary>
    /// Sends a simulated keypress (with up to 3 modifier keys) to this headless window.
    /// </summary>
    /// <param name="key">The key to press.</param>
    /// <param name="modifier1">Optionally, a modifier key to press while pressing the desired key.</param>
    /// <param name="modifier2">Optionally, a modifier key to press while pressing the desired key.</param>
    /// <param name="modifier3">Optionally, a modifier key to press while pressing the desired key.</param>
    public void SendKey(KeyCode key, KeyCode modifier1 = KeyCode.Unknown, KeyCode modifier2 = KeyCode.Unknown, KeyCode modifier3 = KeyCode.Unknown)
    {
        if (modifier1 != KeyCode.Unknown)
            SendKeyDown(modifier1);
        if (modifier2 != KeyCode.Unknown)
            SendKeyDown(modifier2);
        if (modifier3 != KeyCode.Unknown)
            SendKeyDown(modifier3);

        SendKeyDown(key);
        SendKeyUp(key);

        if (modifier3 != KeyCode.Unknown)
            SendKeyUp(modifier3);
        if (modifier2 != KeyCode.Unknown)
            SendKeyUp(modifier2);
        if (modifier1 != KeyCode.Unknown)
            SendKeyUp(modifier1);
    }

    public void SendMouseMove(VectorInt2 mousePos)
    {
        wndMousePos = mousePos;
        inputEvents.Add(new(InputEventType.MouseMove, mousePos));
    }

    public void SendMouseUp(MouseButton mouseButton)
    {
        inputEvents.Add(new(InputEventType.MouseUp, mouseButton));
    }

    public void SendMouseDown(MouseButton mouseButton)
    {
        inputEvents.Add(new(InputEventType.MouseDown, mouseButton));
    }

    public void SendMouseWheel(Vector2 delta)
    {
        inputEvents.Add(new(InputEventType.MouseScroll, delta));
    }

    public void SendKeyDown(KeyCode key)
    {
        inputEvents.Add(new(InputEventType.KeyDown, key));
    }

    public void SendKeyUp(KeyCode key)
    {
        inputEvents.Add(new(InputEventType.KeyUp, key));
    }

    private struct InputEvent
    {
        public InputEventType type;
        public KeyCode key;
        public MouseButton button;
        public VectorInt2 pos;
        public Vector2 delta;

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

        public InputEvent(InputEventType type, VectorInt2 pos)
        {
            this.type = type;
            this.pos = pos;
        }

        public InputEvent(InputEventType type, Vector2 delta)
        {
            this.type = type;
            this.delta = delta;
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

    public enum WindowActiveState
    {
        Active,
        Minimised,
        Closed
    }
}
