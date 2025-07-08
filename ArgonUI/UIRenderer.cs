using ArgonUI.Drawing;
using ArgonUI.Helpers;
using ArgonUI.UIElements;
using ArgonUI.UIElements.Abstract;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ArgonUI;

internal class UIRenderer
{
    private readonly UIWindow window;
    private IDrawContext? context;
    //private readonly DrawCommandGraph drawCommandGraph;
    //private readonly List<Action<IDrawContext>> drawCommands;
    private readonly SortedRefList<int, TemporaryList<Action<IDrawContext>>> drawCommands;
    private Bounds2D? drawBounds;

    // These are used to prevent race conditions
    private volatile int uiTreeOperationsInProgress = 0;

    public UIRenderer(UIWindow window)
    {
        this.window = window;
        //drawCommandGraph = new(window.RootElement);
        drawCommands = [];
        //window.OnLoaded += () => window.DrawContext?.InitRenderer(window);
        //window.DrawContext?.InitRenderer(window);
    }

    /// <summary>
    /// Performs the layout and drawing of all dirty elements.
    /// </summary>
    public void DrawElements()
    {
#if DEBUG && DEBUG_PROP_UPDATES
        Debug.WriteLine($"[UIRenderer] Draw START");
#endif

        // Lazily wait for whatever operation is occuring to finish
        if (uiTreeOperationsInProgress != 0)
        {
            //SpinWait spinner = default;
            int spins = 0;
            while (uiTreeOperationsInProgress > 0 && spins < 1024)
            {
                //spinner.SpinOnce();
                Thread.Yield();
                spins++;
            }
            uiTreeOperationsInProgress = 0;
        }

        drawBounds = null;
        drawCommands.Clear();
        lock (window.UITreeUpdateLock)
        {
            MeasureElementRecurse(window.RootElement);
            LayoutElementRecurse(window.RootElement);
            MeasureDrawnElementRecurse(window.RootElement);
            CollectDrawnRecurse(window.RootElement);
        }
        // Cancel the current draw if elements were dirtied during the draw
        //if (rootWasDirtied)
        //    drawCommands.Clear();

#if DEBUG && DEBUG_PROP_UPDATES
        Debug.WriteLine($"[UIRenderer] Draw END ({drawCommands.Count} layers to draw)");
#endif
    }

    /// <summary>
    /// Renders the drawing commands collected from the dirty elements. Must be called from the UI thread.
    /// </summary>
    public void RenderFrame()
    {
        context = window.DrawContext;
        if (context == null)
            return;

        if (drawCommands.Count == 0 || !drawBounds.HasValue)
            return;

        context.StartFrame(drawBounds.Value);
        //context.StartFrame(Bounds2D.Zero);
        foreach (var drawLayer in drawCommands.Values)
        {
            foreach (var drawCommand in drawLayer)
                drawCommand(context);
            drawLayer.Dispose();
        }
        context.FlushBatch();
        context.EndFrame();
    }

    private static bool MeasureElementRecurse(UIElement element)
    {
        if (element is UIContainer container && (element.DirtyFlags & DirtyFlag.ChildLayout) != 0)
        {
            bool changed = false;
            for (int i = 0; i < container.Children.Count; i++)
            {
                UIElement? child = container.Children[i];
                changed |= MeasureElementRecurse(child);
            }
            if (changed)
                container.BeforeLayoutChildren();
        }

        if ((element.DirtyFlags & DirtyFlag.Layout) != 0)
        {
            var oldSize = element.desiredSize;
            if (element.Visible != Visibility.Hidden)
                element.desiredSize = element.Measure();
            else
                element.desiredSize = Vector2.Zero;
            if (element.desiredSize != oldSize)
                return true;
        }
        return false;
    }

    private static void LayoutElementRecurse(UIElement element, int childIndex = 0)
    {
        if ((element.DirtyFlags & DirtyFlag.Layout) != 0)
        {
            element.ClearDirtyFlag(DirtyFlag.Layout);
            if (element.Visible != Visibility.Hidden)
            {
                var bounds = element.Layout(childIndex);
                if (element.RenderedBoundsAbsolute.Width == 0)
                    element.invalidatedRenderBounds = bounds;
                else
                    element.invalidatedRenderBounds = bounds.Union(element.RenderedBoundsAbsolute);
                element.RenderedBoundsAbsolute = bounds;
                // TODO: Do we even need RenderedBounds
                //element.RenderedBounds = new(bounds.topLeft - parentBounds.topLeft, bounds.bottomRight - parentBounds.topLeft);
                var renderedSize = bounds.Size;
                element.RenderedWidth = renderedSize.X;
                element.RenderedHeight = renderedSize.Y;
            }
        }

        if (element is UIContainer container && (element.DirtyFlags & DirtyFlag.ChildLayout) != 0)
        {
            container.LayoutChildren();
            element.ClearDirtyFlag(DirtyFlag.ChildLayout);
            try
            {
                for (int i = 0; i < container.Children.Count; i++)
                {
                    UIElement? child = container.Children[i];
                    LayoutElementRecurse(child, i);
                }
            }
            catch { } // TODO: There can be a race condition if the element tree is modified during rendering. For now, just skip rendering if we encounter an error.
        }
    }

    private void MeasureDrawnElementRecurse(UIElement element)
    {
        // Expand the draw bounds to include an elements with dirty content
        if ((element.DirtyFlags & DirtyFlag.Content) != 0)
        {
            element.ClearDirtyFlag(DirtyFlag.Content);

            // Draw bounds should be computed during layout...
            if (!drawBounds.HasValue)
                drawBounds = element.invalidatedRenderBounds;
            else
                drawBounds = drawBounds.Value.Union(element.invalidatedRenderBounds);

            // Fast path, given that all child elements must fit within their parent, we know the
            // drawBounds won't expand any further. We still need to clear any dirty flags though.
            ClearContentFlagsRecursive(element);
        } 
        else if ((element.DirtyFlags & DirtyFlag.ChildContent) != 0 && element is UIContainer container)
        {
            element.ClearDirtyFlag(DirtyFlag.ChildContent);
            try
            {
                foreach (UIElement? child in container.Children)
                    MeasureDrawnElementRecurse(child);
            }
            catch { } // TODO: There can be a race condition if the element tree is modified during rendering. For now, just skip rendering if we encounter an error.
        }
    }

    private static void ClearContentFlagsRecursive(UIElement element)
    {
        element.ClearDirtyFlag(DirtyFlag.Content);
        if ((element.DirtyFlags & DirtyFlag.ChildContent) != 0 && element is UIContainer container)
        {
            element.ClearDirtyFlag(DirtyFlag.ChildContent);
            try
            {
                foreach (UIElement? child in container.Children)
                    ClearContentFlagsRecursive(child);
            }
            catch { } // TODO: There can be a race condition if the element tree is modified during rendering. For now, just skip rendering if we encounter an error.
        }
    }

    private void CollectDrawnRecurse(UIElement element)
    {
        if (!drawBounds.HasValue || !element.RenderedBoundsAbsolute.Intersects(drawBounds.Value))
            return;

        if (element.Visible != Visibility.Visible)
            return;

        int index = element.treeDepth + element.ZIndex;

        // TODO: A lot of UI elements are likely to layer multiple different draw commands. It would be good
        // if we could expose the layering system to them so that they can take advantage of auto batching
        // with neighbours. Otherwise we need to layer UI elements, which is a bit heavy weight...
        // This is notably the case for decoraters like outlines or shadows.
        ref var cmdList = ref drawCommands.GetRef(index);
        if (Unsafe.IsNullRef(ref cmdList))
        {
            TemporaryList<Action<IDrawContext>> newCmdList = [element.Draw];
            drawCommands.Add(index, newCmdList);
        }
        else
        {
            // TODO: Since this is a non-static method, getting a delegate to this method results in a small GC allocation.
            cmdList.Add(element.Draw);
        }

        // Debug.WriteLine($"[UIRenderer] Draw: {new string(' ', element.treeDepth)}{element}");

        if (element is UIContainer container)
        {
            try
            {
                foreach (var child in container.Children)
                    CollectDrawnRecurse(child);
            }
            catch { }
        }
    }

    internal void StartUITreeOperation()
    {
        uiTreeOperationsInProgress++;
    }

    internal void EndUITreeOperation()
    {
        uiTreeOperationsInProgress--;
    }
}
