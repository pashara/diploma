using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUserDataPharser : GlobalUserPharser
{

    #region Properties

    public override string Key
    {
        get
        {
            return "user_info";
        }
    }

    #endregion



    #region Protected methods

    protected override GlobalUserData Pharse(string value)
    {
        GlobalUserData result = null;   
        return result;
    }

    #endregion
}
