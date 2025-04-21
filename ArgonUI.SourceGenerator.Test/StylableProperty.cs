using ArgonUI.SourceGenerator.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Styling;

public interface IStylableProperty
{

}

public class StylableProp<T>
{
    public T? Value { get; }

    public StylableProp(T value, Action<UIElement, IStylableProperty> applyFunc)
    {
        Value = value;
    }
}
