using ArgonUI.Input;
using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.UIElements;
using System.Threading;

namespace ArgonUI;

public abstract class UIWindow : IDisposable
{
    protected readonly ArgonManager argonManager;
    private readonly InputManager inputManager;
    private readonly UIRenderer renderer;
    private long lastRenderTime = 0;
    private long updatePeriod;

    private const long TicksPerSecond = 10 * 1000 * 1000;

    public abstract Thread UIThread { get; }
    public abstract string Title { get; set; }
    // Icon
    public abstract IDrawContext? DrawContext { get; }
    public abstract VectorInt2 Size { get; set; }
    public abstract VectorInt2 Position { get; set; }
    public UIWindowElement RootElement { get; init; }
    public double MaxUpdateRate { get => (double)TicksPerSecond / updatePeriod; set => updatePeriod = (long)(TicksPerSecond / value); }

    public abstract event Action? OnLoaded;
    public abstract event Action? OnClosing;
    public abstract event Action? OnResize;
    public abstract event Action<float>? OnRender;
    public abstract event Action<IEnumerable<string>>? OnFileDrop;

    public UIWindow(ArgonManager argonManager)
    {
        this.argonManager = argonManager;
        MaxUpdateRate = 60;
        argonManager.CreateWindow(this);
        inputManager = argonManager.InputManager;
        RootElement = new(this);
        renderer = new(this);

        this.OnRender += HandleOnRender;
    }

    public abstract void Show();
    public abstract void Minimize();
    public abstract void Maximize();
    public abstract void Close();

    public virtual void Dispose()
    {
        argonManager.DestroyWindow(this);
    }

    /// <summary>
    /// Requests that this window redraws all dirty elements.
    /// </summary>
    public void RequestRedraw()
    {
        var now = DateTime.UtcNow.Ticks;
        if (now - lastRenderTime > updatePeriod)
        {
            lastRenderTime = now;
            renderer?.DrawElements();
            RenderFrame();
        }
    }
    
    /// <summary>
    /// Instructs the window backend to start rendering a new frame. This will eventually result in an <see cref="OnRender"/> invocation.
    /// </summary>
    protected abstract void RenderFrame();

    /// <summary>
    /// This handles the render callback from the window backend. All graphics operation should happen in this thread.
    /// </summary>
    /// <param name="obj"></param>
    private void HandleOnRender(float obj)
    {
        lastRenderTime = DateTime.UtcNow.Ticks;

        renderer.RenderFrame();
    }

    protected void OnMouseMove(UIWindow sender, VectorInt2 mousePos)
    {
        inputManager.OnMouseMove(sender, mousePos);
    }

    protected void OnMouseUp(UIWindow sender, MouseButton mouseButton)
    {
        inputManager.OnMouseUp(sender, mouseButton);
    }

    protected void OnMouseDown(UIWindow sender, MouseButton mouseButton)
    {
        inputManager.OnMouseDown(sender, mouseButton);
    }

    protected void OnMouseWheel(UIWindow sender, Vector2 delta)
    {
        inputManager.OnMouseWheel(sender, delta);
    }

    protected void OnKeyDown(UIWindow sender, KeyCode key)
    {
        inputManager.OnKeyDown(sender, key);
    }

    protected void OnKeyUp(UIWindow sender, KeyCode key)
    {
        inputManager.OnKeyUp(sender, key);
    }

    internal protected abstract void SetMousePos(VectorInt2 mousePos);
}
