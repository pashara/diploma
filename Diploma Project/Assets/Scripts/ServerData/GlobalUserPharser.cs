using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalUserPharser
{
    #region Properties

    public virtual string Key
    {
        get;
    } 

    #endregion



    #region Public methods

    public GlobalUserData TryPharse(string key, string value)
    {
        GlobalUserData result = null;
        if (key.Equals(Key))
        {
            result = Pharse(value);
        }

        return result;
    }
    
    #endregion



    #region Protected methods

    protected abstract GlobalUserData Pharse(string value);

    #endregion
}