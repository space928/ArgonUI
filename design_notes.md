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
