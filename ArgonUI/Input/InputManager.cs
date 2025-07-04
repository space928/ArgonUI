using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ArgonUI.Helpers;
using ArgonUI.UIElements;

namespace ArgonUI.Input;

/// <summary>
/// The class responsible for managing and dispatching user input events (eg: mouse, keyboard, etc...) to UI elements.
/// </summary>
public class InputManager
{
    private readonly ArgonManager argonManager;
    private long lastClickTime;
    //private long lastMouseMoveTime;
    private VectorInt2 lastMousePos;
    private UIElement? lastHoveredElement;
    private UIElement? kbFocussedElement;
    private UIElement? mouseCaptureElement;
    private readonly Dictionary<KeyCode, bool> pressedKeys;
    private readonly long doubleClickTime;

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
        doubleClickTime = TimeSpan.FromMilliseconds(300).Ticks;
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
            if (value == kbFocussedElement) 
                return;

            var args = ObjectPool<FocusInputEventArgs>.Rent();
            args.InputManager = this;
            args.Target = kbFocussedElement;
            var obj = kbFocussedElement;
            do
            {
                obj?.InvokeOnFocusLost(args);
            } while (!args.Handled && (obj = obj?.Parent) != null);

            kbFocussedElement = value;

            obj = kbFocussedElement;
            do
            {
                obj?.InvokeOnFocusGot(args);
            } while (!args.Handled && (obj = obj?.Parent) != null);

            ObjectPool<FocusInputEventArgs>.Return(args);
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
            var args = ObjectPool<MousePositionInputEventArgs>.Rent();
            args.InputManager = this;
            args.Target = lastHoveredElement;
            args.MousePosition = mousePos;
            var obj = lastHoveredElement;
            do
            {
                obj?.InvokeOnMouseLeave(args);
            } while (!args.Handled && (obj = obj?.Parent) != null);

            args.Target = hit;
            obj = hit;
            do
            {
                obj?.InvokeOnMouseEnter(args);
            } while (!args.Handled && (obj = obj?.Parent) != null);
            ObjectPool<MousePositionInputEventArgs>.Return(args);
        }

        var hoverArgs = ObjectPool<MousePositionInputEventArgs>.Rent();
        hoverArgs.InputManager = this;
        hoverArgs.Target = hit;
        hoverArgs.MousePosition = mousePos;
        var hoverObj = hit;
        do
        {
            hoverObj?.InvokeOnMouseOver(hoverArgs);
        } while (!hoverArgs.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<MousePositionInputEventArgs>.Return(hoverArgs);

        lastHoveredElement = hit;
        lastMousePos = mousePos;
    }

    internal void OnMouseUp(UIWindow sender, MouseButton mouseButton)
    {
        var args = ObjectPool<MouseButtonInputEventArgs>.Rent();
        args.InputManager = this;
        args.Target = lastHoveredElement;
        args.MouseButton = mouseButton;
        var hoverObj = lastHoveredElement;
        do
        {
            hoverObj?.InvokeOnMouseUp(args);
        } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<MouseButtonInputEventArgs>.Return(args);
    }

    internal void OnMouseDown(UIWindow sender, MouseButton mouseButton)
    {
        var now = DateTime.UtcNow.Ticks;
        var args = ObjectPool<MouseButtonInputEventArgs>.Rent();
        args.InputManager = this;
        args.Target = lastHoveredElement;
        args.MouseButton = mouseButton;
        var hoverObj = lastHoveredElement;
        if (now - lastClickTime <= doubleClickTime)
        {
            do
            {
                hoverObj?.InvokeOnDoubleClick(args);
            } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        }
        do
        {
            hoverObj?.InvokeOnMouseDown(args);
        } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<MouseButtonInputEventArgs>.Return(args);
        lastClickTime = now;
    }

    internal void OnMouseWheel(UIWindow sender, Vector2 delta)
    {
        var args = ObjectPool<MouseScrollInputEventArgs>.Rent();
        args.InputManager = this;
        args.Target = lastHoveredElement;
        args.MouseScroll = delta;
        var hoverObj = lastHoveredElement;
        do
        {
            hoverObj?.InvokeOnMouseWheel(args);
        } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<MouseScrollInputEventArgs>.Return(args);
    }

    internal void OnKeyDown(UIWindow sender, KeyCode key)
    {
        pressedKeys[key] = true;
        var args = ObjectPool<KeyboardInputEventArgs>.Rent();
        args.InputManager = this;
        args.Target = FocussedElement ?? lastHoveredElement;
        args.Key = key;
        var hoverObj = args.Target;
        do
        {
            hoverObj?.InvokeOnKeyDown(args);
        } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<KeyboardInputEventArgs>.Return(args);
    }

    internal void OnKeyUp(UIWindow sender, KeyCode key)
    {
        pressedKeys[key] = false;
        var args = ObjectPool<KeyboardInputEventArgs>.Rent();
        args.InputManager = this;
        args.Target = FocussedElement ?? lastHoveredElement;
        args.Key = key;
        var hoverObj = args.Target;
        do
        {
            hoverObj?.InvokeOnKeyUp(args);
        } while (!args.Handled && (hoverObj = hoverObj?.Parent) != null);
        ObjectPool<KeyboardInputEventArgs>.Return(args);
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

public class InputEventArgs(InputManager? inputManager, UIElement? target, bool handled)
{
    /// <summary>
    /// The input manager which sent this event.
    /// </summary>
    public InputManager? InputManager { get; internal set; } = inputManager;
    /// <summary>
    /// The <see cref="UIElement"/> which initially received this event.
    /// </summary>
    public UIElement? Target { get; internal set; } = target;
    /// <summary>
    /// Whether this event has been handled yet. Once set to <see langword="true"/>, this event 
    /// will stop propagating up to parent elements.
    /// </summary>
    public bool Handled { get; set; } = handled;

    public InputEventArgs() : this(null, null, false) { }
}

public sealed class FocusInputEventArgs : InputEventArgs
{
    public FocusInputEventArgs(InputManager? inputManager, UIElement? target, bool handled) : base(inputManager, target, handled)
    {
    }

    public FocusInputEventArgs() : this(null, null, false) { }
}

public sealed class MousePositionInputEventArgs : InputEventArgs
{
    public VectorInt2 MousePosition { get; internal set; }

    public MousePositionInputEventArgs(InputManager? inputManager, UIElement? target, bool handled, VectorInt2 mousePos)
        : base(inputManager, target, handled)
    {
        MousePosition = mousePos;
    }

    public MousePositionInputEventArgs() : this(null, null, false, VectorInt2.Zero) { }
}

public sealed class MouseButtonInputEventArgs : InputEventArgs
{
    public MouseButton MouseButton { get; internal set; }

    public MouseButtonInputEventArgs(InputManager? inputManager, UIElement? target, bool handled, MouseButton mouseButton)
        : base(inputManager, target, handled)
    {
        MouseButton = mouseButton;
    }

    public MouseButtonInputEventArgs() : this(null, null, false, MouseButton.Mouse0) { }
}

public sealed class MouseScrollInputEventArgs : InputEventArgs
{
    public Vector2 MouseScroll { get; internal set; }

    public MouseScrollInputEventArgs(InputManager? inputManager, UIElement? target, bool handled, Vector2 mouseScroll)
        : base(inputManager, target, handled)
    {
        MouseScroll = mouseScroll;
    }

    public MouseScrollInputEventArgs() : this(null, null, false, Vector2.Zero) { }
}

public sealed class KeyboardInputEventArgs : InputEventArgs
{
    public KeyCode Key { get; internal set; }

    public KeyboardInputEventArgs(InputManager? inputManager, UIElement? target, bool handled, KeyCode key)
        : base(inputManager, target, handled)
    {
        Key = key;
    }

    public KeyboardInputEventArgs() : this(null, null, false, KeyCode.Unknown) { }
}
