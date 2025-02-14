using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.UIElements;

namespace ArgonUI.Input;

public class InputManager
{
    private readonly ArgonManager argonManager;
    private long lastClickTime;
    private long lastMouseMoveTime;
    private VectorInt2 lastMousePos;
    private UIElement? lastHoveredElement;
    private UIElement? focussedElement;
    private Dictionary<KeyCode, bool> pressedKeys;

    public InputManager(ArgonManager argonManager)
    {
        this.argonManager = argonManager;
        pressedKeys = new(Enum.GetValues<KeyCode>().Select(x => new KeyValuePair<KeyCode, bool>(x, false)));
    }

    public VectorInt2 MousePos
    {
        get => lastMousePos;
        set
        {
            if (argonManager.Windows.Count > 0)
                argonManager.Windows[0].SetMousePos(value);
        }
    }

    internal void OnMouseMove(UIWindow sender, VectorInt2 mousePos)
    {
        var hit = RaycastElement(sender, mousePos);
        if (hit != lastHoveredElement)
        {
            lastHoveredElement?.InvokeOnMouseLeave(mousePos);
            hit?.InvokeOnMouseEnter(mousePos);
        }

        if (hit != null)
        {
            hit.InvokeOnMouseOver(mousePos);
        }

        lastHoveredElement = hit;
        lastMousePos = mousePos;
    }

    internal void OnMouseUp(UIWindow sender, MouseButton mouseButton)
    {
        lastHoveredElement?.InvokeOnMouseUp(mouseButton);
    }

    internal void OnMouseDown(UIWindow sender, MouseButton mouseButton)
    {
        lastHoveredElement?.InvokeOnMouseDown(mouseButton);
    }

    internal void OnMouseWheel(UIWindow sender, Vector2 delta)
    {
        lastHoveredElement?.InvokeOnMouseWheel(delta);
    }

    internal void OnKeyDown(UIWindow sender, KeyCode key)
    {
        lastHoveredElement?.InvokeOnKeyDown(key);
    }

    internal void OnKeyUp(UIWindow sender, KeyCode key)
    {
        lastHoveredElement?.InvokeOnKeyUp(key);
    }

    public bool IsKeyPressed(KeyCode key)
    {
        if (pressedKeys.TryGetValue(key, out var pressed))
            return pressed;
        return false;
    }

    public static UIElement? RaycastElement(UIWindow window, VectorInt2 mousePos)
    {
        return RaycastElementRecurse(window.RootElement, new Vector2(mousePos.x, mousePos.y));
    }

    private static UIElement? RaycastElementRecurse(UIElement element, in Vector2 mousePos)
    {
        if (element.RenderedBoundsAbsolute.Contains(mousePos))
        {
            if (element is IContainer container)
                foreach (var child in container.Children)
                    if (RaycastElementRecurse(child, mousePos) is UIElement childHit)
                        return childHit;

            return element;
        }
        return null;
    }
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    Mouse0 = Left,
    Mouse1 = Right,
    Mouse2 = Middle,
    Mouse3,
    Mouse4,
    Mouse5,
    Mouse6,
    Mouse7,
    Mouse8,
    Mouse9,
}
