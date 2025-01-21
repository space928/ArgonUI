using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

public interface IUIWindow
{
    public string Title { get; set; }
    // Icon
    public IDrawContext DrawContext { get; }
    public VectorInt2 Size { get; set; }
    public VectorInt2 Position { get; set; }

    public event Action OnLoaded;
    public event Action OnClosing;
    public event Action OnResize;
    public event Action<float> OnRender;
    public event Action<IEnumerable<string>> OnFileDrop;

    public void Show();
    public void Minimize();
    public void Maximize();
    public void Close();
}
