using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTrigger : MonoBehaviour, ITrigger
{
    public TriggerType TriggerType
    {
        get
        {
            return TriggerType.Trail;
        }
    }

    public GameObject GameObject
    {
        get;
        set;
    }
}
