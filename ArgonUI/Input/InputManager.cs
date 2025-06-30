using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.UIElements;

namespace ArgonUI.Input;

/// <summary>
/// The class responsible for managing and dispatching user input events (eg: mouse, keyboard, etc...) to UI elements.
/// </summary>
public class InputManager
{
    private readonly ArgonManager argonManager;
    private long lastClickTime;
    private long lastMouseMoveTime;
    private VectorInt2 lastMousePos;
    private UIElement? lastHoveredElement;
    private UIElement? kbFocussedElement;
    private UIElement? mouseCaptureElement;
    private readonly Dictionary<KeyCode, bool> pressedKeys;

    public InputManager(ArgonManager argonManager)
    {
        this.argonManager = argonManager;
#if NETSTANDARD
        pressedKeys = [];
        foreach(var key in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
            pressedKeys.Add(key, false);
#else
        pressedKeys = new(Enum.GetValues<KeyCode>().Select(x => new KeyValuePair<KeyCode, bool>(x, false)));
#endif
    }

    /// <summary>
    /// Gets or sets whichever element currently has keyboard focus.
    /// A value of <see langword="null"/> indicates no element has keyboard focus.
    /// </summary>
    public UIElement? FocussedElement 
    { 
        get => kbFocussedElement;
        set
        {
            kbFocussedElement?.InvokeOnFocusLost(this);
            kbFocussedElement = value;
            kbFocussedElement?.InvokeOnFocusGot(this);
        }
    }

    /// <summary>
    /// Gets the <see cref="UIElement"/> which is currently capturing the mouse.
    /// Returns <see langword="null"/> if no element is capturing the mouse.
    /// </summary>
    public UIElement? MouseCaptureElement => mouseCaptureElement;

    /// <summary>
    /// Captures or releases mouse events so that they are only sent to the capturing element.
    /// </summary>
    /// <param name="element">The element which wants to capture the mouse. 
    /// Passing <see langword="null"/> stops capturing the mouse.</param>
    public void CaptureMouse(UIElement? element = null)
    {
        mouseCaptureElement = element;
    }

    /// <summary>
    /// Gets or sets the mouse position in screen space.
    /// </summary>
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
#if DEBUG && DEBUG_PROP_UPDATES
            Debug.WriteLine($"[InputManager] Hovered element changed {lastHoveredElement} -> {hit}");
#endif
            lastHoveredElement?.InvokeOnMouseLeave(this, mousePos);
            hit?.InvokeOnMouseEnter(this, mousePos);
        }

        hit?.InvokeOnMouseOver(this, mousePos);

        lastHoveredElement = hit;
        lastMousePos = mousePos;
    }

    internal void OnMouseUp(UIWindow sender, MouseButton mouseButton)
    {
        lastHoveredElement?.InvokeOnMouseUp(this, mouseButton);
    }

    internal void OnMouseDown(UIWindow sender, MouseButton mouseButton)
    {
        lastHoveredElement?.InvokeOnMouseDown(this, mouseButton);
    }

    internal void OnMouseWheel(UIWindow sender, Vector2 delta)
    {
        lastHoveredElement?.InvokeOnMouseWheel(this, delta);
    }

    internal void OnKeyDown(UIWindow sender, KeyCode key)
    {
        pressedKeys[key] = true;
        lastHoveredElement?.InvokeOnKeyDown(this, key);
    }

    internal void OnKeyUp(UIWindow sender, KeyCode key)
    {
        pressedKeys[key] = false;
        lastHoveredElement?.InvokeOnKeyUp(this, key);
    }

    /// <summary>
    /// Gets the state of a given keyboard key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the given key is currently pressed.</returns>
    public bool IsKeyPressed(KeyCode key)
    {
        if (pressedKeys.TryGetValue(key, out var pressed))
            return pressed;
        return false;
    }

    /// <summary>
    /// Finds whichever element is at the specified point.
    /// </summary>
    /// <param name="window">The window to test.</param>
    /// <param name="mousePos">The position in pixels, relative to the top-left of the window to test.</param>
    /// <returns>The element at the given point, or <see langword="null"/></returns>
    public static UIElement? RaycastElement(UIWindow window, VectorInt2 mousePos)
    {
        return RaycastElementRecurse(window.RootElement, new Vector2(mousePos.x, mousePos.y));
    }

    private static UIElement? RaycastElementRecurse(UIElement element, in Vector2 mousePos)
    {
        if (element.HitTest(mousePos))
        {
            if (element is UIContainer container)
                foreach (var child in container.Children)
                    if (RaycastElementRecurse(child, mousePos) is UIElement childHit)
                        return childHit;

            return element;
        }
        return null;
    }
}

/// <summary>
/// Represents a physical button on a mouse.
/// </summary>
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
