using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrailListner : MonoBehaviour
{
    [SerializeField] CarDriver car;

    public void CheckCollision()
    {
        //Check();
    }

    void OnTriggerEnter(Collider c)
    {
        ITrigger triggerInfo = c.GetComponent<ITrigger>();

        if (triggerInfo != null)
        {
            car.OnEnterTrigger(triggerInfo.TriggerType, triggerInfo);
        }

    }
}
