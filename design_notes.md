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
