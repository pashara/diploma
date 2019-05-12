using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    #region Fields

    [SerializeField] Transform LBBound;
    [SerializeField] Transform RTBound;

    #endregion



    #region Properties

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

    #endregion

    public void Initialize()
    {
        
    }
}
