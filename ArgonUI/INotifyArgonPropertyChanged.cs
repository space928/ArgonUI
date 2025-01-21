using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArgonUI.INotifyArgonPropertyChanged;

namespace ArgonUI;

public interface INotifyArgonPropertyChanged : INotifyPropertyChanged, INotifyPropertyChanging
{
    void OnPropertyChanging(PropertyChangingEventArgs args);
    void OnPropertyChanged(PropertyChangedEventArgs args);

    //event ArgonPropertyChangedEventHandler PropertyChanged;
}

//public delegate void ArgonPropertyChangedEventHandler<T>(object? sender, ref T property, in T value, );
