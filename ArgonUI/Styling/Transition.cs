using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Styling;

public abstract class Transition
{
    public abstract IEnumerator OnFrame();
}
