using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.UIElements;

public interface IContainer
{
    // Must propagate PropertyChanged/OnDirty events from children
    // OnDraw invocations are propagated back down to children
    public ReadOnlyCollection<UIElement> Children { get; }
    public Vector4 InnerPadding { get; set; }
    public bool ClipContents { get; set; }

    public void AddChild(UIElement child);
    public void AddChildren(UIElement[] children);
    public void InsertChild(UIElement child, int index);

    public void RemoveChild(UIElement child);
    public void RemoveChildren(UIElement[] children);

    //internal void Layout();
    // Draw() must layout and draw all children
}
