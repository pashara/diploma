using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkBehaviour : NetworkBehaviour
{
    #region Helpers

    public enum ClientType
    {
        None = 0,
        Client = 1,
        Server = 2
    }

    #endregion


    #region Properties

    public ClientType CurrentClientType
    {
        get
        {
            ClientType result = ClientType.None;
            if (this.isClient)
            {
                result = ClientType.Client;
            }
            else if (this.isServer)
            {
                result = ClientType.Server;
            }
            return result;
        }
    }


    public bool IsLocalPlayer
    {
        get
        {
            return this.isLocalPlayer;
        }
    }

    #endregion
}
