# Data Binding

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
