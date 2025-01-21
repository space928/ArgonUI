using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Backends.OpenGL;

public static partial class ExtensionMethods
{
    public static VectorInt2 ToVectorInt2(this Vector2D<int> vec) => Unsafe.As<Vector2D<int>, VectorInt2>(ref vec);
    public static Vector2D<int> ToVector2D(this VectorInt2 vec) => Unsafe.As<VectorInt2, Vector2D<int>>(ref vec);
    public static VectorInt3 ToVectorInt3(this Vector3D<int> vec) => Unsafe.As<Vector3D<int>, VectorInt3>(ref vec);
    public static Vector3D<int> ToVector3D(this VectorInt3 vec) => Unsafe.As<VectorInt3, Vector3D<int>>(ref vec);

    public static IUIWindow CreateOpenGLWindow(this ArgonManager argon)
    {
        return new OpenGLWindow();
    }
}
