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



    public void Show(Action<BaseScreen> onStartShowing)
    {
        onStartShowing?.Invoke(this);
    }


    public void Hide(Action<BaseScreen> onHided)
    {
        onHided?.Invoke(this);
    }
}
