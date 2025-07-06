# ArgonUI.Freetype - C# Bindings for FreeType

This library provides low-level bindings to the [FreeType](https://freetype.org/index.html) library as 
well as some higher-level abstractions used by [ArgonUI](https://github.com/space928/ArgonUI).

## Usage

*TODO: Example usage script.*
```cs
```

## Building

The low-level bindings are generated using [ClangSharp](https://github.com/dotnet/ClangSharp/tree/main).
To regenerate the bindings follow the steps below:

1. Install the ClangSharp dotnet tool:
```sh
dotnet tool install --global ClangSharpPInvokeGenerator
```
2. Clone the FreeType repo:
```sh
git clone https://gitlab.freedesktop.org/freetype/freetype.git
```
3. Copy the [freetype_include.h](freetype_include.h) file into the `freetype/include/` directory.
4. Copy the [ClangSharpPInvokeGenerator.rsp](ClangSharpPInvokeGenerator.rsp) file into the `freetype/` 
   directory. This file contains all the command line arguments (one per line) to be passed to 
   ClangSharp.
5. Run the ClangSharpPInvokeGenerator:
```sh
ClangSharpPInvokeGenerator "@ClangSharpPInvokeGenerator.rsp"
```
6. Copy the bindings generated in `freetype/generated_bindings/` to `ArgonUI.FreeType/Bindings/`.
7. The generated bindings and high-level abstractions can be built by building the 
   `ArgonUI.FreeType.csproj` project in Visual Studio or using the dotnet CLI using 
   .netstandard2.0 or greater.


On Linux and MacOS, it is expected that the system will provide binaries for freetype (the bindings
look for `libfreetype.so.6`, `libfreetype.6.dylib`, or `freetype6.dll`). On Windows, this package 
ships precompiled FreeType binaries for Windows-X86 and Windows-X64.
