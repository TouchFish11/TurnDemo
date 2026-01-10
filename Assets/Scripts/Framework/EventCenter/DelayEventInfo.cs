using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEventInfo
{
    public Action TriggerCallback { private get; set; }

    public Func<bool> Filter { private get; set; }

    public void Invoke()
    {
        if (Filter.Invoke())
        {
            TriggerCallback?.Invoke();
        }
    }
}
