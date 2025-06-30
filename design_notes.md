# Data Binding

All bindable stuff should implement `INotifyPropertyChanged` (or maybe a fancier 
version of it that's more efficient/provides more info). To bind a view property 
(eg: `Rectangle.Rounding`) to a model property (`eg: ViewModel.RectRound`) we 
create a new `Binding` object. Maybe the `UIElement` should have a factory for 
bindings which registers them in a list in the `UIElement` to keep track of them,
and prevent them from being GC-ed.

When a `Binding : IDisposable` is created it takes a `new Binding(INotifyPropertyChanged 
srcObj, string srcProp, UIElement dstObj, string dstProp, [BindingDirection dir, 
UpdateMode mode, ValidateCallback validate, UpdateCallback update])`. In the 
constructor, it would subscribe to the `NotifyPropertyChanged` event and use it to 
propagate changes as specified by the other parameters.

# Drawing
Say we have a UI with the following component graph:
```
0 [Stack Panel]
1 |- [Grid]
2 |  |- [Button]
3 |  |- [Button]
4 |- [Button]

Eg:
============== - [] X
| [Butt 2] [Butt 3] |
| [     Butt 4    ] |
|                   |
=====================
```

The basic steps for a rendered frame are:

1. Handle input, invalidating the layout/content of any given controls.
1. If the layout is invalid:
    1. Measure invalid components
	1. Layout invalid components -> updates drawing lists
1. If any components have invalid content:
	1. Call draw() on each invalid component -> updates drawing lists
1. Optimise drawing lists
1. Execute drawing lists (clipping to only draw the invalid area)

Drawing lists:  
```
{
	ref: StackPanel0
	bounds: ...,
	cmds: [
		drawRect(...) // Includes bg, border, and rounded corners
		//drawTexturedRect(...) // Includes tex, border, and rounded corners
	]
	children: [
		{
			ref: Grid1,
			bounds: ...,
			cmds: [
				drawRect(...)
			],
			children: [
				{
					ref: Button2,
					bounds: ...,
					cmds: [
						drawRect(...)
						drawText(...)
					],
					children: []
				},
				{
					ref: Button3,
					bounds: ...,
					cmds: [
						drawRect(...)
						drawText(...)
					],
					children: []
				},
			]
		},
		{
			ref: Button4,
			bounds: ...,
			cmds: [
				drawRect(...)
				drawText(...)
			],
			children: []
		}
	]
}
```

Drawing lists should be drawn back to front for correct alpha-blending. This 
means executing drawing lists in a breadth-first traversal. As an optimisation, 
opaque children could be drawn earlier using a z-buffer to reduce overdraw.

An ideal version of the above drawing list (assuming no items are opaque (which is 
the case when using round corners)) would look like:
```
drawRect(...) // StackPanel0
drawRect(...) // Grid1
drawRect(...) // Button2
| drawRect(...) // Button3 // batched with prev
| drawRect(...) // Button4
drawText(...) // Button2
| drawText(...) // Button3
| drawText(...) // Button4
```

Maybe we could make the primitive drawing methods (`drawRect`, `drawTexture`, 
`drawText`, etc...) a bit smarter by automatically batching subsequant calls.
IE:

```cs
int vertPos = 0;
int vertCount = 0;
uint[1024] vertBuff;
ShaderProgramInfo? program = null;

void drawRect(...) {
	if (program != RECT_PROGRAM || _vertBuff.Length - vertPos < sizeof(RectVert) * 4)
		flushBatch();

	drawRectVert(...);
	drawRectVert(...);
	drawRectVert(...);
	drawRectVert(...);

	void drawRectVert(pos, col, rounding) {
		Unsafe.WriteUnaligned(ref vertBuff[vertPos], pos);
		vertPos += sizeof(Vec2);
		Unsafe.WriteUnaligned(ref vertBuff[vertPos], col);
		vertPos += sizeof(Colour);
		Unsafe.WriteUnaligned(ref vertBuff[vertPos], rounding);
		vertPos += sizeof(float);
		vertCount++;
	}
}

void flushBatch() {
	if (vertPos == 0)
		return;

	glUseProgram(program.program);
	// Set all other program properties...
	// Bind textures and stuff

	glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(uint) * vertPos, vertBuff);
	
	glBindVertexArray(renderer->vao);
	glDrawArrays(GL_TRIANGLE_STRIPS, 0, vertCount);
	
	vertPos = 0;
	vertCount = 0;
}

// Don't forget to flush the final batch
```

### Example 1: OnHover BG Change

1. Button 2 receives a mouse enter event
	1. It's background colour is updated
1. It's content is invalidated (`IsLayoutDirty = false`) (`IsContentDirty = true`)
	1. This propagates a `IsChildContentDirty` flag up to the root object
1. Skip Measure/Layout
1. Draw components:
	1. Traverse tree until we find `IsContentDirty`
	1. Update the drawing commands: `Button2`'s `drawRect()` will have it's parameters updated
	1. Expands the current `DirtyRegion` to include this component's bounds
1. Render dirty region:
	1. Set scissor rectangle
	1. Traverse the drawing tree (breadth-first)
		1. Skip any components (and their children) whose bounds don't intersect the dirty region
		1. Push(/insert?) it's drawing commands to the current drawing list
		1. Optimise drawing list
	1. Execute the drawing list:
		1. Clear BG
		1. Foreach cmd -> command()
		1. Swap

## Textures

We need an abstract 2D texture interface which allows:
 - Textures can be loaded from files (png, dds, bmp, jpeg, etc...).
 - Textures can be loaded from a contiguous array of pixels.
 - Textures need to be abstracted from the backend.
 - Textures should be loaded asynchronously.
 - Texture data can be updated after creation.

For the OpenGL backend the plan is to have a couple of classes:

**A general Texture2D class**. Which would implement all the texture functionality needed by 
OpenGL:
 - Texture creation
 - Texture binding
 - Uploading texture data to the GPU
 - Dummy texture generation for unloaded textures (black, magenta, checkerboard)

**A Texture2D loader class**. Implements loading from compressed formats (png, jpeg, dds, etc...):
 - Needs to happen in the backend since dds, should remain compressed and be uploaded to the GPU 
   directly.
 - Should be async, as texture decompression can take quite a while.

**ArgonTexture**  
On the front end we need a texture abstraction which exposes some useful properties loaded from 
the texture (width, height, etc...). These textures (called `ArgonTexture` from here on out), 
provide the following functions:
 - Creation from file name
 - Creation from byte stream
 - Creation from image bytes
 - Update region (with `Memory<byte>`)
 - Updating/creation from outside the `IDrawContext`

The `ArgonTexture` holds a nullable reference to an `ITextureHandle` which is implemented by 
`Texture2D`. All texture GL operations are declared by the `IDrawContext` and as such doing 
anything to a texture requires an instance of the drawing context. For instance when an 
`ArgonTexture` is created, it calls `IDrawContext.LoadTexture(...)` which returns an
`ITextureHandle` which may or may not be valid (ie fully loaded) at any given time. To be able
to perform texture operations at any time, the `ArgonTexture` stores a list of the texture 
operations which need to be applied.

### Drawing Lists V2

Organising our drawing lists into a tree that matches that of the UI hierarchy often makes it hard 
to batch things together. (Take for instance a grid of buttons containing labels). The new 
suggestion, is that the UI engine maitains a list of lists of drawing commands, so when a couple of
buttons are added to the drawing list it would look like so:
```
drawCmds = [
	[wnd.bg],
	[stackPanel.rect],
	[button1.rect, button2.rect],
	[button1.text, button2.text]
]
```
Here, the draw order is preserved localy (so labels appear on top of buttons) but the rects can
take advantage of auto batching. 

For each element draw the sublist index it goes into is determined by:
`listInd = treeDepth + zIndex`. Since `zIndex` can be really big or really small, it might make 
sense to clamp to one above and one below the current min and max index and then re-order when 
needed. Or I guess we could use a heap.


# Input

# Styling

At the moment styling/drawing are done in a fairly conventional way where each `UIElement` exposes 
a number of properties which can be set to control the look of an element. This is simple but has 
a few limitations:

1. Styling many similar components requires manually setting these style properties on each.
	1. Usually users want all components to look similar, ie: share common colours, etc...
	1. Many users are familiar with CSS, which allows components to inherit the style of their 
	   parents. I'm specifically talking about both components that derive from each other (ie: 
	   `Button` and `ImageButton`) and components that are children in the UI hierarchy (ie: a 
	   style on a container should apply to the components it contains).
1. Animating components requires manually updating the style properties at a regular interval.
	1. Again, CSS animations spoil us.
	1. Having animation handled in the style object would be quite nice.
	1. Having style triggers (onclick, onhover, etc...) would be useful.
1. If we want to change styling drastically, we would need to override the `Draw` method of the 
   component. 
	1. For instance, in material design, clicking a button results in a circle animation, 
	   implementing this would require adding an extra draw command to the button.
	1. In many cases though this can be worked around by adding more properties to each element.
	1. Alternatively, components and how they are drawn could be separated so that the drawing of 
	   an element is defined by it's style.
	1. This would might result in overly tight coupling between `UIElement`s and styles, making a 
	   new `UIElement` would require adding drawing code for it to the style system. How would a 
	   style know what kind of element it's drawing?

The basic approach to composable, reusable styles is to have a `Style` object which can be set on a
`UIElement`. The `Style` object would contain stylable properties (note that this creates some 
degree of coupling between `UIElement`s and the `Style`). At draw time, the `UIElement` checks it's
`Style` or parent styles and applies them.

With the current design, there's no way of knowing whether a property has been "overridden" or not,
this is an issue because when a style is applied it will override all properties on an object, 
meaning that to set an override on a single element a new `Style` needs to be created, setting the 
property directly would do nothing. In WPF this is solved with DependencyProperties which know when 
they are overridden but they are quite cumbersome.

Changing `Style` properties also implies invalidating dependent elements, this could be done with 
DataBinding, but would result in a lot of bindings, probably not very efficient? Maybe it's fine 
though if a `Style` property changes then the `UIElement` knows it needs to be invalidated it 
doesn't care what changed (exception being layout vs content).

### Idea

`UIElement`s have a `Style? Style` property. A `Style` instance declares a number of properties which
wrap the common properties of `UIElement`s which can be styled. These wrapped properties would be of 
type `Stylable<>` which would have similar semantics to `Nullable<>` while adding certain features:
 - Can be set to `Stylable<>.Default` to use the inherited value
 - Transitions for OnSet behaviour?
 - Predicate callback for conditionallly applying the property to elements?
 - On set, raises a property changed notification (or similar) which the `UIElement` the style is 
   bound to listens to. When the owning `UIElement` receives a property change notification in the 
   style, it then dispatches a call back to the style with itself and it's subtree of `UIElement`s,
   the style object then applies it's changes properties to the tree.

We need some way of efficiently binding `Style` properties to `UIElement` properties. One method could
be via source generation. When a child element is added to the hierarchy, styles need to be reevaluted.

```cs
public partial class Style
{
	// Base logic for styling
	// ...
}

public partial class Rectangle : UIElement
{
	/// <summary>
    /// The colour of this rectangle.
    /// </summary>
	[Reactive][Dirty(DirtyFlags.Content)]
	[Stylable(DocString: "This element's colour.")]
	private Vector4 Colour;
}

// Generates
public partial class Rectangle : UIElement
{
	/// <summary>
    /// The colour of this rectangle.
    /// </summary>
    public Vector4 Colour
    {
        get => colour; set
        {
            UpdateProperty(ref colour, value);
            Dirty(DirtyFlags.Content);
        }
    }
}

public partial class Style : ReactiveObject
{
	/// <summary>
    /// This element's colour.
    /// </summary>
    public Stylable<Vector4> Colour
    {
        get => colour; set
        {
			colour.Update(value);
			OnPropertyChanged(nameof(Colour));
            //UpdateProperty(ref colour, value);
        }
    }

	internal void Apply_Colour(UIElement element)
	{
		if (colour?.shouldSet(element) ?? true && colour.IsOverridden)
		{
			switch (element)
			{
				case Rectangle rectangle:
					rectangle.Colour = colour.Value;
					break;
				default:
					break;
			}
		}
	}
}
```

Some features of `Stylable<T>`:

```cs
public class Stylable<T>
{
	private readonly bool overridden;
	internal T value;
	private Predicate<UIElement>? shouldSet;
	private Transition? onSet;
	// This allows us to do some magic so that the following usage works:
	//   style.Colour = new Vector4(0.5);
	//   style.Colour.Transition = ...
	//   // When we next update Colour we want the metadata (ie: the transition, etc...)
	//   // to be preserved
	//   style.Colour = Vector4.One;
	//   // We can still completely override the stylable by making anew one ourselves
	//   style.Colour = new Stylable<Vector4>(Vector4.One, transition);
	private isImplict;
	
	public Stylable(T value)
	{
		this.isImplicit = false;
	    this.value = value;
	    overridden = true;
	}
	
	public readonly bool IsOverridden => overridden;
	
	public T Value
	{
	    get
	    {
	        if (!overridden)
	            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_NoValue();
	        return value;
	    }
		set
		{
			this.value = value;
			overridden = true;
			onSet?.Start(ref this);
		}
	}

	public Predicate<UIElement>? ShouldSet { get; set; }
	public Transition? Transition { get; set; }

	public void Update(in Stylable<T> value)
	{
		if (value.isImplicit)
			this.value = value.value;
		else
			this = value;
	}

	public static implicit operator Stylable<T>(T value)
	{
	    var ret = new Stylable<T>(value);
		ret.isImplicit = true;
	}
	
	public static explicit operator T(Stylable<T> value) => value!.Value;
}
```

## Idea Part 2 (Electric Boogaloo)
The `StylableAttribute` is a nice idea, but partial classes are only partial within a given 
assembly, so this idea doesn't work when user created components are involved. One possible
solution could be to use more source generation magic to make it work. Take the following 
(incomplete) example:
```cs
///////////////////////// Assembly: ArgonUI
public class UIElement
{
	public StyleSet? Style
	{
		get => ...
		set
		{
			style = value;
			// Styles need to know what objects they control, so they must be registered.
			value.Register(this);
		}
	}

	public void Dispose()
	{
		// Prevent leaks, unregister your style consumers today
		Style?.Unregister(this);
	}
}

// A single Style is always applied to all child elements indiscriminantly. Using a set of 
// selective styles makes it much easier to define complex styling behaviour at the style
// level rather than by manually updating the tree.
public class StyleSet : ICollection, ...
{
	private List<SelectiveStyle> styles;

	public record struct SelectiveStyle(Predicate selector, Style style);

	public void Register(UIElement) {...}
	public void Unregister(UIElement) {...}
}

public abstract class BaseStyle 
{ 
	// ??
	public void Register(UIElement) {...}
	public void Unregister(UIElement) {...}
}

public partial class Button : UIElement
{
	[Reactive, Dirty(Content), Stylable] public Vector4 bgColour;
}

//// Generates
public partial class Button { /* Reactive properties and stuff */ }

// A style class which contains all the properties of all stylable elements defined in this assembly
[GeneratedAssemblyStyle("ArgonUI")]
public partial class Style_ArgonUI : BaseStyle
{
	// Factory method for a new stylable prop
	public StylableProperty<Vector4> BgColour(Colour value)
	{
		return new(value, Apply_Background);
	}

	private void Apply_Background(UIElement elem, StylableProperty<Color> prop)
	{
		switch(elem)
		{
			case Button button:
			{
				button.Background = prop.Value;
				break;
			}
			default:
				throw ...
		}
	}
}

// An automatically generated internal class, which merges together all the 
// exported 'AssemblyStyle's referenced by this assembly.
// This is the magic that gets around the partial class limitations.
[GeneratedMergedStyle("ArgonUI", ...)]
internal partial class Style
{
	public Vector4 BgColour ...
}
////
```

## Idea Part 3 (Magic Tricyle)

With some of the above suggestions I have concerns about performance. We consider that most
users will want to set a small handful of StyleSets; for instance: a main application style 
set, a style set for a specialised panel, small style sets to dynamically override individual
components. The later could also be achieved by adding and removing individual rules from a
larger style set, both approaches should be efficient.

Well respected and performant UI frameworks such as QT, GTK, and JUCE implement styling by
having the Style class implement various drawing functions for different primitives which
widgets are composed of; the style class effectively draws the widget. There's more to how
they work than this description. I'm in two minds about this approach, while it offers 
ultimate flexibility when it comes to what a style can do, I feel the implementation of new
UI elements and styles can be more complex.

Here's an idea:
```cs
public class StyleSet : ICollection 
{
	// This effectively gives us a colection of collections of properties
	// the reason for this is that many properties are likely to share selectors,
	// so we can group them together into one style for efficiency.
	public Style[] Styles => ...
}

public class Style : ICollection 
{
	// We need to use IStylableProperty as StylableProperty<> is generic and as such needs boxing.
	public IStylableProperty this[int index] => ...

	public string[]? selectTags => ... // The list of tags a UIElement must have to be styled, all are required (AND)
	public Type[]? selectTypes => ... // The list of types this style applies to (OR)
	public StateFlags state => ... // (elem.state & state) == state
	// Need to work out when this should be allowed to be called...
	public Predicate? customSelector => ... // A custom selector, this is very expensive and shouldn't be used.
}

[Flags]
public enum StateFlags
{
	None = 0,
	Active = 1,
	Disabled = 2,
	Hovered = 4,
	Pressed = 8,
	Focussed = 16,

	Special0 = 256
	Special1 = 
	Special2 = 
	Special3 = 
	Special4 = 
	Special5 = 
	Special6 = 
	Special7 = 
}

public class StylableProperty<T> : IStylableProperty
{
	public T Value {get; set;}...
	internal Action<UIElement, T> ApplyFunc;

	...
}

[Autogenerated]
public class Style_ArgonUI
{
	// Factory method for a new stylable prop
	public StylableProperty<Colour> Background(Colour value)
	{
		return new(value, Apply_Background);
	}

	private void Apply_Background(UIElement elem, StylableProperty<Color> prop)
	{
		switch(elem)
		{
			case Button button:
			{
				button.Background = prop.Value;
				break;
			}
			default:
				throw ...
		}
	}
}

public void Main()
{
	var colAccent = ...;
	var colBg = ...;
	var colFg = ...;

	StyleSet styleSet = new([
		new([typeof(Button)], ["btn", "btn-primary"],[
			Style_ArgonUI.Background(colAccent),
			Style_ArgonUI.Foreground(colFg)
		])
	]);
}
```

```
When a stylable property's value is updated:

StylableProp<Color> = val
 |
 \/ StyleablePropChangedNotification(prop)
Style					// The foreach could be moved up to here by caching the list of selected elements
 |
 \/ StyleChangedNotification(style, prop)
StyleSet
 |
 \-> Foreach registered UIElement
	 |
	 \/
	 Apply selector (style)
	 |
	 \/
	 prop.Apply(uiElement)
```

## Idea Part 4: Floating Quadbike

Idea part 3 seems reasonable for the most part, but handling the hierarchical properties of styles
is complicated. Let's make the `StylableProp<T>` store a reference to it's parent `StylableProp<T>`
to make it easier to reapply parent styles. Does this make adding and removing styles more expensive?
```
// TODO: All this gets complicated, what if each StylableProp stored a link to it's parent property
// (must enforce lack of cycles!) to effectively make a big linked list so we don't have to search 
// the element tree every time.
```

Is this really simpler than having elements pull properties from styles on draw? Well, no, at the 
end of the day, elements need to know when they need to redraw based on changes to styles, and 
working in this direction gives us these redraw notifications for 'free'.

Considering all the update cases below the following data model makes sense:
```cs
class UIElement
{
	StyleSet Style;
}

enum TreeOperation
{
	Added,
	Removed
}

class StyleSet
{
	List<UIElement> ControlledElements;
	List<Style> Styles;

	// Get UI Tree updates from ui elements.
	// We care about updates that affect all elements directly under this style set and any updates 
	// which affect the parent style set.
	void OnTreeUpdate(UIElement target, TreeOperation op);
}

class Style
{
	ISelector Selector;
	List<StylableProp> Props;

	// If the selector rejects the majority of elements, then it makes sense to cache the 
	// selected elements. 
}

class StylableProp
{
	int NameHash;
}

```

#### Case 1A: StyleSet Added
Apply all styles to all controlled elements (and children? or are the children added as controlled 
elements?).

#### Case 1B: StyleSet Removed
When a styleset is removed it's likely many UI Elements will need to be updated. For this reason, 
it makes sense if a `StyleSet` knows what it's parent `StyleSet` is (in a linked list kind of way).

Working our way down the list of `StyleSet` in the hierarchy (from root to new leaf) apply each of 
the style sets to the elements controlled by the old style set.

#### Case 2A: Style Added
Effectively the same as case 1A. 

Depending on the cost of running the selector, it might make sense to cache the selected elements.

#### Case 2B: Style Removed
Effectively the same as case 1B except we then need to reapply the styles in the current style set
as well. This could be made more effecient by ANDing the parent selectors with the just removed
selectors, as we know those are the only elements which could have been touched.  
Even better if we could also know which properties were touched, but that's probs too memory 
intensive.

#### Case 3A: Prop Added
Effectively the same as 1A. We enumerate the selected elements and apply the property to each.

#### Case 3B: Prop Removed
We could do the effectively the same as 1B and traverse down the tree, re-applying styles.

Or if we can quickly test for a property's membership in a style, then it might be faster to work
our way up the tree until the property has been set. --> Yes, we can test this quickly, as we store
the property name hash in the property.

#### Case 3C: Prop Changed
Effectively the same as 3A.

#### Case 4A: UIElement Added
Find this element's controlling style set by going up the tree. Register itself and it's children.
Apply all the styles to itself and it's children.

Selectors which depend on sibling order, may need to be rerun -> styles re-applied.

#### Case 4B: UIElement Removed
Unregister this element and it's children from the controlling UI element.

Selectors which depend on sibling order, may need to be rerun -> styles re-applied.

#### Case 4C: UIElement Moved/Grafted
If a node is moved in the tree, or grafted (ie it's inserted as a none-leaf node) then this should
be treated as a deletion and an insertion.

```
A
|-> B
|-> C
|-> D
    |-> E
    |-> F
        |-> G

Move F under C: ==> Delete F -> Add F under C
A
|-> B
|-> C
    |-> F
        |-> G
|-> D
    |-> E

Move and insert B under D: ==> Delete B -> Delete E & F -> Add B under D -> Add E under B -> Addd F under B
A
|-> C
|-> D
    |-> B
        |-> E
        |-> F
            |-> G
```

#### Case 5A: UIElement Property Changed
Selectors depend on UIElement properties (such as tags). As such a selector should be able to 
subscribe to ui element property changes (this also implies listening to ui tree changes) and
trigger a style re-apply as needed.

#### Case 6A: Selector changed
A selector has been added or removed.

### Selector Updates
Selectors need to be able to tell the UI engine when they need to be re-evaluated.

Selectors:  
 - Tag selector
 - Type selector
 - Hover/click/focus selector
 - Hierarchy selector (even/odd, nth child, child of type)

To cover all of these cases we need: hierarchy update notifications (likely to cause the whole 
selector subtree to be re-evaluated); and element property change notifications (can only 
affect the changed element and its children). As such, all event notifications should bubble up
automatically; the `StyleSet` will subscribe to these on registration and pass these on to each
selector which can do what 


## Transitions

A `Transition` is effectively just an IEnumerable delegate. It is responsible for updating the 
given property as needed. It is invoked at the end of each `RenderFrame()` 

```cs
void RenderFrame()
{
	//...

	// Apply transitions
}

public abstract class Transition
{
	public IEnumerator OnFrame()
	{
		if (complete)
			yield return false;
		// Need to get time since start somehow?
		Colour = Lerp(...);
		yield return true;
	}
}
```

## Fonts

MSDF font rendering as in pySSV (https://github.com/space928/Shaders-For-Scientific-Visualisation/blob/main/pySSV/ssv_fonts.py).

Maybe make a dotnet tool to use https://github.com/Chlumsky/msdfgen to automatically generate
assets from TTFs and embed them in the assembly?
