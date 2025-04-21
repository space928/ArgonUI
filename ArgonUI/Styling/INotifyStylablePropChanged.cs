using System;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public interface INotifyStylablePropChanged
{
    public event Action<IStylableProperty> OnStylablePropChanged;
}
