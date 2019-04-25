using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitializableMonobehaviour : MonoBehaviour
{
    public abstract void Initialize();


    public virtual void Deinitialize()
    {

    }
}
