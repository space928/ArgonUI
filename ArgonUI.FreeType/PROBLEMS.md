## Bindings are difficult...
msvc always treats `long`s as 32bit even on 64bit platforms.

gcc doesn't... For compat with linux/unix/osx we should expect platform specific `long`s.

meson can build with gcc (through msys?), but it's kinda annyoing to configure. We don't want the generated lib to depend on runtime libs we're not shipping so we need to configure meson with the following (`--buildtype=release -Dprefer_static=True -Dbrotli=disabled -Dbzip2=disabled -Dzlib=disabled -Dlibpng:brotli=disabled -Dlibpng:bzip2=disabled -Dlibpng:zlib=disabled` (for zlib we might be able to use internal)). As a bonus, the build file is broken, even with bzip2 disabled, it still seems to add a dependency to it if it finds it on your system (eg: `C:\Users\Public\msys64\msys64\mingw64\lib\libbz2.dll.a`).

Anyway, even with all that, .NET won't load the DLL, no idea why. So back to msvc...

We applied a small patch to `fttypes.h` (see the .patch file) to make `long`s 64bit on 64bit platforms.

cmake config:
```
cmake -B bin -D CMAKE_BUILD_TYPE=Release -D BUILD_SHARED_LIBS=true
cmake --build bin -j 4 --config Release
```

And in conclusion, even when fixing the `long` 32bit issue, the `FT_FaceRec_` struct still doesn't seem to line up with the c++ version (could be a packing thing). So, not sure how to fix, seems to be too much trouble...

