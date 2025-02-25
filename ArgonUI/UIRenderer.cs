using ArgonUI.UIElements;
using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

internal class UIRenderer
{
    private readonly UIWindow window;
    private IDrawContext? context;
    private readonly DrawCommandGraph drawCommandGraph;
    private readonly List<Action<IDrawContext>> drawCommands;
    private Bounds2D drawBounds;

    public UIRenderer(UIWindow window)
    {
        this.window = window;
        drawCommandGraph = new(window.RootElement);
        drawCommands = [];
        //window.OnLoaded += () => window.DrawContext?.InitRenderer(window);
        //window.DrawContext?.InitRenderer(window);
    }

    /// <summary>
    /// Performs the layout and drawing of all dirty elements.
    /// </summary>
    public void DrawElements()
    {
        drawBounds = Bounds2D.Zero;
        drawCommands.Clear();
        LayoutElementRecurse(window.RootElement, drawCommandGraph);
        DrawElementRecurse(window.RootElement, drawCommandGraph);
        BuildDrawCommandsRecurse(drawCommandGraph);
    }

    /// <summary>
    /// Renders the drawing commands collected from the dirty elements. Must be called from the UI thread.
    /// </summary>
    public void RenderFrame()
    {
        context = window.DrawContext;
        if (context == null)
            return;

        if (drawCommands.Count == 0)
            return;

        context.StartFrame(drawBounds);
        //context.StartFrame(Bounds2D.Zero);
        foreach (var drawCommand in drawCommands)
            drawCommand(context);
        context.FlushBatch();
        context.EndFrame();
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

    private void DrawElementRecurse(UIElement element, DrawCommandGraph drawGraph)
    {
        if ((element.DirtyFlags & DirtyFlags.Content) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.Content);
            element.Draw(element.RenderedBoundsAbsolute, drawGraph.DrawCommands);

            drawBounds = drawBounds.Union(element.RenderedBoundsAbsolute);
        }

        drawCommands.AddRange(drawGraph.DrawCommands);

        if (element is IContainer container && (element.DirtyFlags & DirtyFlags.ChildContent) != 0)
        {
            element.ClearDirtyFlag(DirtyFlags.ChildContent);
            foreach (var child in container.Children)
                DrawElementRecurse(child, drawGraph.Children.GetOrAdd(child, () => new(child)));
        }
    }

    private void BuildDrawCommandsRecurse(DrawCommandGraph drawGraph)
    {
        drawCommands.AddRange(drawGraph.DrawCommands);
        foreach (var child in drawGraph.Children.Values)
            BuildDrawCommandsRecurse(child);
    }
}
