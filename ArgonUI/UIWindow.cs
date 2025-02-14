using ArgonUI.Input;
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
    private long lastRenderTime = 0;
    private long updatePeriod;
    private DrawCommandGraph drawCommandGraph;

    private const long TicksPerSecond = 10 * 1000 * 1000;

    public abstract Thread UIThread { get; }
    public abstract string Title { get; set; }
    // Icon
    public abstract IDrawContext DrawContext { get; }
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
        drawCommandGraph = new(RootElement);

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

        LayoutElementRecurse(RootElement, drawCommandGraph);
        DrawElementRecurse(RootElement, drawCommandGraph);
    }

    private static void LayoutElementRecurse(UIElement element, DrawCommandGraph drawGraph)
    {
        if ((element.DirtyFlags & DirtyFlags.Layout) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.Layout);
            element.Layout();
        }

        if (element is IContainer container && (element.DirtyFlags & DirtyFlags.ChildLayout) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.ChildLayout);
            foreach (var child in container.Children)
                LayoutElementRecurse(child, drawGraph.Children.GetOrAdd(child, () => new(child)));
        }
    }

    private static void DrawElementRecurse(UIElement element, DrawCommandGraph drawGraph)
    {
        if ((element.DirtyFlags & DirtyFlags.Content) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.Content);
            element.Draw(element.RenderedBoundsAbsolute, drawGraph.DrawCommands);
        }

        if (element is IContainer container && (element.DirtyFlags & DirtyFlags.ChildContent) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.ChildContent);
            foreach (var child in container.Children)
                DrawElementRecurse(child, drawGraph.Children.GetOrAdd(child, () => new(child)));
        }
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
