using ArgonUI.Helpers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ArgonUI.Backends.OpenGL;

// Adapted from https://github.com/dotnet/Silk.NET/blob/main/examples/CSharp/OpenGL%20Tutorials/Tutorial%202.1%20-%20Co-ordinate%20Systems/Shader.cs
public partial class Shader : IDisposable
{
    private readonly uint handle;
    private readonly GL gl;

    private readonly StringDict<int> uniformLocationCache = [];//Dictionary<string, int> uniformLocationCache = [];

    public Shader(GL gl, string vertexPath, string fragmentPath, string[]? defines = null)
    {
        this.gl = gl;

        uint vertex = LoadShader(ShaderType.VertexShader, vertexPath, defines);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath, defines);
        handle = this.gl.CreateProgram();
        this.gl.AttachShader(handle, vertex);
        this.gl.AttachShader(handle, fragment);
        this.gl.LinkProgram(handle);
        this.gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {this.gl.GetProgramInfoLog(handle)}");
        }
        this.gl.DetachShader(handle, vertex);
        this.gl.DetachShader(handle, fragment);
        this.gl.DeleteShader(vertex);
        this.gl.DeleteShader(fragment);
    }

    public void Use()
    {
        gl.UseProgram(handle);
    }

    public bool GetLocation(out int location, string name, bool silentFail)
    {
        if (uniformLocationCache.TryGetValue(name, out location))
            return true;

        location = gl.GetUniformLocation(handle, name);
        if (location == -1)
        {
            if (silentFail)
                return false;
            throw new Exception($"{name} uniform not found on shader.");
        }
        uniformLocationCache.Add(name, location);
        return true;
    }

    public void SetUniform(string name, int value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform1(location, value);
    }

    public void SetUniform(string name, bool value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform1(location, value ? 1f : 0f);
    }

    public void SetUniform(string name, Matrix4x4 value, bool silentFail = true)
    {
#if NETSTANDARD
        if (GetLocation(out int location, name, silentFail))
            gl.UniformMatrix4(location, 1, false, MemoryMarshal.Cast<Matrix4x4, float>(PolyFill.CreateReadOnlySpan(ref value, 1)));
#else
        if (GetLocation(out int location, name, silentFail))
            gl.UniformMatrix4(location, 1, false, MemoryMarshal.Cast<Matrix4x4, float>(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));
#endif
    }

    public void SetUniform(string name, float value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2 value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform2(location, value);
    }

    public void SetUniform(string name, Vector3 value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform3(location, value);
    }

    public void SetUniform(string name, Vector4 value, bool silentFail = true)
    {
        if (GetLocation(out int location, name, silentFail))
            gl.Uniform4(location, value);
    }

    public void Dispose()
    {
        gl.DeleteProgram(handle);
    }

    private uint LoadShader(ShaderType type, string path, string[]? defines)
    {
        string src;
        try
        {
            src = File.ReadAllText(path);
        }
        catch
        {
            // If we can't find the shader file locally, try loading it from embedded resources...
            using Stream stream = OpenGLWindow.LoadResourceFile(path);
            using StreamReader reader = new(stream);
            src = reader.ReadToEnd();
        }
        uint handle = gl.CreateShader(type);
        src = ApplyDefines(src, defines);
        gl.ShaderSource(handle, src);
        gl.CompileShader(handle);
        string infoLog = gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    /// <summary>
    /// Appends the given lines to the start of the shader; ensuring they 
    /// are inserted after the #version statement, on separate line.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="defines"></param>
    /// <returns></returns>
    private static string ApplyDefines(string source, string[]? defines)
    {
        if (defines == null || defines.Length == 0)
            return source;

        StringBuilder sb = new(source.Length);
#if !NETSTANDARD
        var versionStatement = FindVersionStatement().Match(source);
#else
        var versionStatement = Regex.Match(source, "#version.*");
        var multilineRegex = new Regex("\\/\\*(\\*(?!\\/)|[^*])*\\*\\/", RegexOptions.Compiled);
#endif
        if (versionStatement.Success)
        {
            // Need to insert defines after the #version and outside of any comments
            // Evil case: "/* foo */ #version 330 /* bar 
            //               baz */"
            var versionStart = versionStatement.Index;
            var versionEnd = versionStart + versionStatement.Value.Length + 1;
            sb.Append(source.AsSpan(0, versionEnd));

#if !NETSTANDARD
            var multilineComments = FindMultilineComment().Match(source, versionStart);
#else
            var multilineComments = multilineRegex.Match(source, versionStart);
#endif
            int end = versionEnd;

            while (multilineComments != null)
            {
                if (!multilineComments.Success)
                    break;

                if (multilineComments.Index > versionStart)
                {
                    if (multilineComments.Index < versionEnd)
                    {
                        // This multiline comment starts on the same line as the #version
                        int backTrack = versionEnd - multilineComments.Index;
                        sb.Remove(multilineComments.Index, backTrack); // Trim the chars stolen by the #version match
                        sb.AppendLine(); // Break line to start defines
                        end -= backTrack;
                    } 
                    break;
                }

                multilineComments = multilineComments.NextMatch();
            }

            foreach (var line in defines)
                sb.AppendLine(line);
            sb.AppendLine("#line 2"); // Not correct but close enough for now
            // Add the rest of the shader source
            sb.Append(source.AsSpan(end));
        }
        else
        {
            foreach (var line in defines)
                sb.AppendLine(line);
            sb.AppendLine("#line 1");
            sb.Append(source);
        }

        return sb.ToString();
    }

#if !NETSTANDARD
    [GeneratedRegex("#version.*")]
    private static partial Regex FindVersionStatement();

    [GeneratedRegex("\\/\\*(\\*(?!\\/)|[^*])*\\*\\/")]
    private static partial Regex FindMultilineComment();
#endif
}
