using ArgonUI.SourceGenerator.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Styling;

// This file mocks the StylableProp type and IStylableProperty interface so that this project doesn't need to depend on the ArgonUI assembly.

public interface IStylableProperty
{

}

public class StylableProp<T>
{
    public T? Value { get; }

    public StylableProp(T value, Action<UIElement, IStylableProperty> applyFunc, string name)
    {
        Value = value;
    }
}
