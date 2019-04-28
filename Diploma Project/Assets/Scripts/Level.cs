using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    
    [SerializeField] Transform LBBound;
    [SerializeField] Transform RTBound;
    
    public Transform LBBoundTransform
    {
        get
        {
            return LBBound;
        }
    }

    public Transform RTBoundTransform
    {
        get
        {
            return RTBound;
        }
    }
    

    public void Initialize()
    {
        
    }
}
