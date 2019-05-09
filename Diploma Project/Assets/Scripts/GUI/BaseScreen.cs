using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
public class BaseScreen : MonoBehaviour
{
    #region Fields

    [SerializeField] ScreenType screenType;

    #endregion



    #region Properties

    public ScreenType ScreenType
    {
        get
        {
            return screenType;
        }
    }

    #endregion



    public virtual void Show(Action<BaseScreen> onStartShowing)
    {
        onStartShowing?.Invoke(this);
    }


    public virtual void Hide(Action<BaseScreen> onHided)
    {
        onHided?.Invoke(this);
    }
}
