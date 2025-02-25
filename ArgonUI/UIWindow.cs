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
    private readonly ManualResetEventSlim closingEvent;
    protected long lastRenderTime = 0;
    protected long updatePeriod;

    private const long TicksPerSecond = 10 * 1000 * 1000;

    /// <summary>
    /// Gets the intenrally managed thread used for UI drawing.
    /// </summary>
    public abstract Thread UIThread { get; }
    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    public abstract string Title { get; set; }
    // Icon
    /// <summary>
    /// Gets the drawing context instance used to draw on the window. 
    /// Must be used from the UI thread.
    /// </summary>
    public abstract IDrawContext? DrawContext { get; }
    /// <summary>
    /// Gets or sets the size of the window in pixels.
    /// Doesn't include the size of any window decorations or borders.
    /// </summary>
    public abstract VectorInt2 Size { get; set; }
    /// <summary>
    /// Gets or sets the position of the window on the screen.
    /// </summary>
    public abstract VectorInt2 Position { get; set; }
    /// <summary>
    /// Gets the root <see cref="UIElement"/> of this window. 
    /// </summary>
    public UIWindowElement RootElement { get; init; }
    /// <summary>
    /// Gets or sets the maximum rate at which the window can be redrawn.
    /// </summary>
    public double MaxUpdateRate { get => (double)TicksPerSecond / updatePeriod; set => updatePeriod = (long)(TicksPerSecond / value); }

    /// <summary>
    /// An event invoked when the window is first loaded and ready to be drawn to.
    /// </summary>
    public abstract event Action? OnLoaded;
    /// <summary>
    /// An event invoked when the window is closing.
    /// </summary>
    public abstract event Action? OnClosing;
    /// <summary>
    /// An event invoked when the window is resized.
    /// </summary>
    public abstract event Action? OnResize;
    /// <summary>
    /// An event invoked by the UI thread at the begining of each frame.
    /// </summary>
    public abstract event Action<float>? OnRender;
    /// <summary>
    /// An event invoked when a drag and drop operation on this window is completed.
    /// </summary>
    public abstract event Action<IEnumerable<string>>? OnFileDrop;

    public UIWindow(ArgonManager argonManager)
    {
        closingEvent = new();
        this.argonManager = argonManager;
        MaxUpdateRate = 60;
        argonManager.CreateWindow(this);
        inputManager = argonManager.InputManager;
        RootElement = new(this);
        renderer = new(this);

        this.OnRender += HandleOnRender;
        this.OnClosing += HandleClosing;
    }

    /// <summary>
    /// Causes the window to become focussed and move to the foreground.
    /// </summary>
    public abstract void Show();
    /// <summary>
    /// Minimises the window to the taskbar.
    /// </summary>
    public abstract void Minimize();
    /// <summary>
    /// Maximises the window to fill the screen.
    /// </summary>
    public abstract void Maximize();
    /// <summary>
    /// Closes the window.
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// Blocks the caller until the window is closed.
    /// </summary>
    public virtual void Wait()
    {
        closingEvent.Wait();
        closingEvent.Reset();
    }

    public virtual void Dispose()
    {
        closingEvent.Dispose();
        argonManager.DestroyWindow(this);
    }

    /// <summary>
    /// Requests that this window redraws all dirty elements.
    /// </summary>
    public abstract void RequestRedraw();
    /*{
        var now = DateTime.UtcNow.Ticks;
        if (now - lastRenderTime > updatePeriod)
        {
            lastRenderTime = now;
            renderer?.DrawElements();
            RenderFrame();
        }
    }*/

    /// <summary>
    /// This handles the render callback from the window backend. All graphics operation should happen in this thread.
    /// </summary>
    /// <param name="obj"></param>
    private void HandleOnRender(float obj)
    {
        lastRenderTime = DateTime.UtcNow.Ticks;

        renderer.DrawElements();
        renderer.RenderFrame();
    }

    private void HandleClosing()
    {
        closingEvent.Set();
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

    /// <summary>
    /// Sets the mouse position in screen space.
    /// </summary>
    /// <param name="mousePos">The screen-space mouse position to set.</param>
    internal protected abstract void SetMousePos(VectorInt2 mousePos);
}
