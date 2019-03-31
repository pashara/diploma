using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum TriggerType
{
    None    =   0,
    Trail   =   1,
}
public interface ITrigger
{
    TriggerType TriggerType
    {
        get;
    }

    GameObject GameObject
    {
        get;
    }
}
