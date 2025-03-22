# ArgonUI

<p align="center">
    <img src="https://github.com/space928/ArgonUI/blob/main/QPlayer/Resources/IconM.png?raw=true" alt="ArgonUI Logo" width="128" height="128">
</p>

[![Build Status](https://github.com/space928/ArgonUI/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/space928/ArgonUI/actions/workflows/dotnet.yml)
[![Build Status](https://github.com/space928/ArgonUI/actions/workflows/docs.yml/badge.svg?branch=main)](https://space928.github.io/ArgonUI/)
[![GitHub License](https://img.shields.io/github/license/space928/ArgonUI)](https://github.com/space928/ArgonUI/blob/main/LICENSE.txt)
[![Nuget](https://img.shields.io/nuget/v/ArgonUI?logo=Nuget&logoColor=fff)](https://www.nuget.org/packages/ArgonUI/)
![NuGet Downloads](https://img.shields.io/nuget/dt/ArgonUI)



ArgonUI is a fast cross-platform UI framework for .NET. It aims to aims to provide a fast and flexible UI 
framework, without all of the boilerplate.

**Features:**
 - A collection of common UI controls built in
 - Windows/Linux/Mac support
 - Support for multiple rendering backends
   - Efficient OpenGL based rendering backend
 - Event driven rendering, means elements or only rendered when they change
 - Flexible control styling system
 - Data binding

 <!--![Application screenshot](https://github.com/space928/QPlayer/assets/15130114/1a63eaaa-2c13-48e4-be0e-e33b5921bb41)-->

## Usage

For most users, it's easiest to simply intall the NuGet package:  
[![Nuget](https://img.shields.io/nuget/v/ArgonUI?logo=Nuget&logoColor=fff)](https://www.nuget.org/packages/ArgonUI/)
```
dotnet add package ArgonUI
```
Install a suitable rendering backend as well:  
[![Nuget](https://img.shields.io/nuget/v/ArgonUI.Backends.OpenGL?logo=Nuget&logoColor=fff)](https://www.nuget.org/packages/ArgonUI.Backends.OpenGL/)
```
dotnet add package ArgonUI.Backends.OpenGL
```

Using ArgonUI is as simple as creating an `ArgonManager` and a `UIWindow`:
```cs
using ArgonUI;
using ArgonUI.UIElements;
using ArgonUI.Backends.OpenGL;

namespace ArgonUI.Examples.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        ArgonManager argon = new();
        var wnd = argon.CreateOpenGLWindow();

        // Wait for the window to exit before returning
        wnd.Wait();
    }
}
```

Adding UI elements and subscribing to events is equally simple:
```
ArgonManager argon = new();
var wnd = argon.CreateOpenGLWindow();
var rect = new Rectangle();
var label = new Label();
wnd.RootElement.AddChild(rect);

int counter = 0;

wnd.RootElement.BGColour = new(0, 0.5f, 1, 1);
rect.Width = 100;
rect.Height = 100;
rect.Rounding = 5;
rect.Colour = new(0, .8f, .25f, 1);
rect.OnMouseDown += () =>
{
    label.Text = $"Hello World! {counter++}";
};
rect.OnMouseEnter += () =>
{
    rect.Colour = new(1f, .5f, .1f, 1);
    rect.Rounding = 20;
};
rect.OnMouseLeave += () =>
{
    rect.Colour = new(0, .8f, .25f, 1);
    rect.Rounding = 5;
};

wnd.RootElement.AddChild(label);
label.Text = "Hello World!";

wnd.Wait();
```

For more details see the [Documentation](https://space928.github.io/ArgonUI/).


## Building

ArgonUI can be built with Visual Studio 2022 using the .NET SDK 8.

See the [design_notes.md](design_notes.md) document for more details on the internal 
design of ArgonUI. (Note that this is a working document, and may not reflect the latest
changes)
