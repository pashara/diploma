using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrailListner : MonoBehaviour
{
    // void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("OnCollision");
    // }
    [SerializeField] BoxCollider collider;
    Collider[] colliders = new Collider[20];
    [SerializeField] CarDriver car;

    public void CheckCollision()
    {
        Check();
    }


    [SerializeField] int mask;

    void Check()
    {
        List<ITrigger> triggers = null;
        
        int triggeredColliders = Physics.OverlapBoxNonAlloc(
            transform.position, 
            collider.size * 0.5f, 
            colliders, 
            transform.rotation, 
            1 << Layers.TRIGGERS
            );
        if (triggeredColliders > 0)
        {
            for(int i = 0; i < triggeredColliders; i++)
            {
                ITrigger trigger = colliders[i].GetComponent<ITrigger>();
                if (trigger != null)
                {
                    if (triggers == null)
                    {
                        triggers = new List<ITrigger>();
                    }
                    triggers.Add(trigger);
                }
            }
        }


        if (triggers != null)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                // Debug.Log("EEEEE");
                // Debug.Break();
                car.OnTriggerEnter(triggers[i].TriggerType, triggers[i]);
            }
        }
    }

    

    // void OnTriggerEnter(Collider c)
    // {
    //     Debug.Log("OnTriggerEnter");

    // }
}
